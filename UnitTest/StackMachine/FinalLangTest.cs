﻿using System;
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
            TestOnResourceCount(Resources.LangExample, 154);
        }

        [TestMethod]
        public void ParserTest()
        {
            StreamReader input = StringToStream(Resources.LangExample);
            List<Token> tokens = Lang.lexerLang.SearchTokens(input);
            tokens.RemoveAll((Token t) => t.Type.Name.Contains("CH_"));
            input.Close();
            ReportParser report = Lang.parserLang.Check(tokens);
            Console.WriteLine(string.Join("\n", report.Info));
            Assert.IsTrue(report.IsSuccess);
        }

        [TestMethod]
        public void CompileTest()
        {
            StreamReader input = StringToStream(Resources.LangExample);
            List<Token> tokens = Lang.lexerLang.SearchTokens(input);
            tokens.RemoveAll((Token t) => t.Type.Name.Contains("CH_"));
            input.Close();
            Console.WriteLine(Lang.parserLang.Check(tokens).Compile);
            List<string> Polish = Lang.parserLang.Compile(tokens);
            Console.WriteLine(string.Join("\n", Polish));
        }

        [TestMethod]
        public void ExecuteTest()
        {
            StreamReader input = StringToStream(Resources.LangExample);
            List<Token> tokens = Lang.lexerLang.SearchTokens(input);
            tokens.RemoveAll((Token t) => t.Type.Name.Contains("CH_"));
            input.Close();
            List<string> Polish = Lang.parserLang.Compile(tokens);
            Console.WriteLine(string.Join("\n", Polish));
            Lang.stackMachine.Execute(Polish);
            Assert.AreEqual(0, Lang.stackMachine.list.Count);
            Assert.AreEqual(1, Lang.stackMachine.Variables["test1"]);
            Assert.AreEqual(1, Lang.stackMachine.Variables["test2"]);
            Assert.AreEqual(1, Lang.stackMachine.Variables["test3"]);
            Assert.AreEqual(1, Lang.stackMachine.Variables["test4"]);
            Assert.AreEqual(1, Lang.stackMachine.Variables["test"]);
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
            List<Token> tokens = Lang.lexerLang.SearchTokens(input);
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
