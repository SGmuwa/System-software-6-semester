using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Lexer;
using static Parser.ExampleLang.CommandsList;
using Parser;

namespace Optimizing.Test
{
    [TestClass]
    public class TestsForOptimizingTest
    {
        [TestMethod]
        public void TestResx()
        {
            Assert.AreEqual("Hey! It is work!", Resources.ResxTest);
        }


        [TestMethod]
        public void CheckOptimizingSimple()
        {
            Assert.AreEqual($"a = 1 + 1{Environment.NewLine}print(a)", Resources.OptimizeFirst);
            var tokens = Lexer.ExampleLang.Lang.SearchTokens(StringToStream(Resources.OptimizeFirst));
            tokens.RemoveAll(t => t.Type.Name.StartsWith("CH_"));
            Console.WriteLine(string.Join("\n", tokens));
            var output = Parser.ExampleLang.Lang.Compile(
                tokens,
                Parser.ExampleLang.Lang.Check(tokens)
            );
            Console.WriteLine(string.Join(", ", output));
            CollectionAssert.AreEqual(new string[]{"a", "1", "1", Plus, Assign, "9", "a", "print", Goto, StackPopDrop}, output);
        }
        

        [DataTestMethod]
        [DataRow("OptimizeFirst", "a 2 = 7 a print goto! $stackPopDrop")]
        [DataRow("VarInVar", "a 3 = b 6 = 10 a print goto! $stackPopDrop 15 b print goto! $stackPopDrop")]
        [DataRow("VarVarInVar", "a 7 = b 14 = 10 a print goto! $stackPopDrop 15 b print goto! $stackPopDrop")]
        [DataRow("If", "1 6 !f a 1 = b 2 = 13 a print goto! $stackPopDrop 18 b print goto! $stackPopDrop")]
        public void OptimizingSimpleTest(string resourceName, string expect)
        {
            var output = CompileAndOptimizing(Resources.GetString(resourceName));
            CollectionAssert.AreEqual(expect.Split(' '), output);
        }

        [TestMethod]
        public void FinalLangTest()
        {
            /*
            StackMachine.ExampleLang.MyMachineLang stackMachine = ExecuteResource(Resources.LangExample);
            Assert.AreEqual(0, stackMachine.list.Count);
            Assert.AreEqual(1, stackMachine.Variables["test1"]);
            Assert.AreEqual(1, stackMachine.Variables["test2"]);
            Assert.AreEqual(1, stackMachine.Variables["test3"]);
            Assert.AreEqual(1, stackMachine.Variables["test4"]);
            Assert.AreEqual(1, stackMachine.Variables["test"]);
            */
        } 

        [TestMethod]
        public void TestFunctionCalculateNotOptimize()
        {
            StackMachinePrint stackMachine = ExecuteResource(Resources.FunctionCalculate, false);
            CollectionAssert.AreEqual(new double[] {100}, stackMachine.PrintHistory);
        }

        /*
        [TestMethod]
        public void TestFunctionCalculateOptimize()
        {
            StackMachinePrint stackMachine = ExecuteResource(Resources.FunctionCalculate, true);
            CollectionAssert.AreEqual(new double[] {100}, stackMachine.PrintHistory);
        }
        */


        [TestMethod]
        public void TestOp()
        {
            StackMachinePrint stackMachine = ExecuteResource(Resources.Op);
            CollectionAssert.AreEqual(new double[] {4}, stackMachine.PrintHistory);
        }

        private static (ReportParser, List<Token>, List<string>) Compile(string resourceBody)
        {
            List<Token> tokens;
            using(StreamReader stream = StringToStream(resourceBody))
                tokens = Lexer.ExampleLang.Lang.SearchTokens(stream);
            tokens.RemoveAll(t => t.Type.Name.StartsWith("CH_"));
            Console.WriteLine($":: tokens:\n{string.Join('\n', tokens)}");
            var reportParser = Parser.ExampleLang.Lang.Check(tokens);
            Console.WriteLine($":: preLang:\n{reportParser}");
            Assert.IsTrue(reportParser.IsSuccess, "Ошибка компиляции.");
            List<string> commands = Parser.ExampleLang.Lang.Compile(tokens, reportParser);
            Console.WriteLine($":: PreStackMachine:\n{string.Join(" ", commands)}");
            return (reportParser, tokens, commands);
        }

        private static List<string> CompileAndOptimizing(string resourceBody)
        {
            (ReportParser reportParser, List<Token> tokens, _) = Compile(resourceBody);
            reportParser = Example.AllOptimizing.Instance.Optimize(reportParser);
            Console.WriteLine($":: postLang:\n{reportParser}");
            Assert.IsTrue(reportParser.IsSuccess, "Ошибка компиляции.");
            var output = Parser.ExampleLang.Lang.Compile(tokens, reportParser);
            Console.WriteLine($":: optimizing:\n{string.Join("; ", output)}");
            return output;
        }

        private static StackMachinePrint ExecuteResource(string resource, bool needOptimize = true)
        {
            StackMachinePrint stackMachine = new StackMachinePrint();
            List<string> compiled = needOptimize ? CompileAndOptimizing(resource) : Compile(resource).Item3;
            stackMachine.Execute(compiled);
            return stackMachine;
        }

        private static StreamReader StringToStream(string resource)
        {
            return new StreamReader(
               new MemoryStream(
                   Encoding.UTF8.GetBytes(resource)
               ));
        }
    }
}