using System;
using Lexer;
using Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static UnitTest.LexerUnitTest;
using static Parser.RuleOperator;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class ParserUnitTest
    {
        /// <summary>
        /// Содержит нетерминалы среды тестирования.
        /// </summary>
        private readonly ParserLang parserLang = Parser.ExampleLang.Lang;
        /// <summary>
        /// Содержит терминалы среды тестирования.
        /// </summary>
        private readonly LexerLang lexerLang = Lexer.ExampleLang.Lang;

        [TestMethod]
        public void ParserOR_assign_op()
        {
            Nonterminal lang = new Nonterminal(OR, "ASSIGN_OP");
            CheckTest(Resources.ParserOR_assign_op, true, 1, new ParserLang(lang));
        }
        [TestMethod]
        public void ParserOR_assign_op2()
        {
            Nonterminal lang = new Nonterminal(OR, "абвгд", "ASSIGN_OP");
            CheckTest(Resources.ParserOR_assign_op, true, 1, new ParserLang(lang));
        }

        [TestMethod]
        public void ParserONE_AND_MORE_OR__ASSIGN_OP__VAR()
        {
            Nonterminal expr = new Nonterminal(OR, "ASSIGN_OP", "VAR");
            Nonterminal lang = new Nonterminal(ONE_AND_MORE, expr);
            CheckTest(Resources.ParserONE_AND_MORE_OR__ASSIGN_OP__VAR, true, 21, new ParserLang(lang));
        }

        [TestMethod]
        public void ParserOR_assign_op3()
            // Напомню: входная строка: "="
            => CheckTest(Resources.ParserOR_assign_op, false, 1);

        [TestMethod]
        public void Parser_assign_op_full()
            => CheckTest(Resources.Parser_assign_op_full, true, 17);
#pragma warning disable IDE1006 // Стили именования
        [TestMethod]
        public void _while()
            => CheckTest(Resources._while, true, 16);
#pragma warning restore IDE1006

        [TestMethod]
        public void Parser_var_op_while_print_var()
            => CheckTest(Resources.Parser_var_op_while_print_var, true, 21);

        [TestMethod]
        public void Parser_do_while()
            => CheckTest(Resources.Parser_do_while, true, 13);

        [TestMethod]
        public void Parser_function()
            => CheckTest(Resources.function, true, 14);

        [TestMethod]
        public void Parser_for()
            => CheckTest(Resources.Parser_for, true, 24);

        [TestMethod]
        public void Parser_finalLang()
        {
            ReportParser report = CheckTest(Resources.LangExample, true, 173);
            var all = string.Join(" ", from a in report.Compile select a.Current.ToString());
            Console.WriteLine(all);
            Assert.AreEqual(Resources.LangExampleJson,
                all);
        }

        /// <summary>
        /// Быстрое проведение тестирования <see cref="Parser.ParserLang"/>.
        /// Удаляет жетоны с CH_.
        /// </summary>
        /// <param name="resource">Текст тестирования.</param>
        /// <param name="isSuccess">True, если ожидается успех парсирования.</param>
        /// <param name="tokens">Количество ожидаемых жетонов. Установите -1 для игнорирования.</param>
        /// <param name="parser">Особые правила парсера. Оставьте null, если нужен язык <see cref="parserLang"/>.</param>
        /// <param name="lexer">Особые правила лексера. Оставьте null, если нужен язык <see cref="lexerLang"/>.</param>
        public ReportParser CheckTest(string resource, bool isSuccess = true, int tokens = -1, ParserLang parser = null, LexerLang lexer = null)
        {
            if (parser == null)
                parser = parserLang;
            if (lexer == null)
                lexer = lexerLang;
            List<Token> listT = lexer.SearchTokens(StringToStream(resource));
            Assert.IsNotNull(listT);
            Console.WriteLine("Count tokens: " + listT.Count);
            foreach (Token token in listT)
                Console.WriteLine(token);
            Console.WriteLine("\n ---- Without CH_:");
            listT.RemoveAll((Token t) => t.Type.Name.Contains("CH_"));
            Console.WriteLine("Count tokens: " + listT.Count);
            foreach (Token token in listT)
                Console.WriteLine(token);
            ReportParser report = parser.Check(listT);
            Console.WriteLine(report);
            if (tokens != -1)
                Assert.AreEqual(tokens, listT.Count);
            Assert.AreEqual(isSuccess, report.IsSuccess);
            return report;
        }
    }
}
