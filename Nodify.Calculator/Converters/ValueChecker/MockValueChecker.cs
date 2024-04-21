using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator.Converters.ValueChecker
{
    internal class MockValueChecker : ValueCheckerBase<int>
    {
        public override string ProcessInput(string input)
        {
            return input;
        }
    }
}
