using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lexer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parser;
using UnitTest;

namespace StackMachine.Test
{
    [TestClass]
    public class FinalLangTest
    {
        [TestMethod]
        public void LexerTest()
        {
            TestOnResourceCount(Resources.LangExample, 167);
        }

        [TestMethod]
        public void ParserTest()
        {
            StreamReader input = StringToStream(Resources.LangExample);
            List<Token> tokens = Lexer.ExampleLang.Lang.SearchTokens(input);
            tokens.RemoveAll((Token t) => t.Type.Name.Contains("CH_"));
            Console.WriteLine(string.Join("\n", tokens));
            input.Close();
            ReportParser report = Parser.ExampleLang.Lang.Check(tokens);
            Console.WriteLine(string.Join("\n", report.Info));
            Assert.IsTrue(report.IsSuccess);
        }

        [TestMethod]
        public void CompileTest()
        {
            StreamReader input = StringToStream(Resources.LangExample);
            List<Token> tokens = Lexer.ExampleLang.Lang.SearchTokens(input);
            tokens.RemoveAll((Token t) => t.Type.Name.Contains("CH_"));
            input.Close();
            var check = Parser.ExampleLang.Lang.Check(tokens);
            Console.WriteLine(check.Compile);
            Assert.IsTrue(check.IsSuccess);
            List<string> Polish = Parser.ExampleLang.Lang.Compile(tokens, check);
            Console.WriteLine(string.Join("\n", Polish));
        }

        [TestMethod]
        public void ExecuteTest()
        {
            ExampleLang.MyMachineLang stackMachine = ExecuteResource(Resources.LangExample);
            Assert.AreEqual(0, stackMachine.list.Count);
            Assert.AreEqual(1, stackMachine.Variables["test1"]);
            Assert.AreEqual(1, stackMachine.Variables["test2"]);
            Assert.AreEqual(1, stackMachine.Variables["test3"]);
            Assert.AreEqual(1, stackMachine.Variables["test4"]);
            Assert.AreEqual(1, stackMachine.Variables["test"]);
        }

        [TestMethod]
        public void ExecuteFunction()
        {
            StackMachinePrint stackMachine = ExecuteResource(Resources.function);
            Console.WriteLine(string.Join(" ", stackMachine.PrintHistory));
            CollectionAssert.AreEqual(new double[] {2}, stackMachine.PrintHistory);
        }

        public void ExecuteParser_SimpleFunction()
        {
            StackMachinePrint stackMachine = ExecuteResource(Resources.Parser_SimpleFunction);
            Console.WriteLine(string.Join(" ", stackMachine.PrintHistory));
            CollectionAssert.AreEqual(new double[] {1}, stackMachine.PrintHistory);
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

        private static StackMachinePrint ExecuteResource(string resource)
        {
            StackMachinePrint stackMachine = new StackMachinePrint();
            List<string> compiled = Compile(resource).Item3;
            stackMachine.Execute(compiled);
            return stackMachine;
        }

        /// <summary>
        /// Функция запускает тестирование на основание текста программы.
        /// Ожидается count терминалов, не считая терминалов CH_.
        /// </summary>
        /// <param name="text">Текст программы.</param>
        /// <param name="count">Количество ожидаемых терминалов.</param>
        private List<Token> TestOnResourceCount(string text, int count)
        {
            StreamReader input = StringToStream(text);
            List<Token> tokens = Lexer.ExampleLang.Lang.SearchTokens(input);
            tokens.RemoveAll((Token t) => t.Type.Name.Contains("CH_"));
            input.Close();
            foreach (Token token in tokens)
                // Печатаем жетоны.
                Console.WriteLine(token);
            Assert.AreEqual(count, tokens.Count);
            return tokens;
        }

        public static StreamReader StringToStream(string resource)
        {
            return new StreamReader(
               new MemoryStream(
                   Encoding.UTF8.GetBytes(resource)
               ));
        }
    }
}
