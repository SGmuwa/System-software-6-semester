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
            TestOnResourceCount(Resources.LangExample, 173);
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
            StreamReader input = StringToStream(Resources.LangExample);
            List<Token> tokens = Lexer.ExampleLang.Lang.SearchTokens(input);
            tokens.RemoveAll((Token t) => t.Type.Name.Contains("CH_"));
            input.Close();
            List<string> Polish = Parser.ExampleLang.Lang.Compile(tokens);
            Console.WriteLine(string.Join("\n", Polish));
            ExampleLang.StackMachine.Execute(Polish);
            Assert.AreEqual(0, ExampleLang.StackMachine.list.Count);
            Assert.AreEqual(1, ExampleLang.StackMachine.Variables["test1"]);
            Assert.AreEqual(1, ExampleLang.StackMachine.Variables["test2"]);
            Assert.AreEqual(1, ExampleLang.StackMachine.Variables["test3"]);
            Assert.AreEqual(1, ExampleLang.StackMachine.Variables["test4"]);
            Assert.AreEqual(1, ExampleLang.StackMachine.Variables["test"]);
        }

        /// <summary>
        /// Функция запускает тестирование на основание текста программы.
        /// Ожидается count терминалов, не считая терминалов CH_.
        /// </summary>
        /// <param name="text">Текст программы.</param>
        /// <param name="count">Количество ожидаемых терминалов.</param>
        public List<Token> TestOnResourceCount(string text, int count)
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
