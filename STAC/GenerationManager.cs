using LearnOpenTK.Common;
using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using STAC.Components;
using STAC.Components.CameraRay;
using STAC.Components.Color;
using STAC.Components.March;
using STAC.Components.March.Composite;
using STAC.Components.March.Composite.Parts;
using STAC.Components.Normals;
using STAC.Components.SDF;
using STAC.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC
{
    public class GenerationManager
    {
        public string MAX_MARCHING_STEPS = "255";
        public const string NEAR_PLANE_NAME = "MIN_DIST";
        public const string INFINITY = "1.0 / 0.0";
        public const string FAR_PLANE_NAME = "MAX_DIST";
        public const string EPSILON_NAME = "EPSILON";
        public const string SCREEN_SIZE_NAME = "ScreenSize";
        public const float FIELD_OF_FIEW = 60;
        public required GameWindow Window { get; init; }

        readonly HashSet<string> UsedGlobalIdentifiers = new();
        public MainComponent? MainComponent;
        public string GetUniqueGlobalIdentifier(string preferedIdentifier)
        {
            bool tryUseIdentifier(string i)
            {
                if (UsedGlobalIdentifiers.Contains(i))
                    return false;

                UsedGlobalIdentifiers.Add(i);
                return true;
            }


            if (tryUseIdentifier(preferedIdentifier))
                return preferedIdentifier;

            for (int i = 2; i < 100; i++)
            {
                string identifier = $"{preferedIdentifier}_{i}";
                if (tryUseIdentifier(identifier))
                    return identifier;
            }

            throw new NotImplementedException($"Identifier '{preferedIdentifier}' used too often and alternatives are exhausted! Handling this is not yet implemented.");
        }

        public string GetFragShaderSource()
        {
            MainComponent ??= GetMainComponentFromSource();

            InitializeComponents(MainComponent);

            string source = MainComponent.Generate();
            var formattedSource = HLSLFormatter.Format(source);
            if (!string.IsNullOrWhiteSpace(formattedSource))
                source = formattedSource;

            var numberedSource = AddLineNumbers(source);
            Console.WriteLine(numberedSource);

            return source;
        }

        public void InitializeComponents(IComponent root)
        {
            TraverseComponents(root, c => c.GenerationManager = this);
            TraverseComponents(root, c =>
            {
                foreach (var globalIdentifier in c.GetType()
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof(GlobalIdentifier))
                    .Select(p => (GlobalIdentifier)p.GetValue(c)!))
                {
                    if (globalIdentifier.Identifier != null)
                        continue;

                    string preferedName = globalIdentifier.PreferredIdentifier;
                    string allocatedName = GetUniqueGlobalIdentifier(preferedName);
                    globalIdentifier.Identifier = allocatedName;
                }
            });
            TraverseComponents(root, c => c.Initialize());
        }

        private static MainComponent GetMainComponentFromSource()
        {
            return PipelineProvider.MCDemoMaterialsShaded();
        }

        public string AdditionalFunctionDefinitions { get; private set; } = "";

        public void AddAdditionalGlobalDefinition(string functionDefinition)
        {
            AdditionalFunctionDefinitions += Environment.NewLine + functionDefinition;
        }
        
        public string PostHitCalls { get; private set; } = "";
        public void AddPostHitCall(string call)
        {
            PostHitCalls += Environment.NewLine + call;
        }


        public void OnPostCompilation(Shader shader)
        {
            ArgumentNullException.ThrowIfNull(MainComponent);
            TraverseComponents(MainComponent, c => { if (c is IPostCompilationHandler handler) handler.OnPostCompilation(shader); });
        }

        static string AddLineNumbers(string input)
        {
            var lines = input.Split(Environment.NewLine);
            string format = "D" + lines.Length.ToString().Length;
            return string.Join(Environment.NewLine, lines.Select((line, i) => $"{i.ToString(format)} {line}"));
        }

        private static void TraverseComponents(IComponent generator, Action<IComponent> action)
        {
            HashSet<IComponent> processed = new();
            Stack<IComponent> components = new();
            components.Push(generator);
            while (components.Count > 0)
            {
                var component = components.Pop();
                if (processed.Contains(component))
                    continue;
                processed.Add(component);

                action(component);
                foreach (var item in component.Components)
                {
                    if (item != null)
                        components.Push(item);
                }
            }
        }
    }
}
