using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator.Converters.ValueChecker
{
    public interface IValueChecker
    {
        public object ParsedObject { get; set; }
        public string ProcessInput(string input);
    }
}
