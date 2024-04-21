using System;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Windows.Input;

namespace Nodify.Calculator
{
    public class ApplicationViewModel : ObservableObject
    {
        public NodifyObservableCollection<EditorViewModel> Editors { get; } = new NodifyObservableCollection<EditorViewModel>();

        public ApplicationViewModel()
        {
            AddEditorCommand = new DelegateCommand(() => Editors.Add(new EditorViewModel
            {
                Name = $"Editor {Editors.Count + 1}"
            }));

            CloseEditorCommand = new DelegateCommand<Guid>(
                id => Editors.RemoveOne(editor => editor.Id == id),
                _ => Editors.Count > 0 && SelectedEditor != null);

            CloneCommand = new DelegateCommand<Guid>((id) =>
            {
                EditorViewModel source = Editors.First(e => e.Id == id);
                EditorViewModel clone = new()
                {
                    Name = source.Name + "(clone)",
                    Calculator = source.Calculator.Clone()
                };

                Editors.Add(clone);
            });


            Editors.WhenAdded((editor) =>
            {
                if (AutoSelectNewEditor || Editors.Count == 1)
                {
                    SelectedEditor = editor;
                }
            })
            .WhenRemoved((editor) =>
            {
                var childEditors = Editors.Where(ed => ed.Parent == editor).ToList();
                childEditors.ForEach(ed => Editors.Remove(ed));
            });
            Editors.Add(new EditorViewModel
            {
                Name = $"Editor {Editors.Count + 1}"
            });
        }

        public ICommand AddEditorCommand { get; }
        public ICommand CloseEditorCommand { get; }
        public ICommand CloneCommand { get; }

        private EditorViewModel? _selectedEditor;
        public EditorViewModel? SelectedEditor
        {
            get => _selectedEditor;
            set => SetProperty(ref _selectedEditor, value);
        }

        private bool _autoSelectNewEditor = true;
        public bool AutoSelectNewEditor
        {
            get => _autoSelectNewEditor;
            set => SetProperty(ref _autoSelectNewEditor , value); 
        }
    }
}
