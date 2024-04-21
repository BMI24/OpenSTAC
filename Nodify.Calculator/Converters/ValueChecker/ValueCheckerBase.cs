using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator.Converters.ValueChecker
{
    internal abstract class ValueCheckerBase<T> : IValueChecker where T:struct
    {
        public object ParsedObject { get; set; } = default(T);
        protected T ParsedValue 
        {
            get => (T)ParsedObject!;
            set => ParsedObject = value!;
        }
        public abstract string ProcessInput(string input);
    }
}
