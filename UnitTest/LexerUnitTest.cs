using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lexer;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class LexerUnitTest
    {
        private readonly LexerLang lang = ExampleLang.Lang;

#pragma warning disable IDE1006 // Стили именования
        /// <summary>
        /// Тестирование assign_op.txt
        /// </summary>
        [TestMethod]
        public void assign_op()
        {
            var tokens = TestOnResourceCount(Resources.assign_op, 3);
            Assert.IsTrue(tokens[0].Type == ExampleLang.VAR);
            Assert.IsTrue(tokens[0].Value == "a");
            Assert.IsTrue(tokens[1].Type == ExampleLang.ASSIGN_OP);
            Assert.IsTrue(tokens[1].Value == "=");
            Assert.IsTrue(tokens[2].Type == ExampleLang.DIGIT);
            Assert.IsTrue(tokens[2].Value == "2");
        }

        [TestMethod]
        public void assign_op_multiline()
        {
            IList<Token> tokens = TestOnResourceCount(Resources.assign_op_multiline, 6);
            Assert.IsTrue(tokens[0].Type == ExampleLang.VAR);
            Assert.IsTrue(tokens[0].Value == "Abs");
            Assert.IsTrue(tokens[1].Type == ExampleLang.ASSIGN_OP);
            Assert.IsTrue(tokens[1].Value == "=");
            Assert.IsTrue(tokens[2].Type == ExampleLang.DIGIT);
            Assert.IsTrue(tokens[2].Value == "3");

            Assert.IsTrue(tokens[3].Type == ExampleLang.VAR);
            Assert.IsTrue(tokens[3].Value == "Tri");
            Assert.IsTrue(tokens[4].Type == ExampleLang.ASSIGN_OP);
            Assert.IsTrue(tokens[4].Value == "=");
            Assert.IsTrue(tokens[5].Type == ExampleLang.DIGIT);
            Assert.IsTrue(tokens[5].Value == "4");
        }

        [TestMethod]
        public void op()
        {
            TestOnResourceCount(Resources.op, 11);
        }

        [TestMethod]
        public void while_kw(){
            TestOnResourceCount(Resources._while, 16);
        }



        [TestMethod]
        public void print_kw()
        {
            List<Token> tokens = TestOnResourceCount(Resources.print_kw, 1);

            Assert.AreEqual("PRINT_KW", tokens[0].Type.Name);

        }
        [TestMethod]
        public void condition()
        {
            TestOnResourceCount(Resources.condition, 27);
        }
        [TestMethod]
        public void cycle_for()
        {
            IList<Token> tokens = TestOnResourceCount(Resources.cycle_for, 29);
            int i = 0;
            Assert.IsTrue(tokens[i].Type == ExampleLang.VAR);
            Assert.IsTrue(tokens[i++].Value == "n");
            Assert.IsTrue(tokens[i].Type == ExampleLang.ASSIGN_OP);
            Assert.IsTrue(tokens[i++].Value == "=");
            Assert.IsTrue(tokens[i].Type == ExampleLang.DIGIT);
            Assert.IsTrue(tokens[i++].Value == "2");

            Assert.IsTrue(tokens[i].Type == ExampleLang.VAR);
            Assert.IsTrue(tokens[i++].Value == "b");
            Assert.IsTrue(tokens[i].Type == ExampleLang.ASSIGN_OP);
            Assert.IsTrue(tokens[i++].Value == "=");
            Assert.IsTrue(tokens[i].Type == ExampleLang.DIGIT);
            Assert.IsTrue(tokens[i++].Value == "0");

            Assert.IsTrue(tokens[i].Type == ExampleLang.FOR_KW);
            Assert.IsTrue(tokens[i++].Value == "for");
            Assert.IsTrue(tokens[i].Type == ExampleLang.L_B);
            Assert.IsTrue(tokens[i++].Value == "(");
            Assert.IsTrue(tokens[i].Type == ExampleLang.VAR);
            Assert.IsTrue(tokens[i++].Value == "a");
            Assert.IsTrue(tokens[i].Type == ExampleLang.ASSIGN_OP);
            Assert.IsTrue(tokens[i++].Value == "=");
            Assert.IsTrue(tokens[i].Type == ExampleLang.DIGIT);
            Assert.IsTrue(tokens[i++].Value == "0");
            Assert.IsTrue(tokens[i].Type == ExampleLang.COMMA);
            Assert.IsTrue(tokens[i++].Value == ";");
            Assert.IsTrue(tokens[i].Type == ExampleLang.VAR);
            Assert.IsTrue(tokens[i++].Value == "a");
            Assert.IsTrue(tokens[i].Type == ExampleLang.OP);
            Assert.IsTrue(tokens[i++].Value == "<");
            Assert.IsTrue(tokens[i].Type == ExampleLang.VAR);
            Assert.IsTrue(tokens[i++].Value == "n");
            Assert.IsTrue(tokens[i].Type == ExampleLang.COMMA);
            Assert.IsTrue(tokens[i++].Value == ";");
            Assert.IsTrue(tokens[i].Type == ExampleLang.VAR);
            Assert.IsTrue(tokens[i++].Value == "a");
            Assert.IsTrue(tokens[i].Type == ExampleLang.ASSIGN_OP);
            Assert.IsTrue(tokens[i++].Value == "=");
            Assert.IsTrue(tokens[i].Type == ExampleLang.VAR);
            Assert.IsTrue(tokens[i++].Value == "a");
            Assert.IsTrue(tokens[i].Type == ExampleLang.OP);
            Assert.IsTrue(tokens[i++].Value == "+");
            Assert.IsTrue(tokens[i].Type == ExampleLang.DIGIT);
            Assert.IsTrue(tokens[i++].Value == "1");
            Assert.IsTrue(tokens[i].Type == ExampleLang.R_B);
            Assert.IsTrue(tokens[i++].Value == ")");
            Assert.IsTrue(tokens[i].Type == ExampleLang.L_QB);
            Assert.IsTrue(tokens[i++].Value == "{");
            Assert.IsTrue(tokens[i].Type == ExampleLang.VAR);
            Assert.IsTrue(tokens[i++].Value == "b");
            Assert.IsTrue(tokens[i].Type == ExampleLang.ASSIGN_OP);
            Assert.IsTrue(tokens[i++].Value == "=");
            Assert.IsTrue(tokens[i].Type == ExampleLang.VAR);
            Assert.IsTrue(tokens[i++].Value == "b");
            Assert.IsTrue(tokens[i].Type == ExampleLang.OP);
            Assert.IsTrue(tokens[i++].Value == "+");
            Assert.IsTrue(tokens[i].Type == ExampleLang.DIGIT);
            Assert.IsTrue(tokens[i++].Value == "1");
            Assert.IsTrue(tokens[i].Type == ExampleLang.R_QB);
            Assert.IsTrue(tokens[i++].Value == "}");
        }
#pragma warning restore IDE1006 // Стили именования
        /// <summary>
        /// Функция запускает тестирование на основание текста программы.
        /// Ожидается count терминалов, не считая терминалов CH_.
        /// </summary>
        /// <param name="text">Текст программы.</param>
        /// <param name="count">Количество ожидаемых терминалов.</param>
        public List<Token> TestOnResourceCount(string text, int count)
        {
            StreamReader input = StringToStream(text);
            List<Token> tokens = lang.SearchTokens(input);
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
