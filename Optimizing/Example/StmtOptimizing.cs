using Parser;
using Lexer;
using System.Linq;
using System.Collections.Generic;
using MyTypes.Tree;
using System;

namespace Optimizing.Example
{
    /// <summary>
    /// Заранее вычисляет stmt.
    /// </summary>
    public class StmtOptimizing : IOptimizing
    {
        /// <summary>
        /// Получает готовый экземпляр <see cref="StmtOptimizing"/> из кэша.
        /// </summary>
        public static readonly StmtOptimizing Instance = new StmtOptimizing();
        private static readonly Random ran = new Random();

        private StmtOptimizing() { }

        /// <summary>
        /// Оптимизирует входное дерево компиляции.
        /// </summary>
        /// <param name="input">Входное дерево компиляции.</param>
        /// <returns>Оптимизированное дерево компиляции.</returns>
        public ReportParser Optimize(ReportParser compiledCode)
        {
            if (!compiledCode.IsSuccess)
                throw new OptimizingException("Входное дерево компиляции построено не верно!");
            if (compiledCode.Compile == null)
                throw new OptimizingException("Вызовите compiledCode.Compile() перед началом.");
            ITreeNode<object> outputCompile = compiledCode.Compile.CloneCompileTree();

            var stmts = from a in outputCompile
                        where a.Current is ParserToken rpc && (rpc.Source == Parser.ExampleLang.stmt || rpc.Source == Parser.ExampleLang.assign_expr)
                        select a;

            Dictionary<string, double> varsValues = new Dictionary<string, double>();

            foreach (var stmt in stmts)
            {
                string stmtResult = TryCalculate(stmt, varsValues);
                if (stmtResult != null)
                {
                    ParserToken current = (ParserToken)stmt.Current;
                    if (current.Source == Parser.ExampleLang.stmt && double.TryParse(stmtResult, out _))
                    {
                        stmt[0].Current =
                            new Token(Lexer.ExampleLang.DIGIT, stmtResult, ran.NextULong());
                    }
                }
            }
            return new ReportParser(outputCompile);
        }

        private string TryCalculate(ITreeNode<object> assign_expr, Dictionary<string, double> varsValues)
        {
            List<string> commands = Parser.ExampleLang.Lang.Compile((from a in assign_expr where a.Current is Token t select (Token)a.Current).ToList(),
                new ReportParser(assign_expr));
            Dictionary<string, double> toSend = new Dictionary<string, double>(varsValues);
            foreach (string varName in from a in assign_expr where a.Current is Token tk && tk.Type == Lexer.ExampleLang.VAR && !toSend.ContainsKey(tk.Value) select ((Token)a.Current).Value)
            {
                toSend[varName] = double.NaN;
            }
            OptimizingStackMachine localStackMachine = new OptimizingStackMachine(toSend);
            try
            {
                localStackMachine.Execute(commands);
            }
            catch { return null; }
            return localStackMachine.GetLastValueStack();
        }

        class OptimizingStackMachine : StackMachine.ExampleLang.MyMachineLang
        {
            public OptimizingStackMachine(IDictionary<string, double> startVariables = null) : base(startVariables)
            {
            }

            public string GetLastValueStack()
            {
                string output = null;
                while (Stack.TryPop(out string b)) output = b;
                return output;
            }
        }
    }
}