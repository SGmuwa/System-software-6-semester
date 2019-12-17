using StackMachine;
using System.Collections.Generic;

namespace Optimizing.Test
{
    class StackMachinePrint : ExampleLang.MyMachineLang
    {
        public readonly List<double> PrintHistory = new List<double>();

        protected override int Print(double value)
        {
            PrintHistory.Add(value);
            return base.Print(value);
        }
    }
}