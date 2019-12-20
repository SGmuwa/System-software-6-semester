using StackMachine;
using System.Collections.Generic;

namespace UnitTest
{
    class StackMachinePrint : ExampleLang.MyMachineLang
    {
        public readonly List<double> PrintHistory = new List<double>();

        private object sync = new object();

        protected override int Print(double value)
        {
            lock(sync)
            {
                PrintHistory.Add(value);
            }
            return base.Print(value);
        }
    }
}