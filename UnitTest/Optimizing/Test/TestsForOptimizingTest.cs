using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Lexer;

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
            Assert.AreEqual($"a = 1 + 1{Environment.NewLine}print", Resources.OptimizeFirst);
            var tokens = Lexer.ExampleLang.Lang.SearchTokens(StringToStream(Resources.OptimizeFirst));
            tokens.RemoveAll(t => t.Type.Name.StartsWith("CH_"));
            Console.WriteLine(string.Join("\n", tokens));
            var output = Parser.ExampleLang.Lang.Compile(
                tokens,
                Parser.ExampleLang.Lang.Check(tokens)
            );
            Console.WriteLine(string.Join(", ", output));
            CollectionAssert.AreEqual(new string[]{"a", "1", "1", "+", "=", "print"}, output);
        }
        

        [DataTestMethod]
        //[DataRow("OptimizeFirst", "a 2 = print")]
        //[DataRow("VarInVar", "a 3 = b 6 = print")]
        [DataRow("VarVarInVar", "a 7 = b 14 = print")]
        //[DataRow("If", "1 6 !f a 1 = b 2 = print")]
        public void OptimizingSimple(string resourceName, string expect)
        {
            var output = CompileAndOptimizing(Resources.GetString(resourceName));
            CollectionAssert.AreEqual(expect.Split(' '), output);
        }

        private static List<string> CompileAndOptimizing(string resourceBody)
        {
            List<Token> tokens;
            using(StreamReader stream = StringToStream(resourceBody))
                tokens = Lexer.ExampleLang.Lang.SearchTokens(stream);
            tokens.RemoveAll(t => t.Type.Name.StartsWith("CH_"));
            Console.WriteLine($"tokens:\n{string.Join('\n', tokens)}");
            var checkedTokens = Parser.ExampleLang.Lang.Check(tokens);
            Console.WriteLine($"preLang:\n{checkedTokens}");
            Console.WriteLine($"PreStackMachine:\n{string.Join(" ", Parser.ExampleLang.Lang.Compile(tokens, checkedTokens))}");
            Assert.IsTrue(checkedTokens.IsSuccess, "Ошибка компиляции.");
            var output = Parser.ExampleLang.Lang.Compile(
                tokens,
                Example.AllOptimizing.Instance.Optimize(
                    checkedTokens
                )
            );
            Console.WriteLine($"optimizing:\n{string.Join(" ", output)}");
            return output;
        }

        [TestMethod]
        public void FinalLangTest()
        {
            StreamReader input = StringToStream(Resources.LangExample);
            List<Token> tokens = Lexer.ExampleLang.Lang.SearchTokens(input);
            tokens.RemoveAll((Token t) => t.Type.Name.Contains("CH_"));
            input.Close();
            List<string> Polish = Parser.ExampleLang.Lang.Compile(
                tokens,
                Example.AllOptimizing.Instance.Optimize(
                    Parser.ExampleLang.Lang.Check(tokens)
                )
            );
            Console.WriteLine(string.Join("\n", Polish));
            StackMachine.ExampleLang.StackMachine.Execute(Polish);
            Assert.AreEqual(0, StackMachine.ExampleLang.StackMachine.list.Count);
            Assert.AreEqual(1, StackMachine.ExampleLang.StackMachine.Variables["test1"]);
            Assert.AreEqual(1, StackMachine.ExampleLang.StackMachine.Variables["test2"]);
            Assert.AreEqual(1, StackMachine.ExampleLang.StackMachine.Variables["test3"]);
            Assert.AreEqual(1, StackMachine.ExampleLang.StackMachine.Variables["test4"]);
            Assert.AreEqual(1, StackMachine.ExampleLang.StackMachine.Variables["test"]);
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