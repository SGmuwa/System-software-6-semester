using System.Collections.Generic;
using Lexer;
using Parser;

namespace Optimizing
{
    class Program
    {
        static void Main(string[] args)
        {
            (IList<Token>, ReportParser) tokensReport = StackMachine.Program.GetParserReportFromUser(args);
            tokensReport.Item2 = Example.AllOptimizing.Instance.Optimize(tokensReport.Item2);
            StackMachine.Program.CompileAndExecute(tokensReport);
        }
    }
}
