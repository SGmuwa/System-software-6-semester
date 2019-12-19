using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using Lexer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parser;
using StackMachine;
using static Parser.RuleOperator;

namespace UnitTest
{
    [TestClass]
    public class StackMachineTest
    {
        private readonly LexerLang EasyLexerLang;
        private readonly ParserLang EasyParserLang;
        private readonly ParserLang SuperEasyParserLang;
        private readonly AbstractStackExecuteLang EasyStackLang;

        public StackMachineTest()
        {
            EasyLexerLang = new LexerLang(new Terminal[]{
                new Terminal("ASSIGN_OP", "^=$"),
                new Terminal("VAR", "^[a-zA-Z]+$", uint.MaxValue),
                new Terminal("DIGIT", "^0|([1-9][0-9]*)$"),
                new Terminal("OP", "^\\+|-|\\*|/$"),
                new Terminal("WHILE_KW", "^while$", 0),
                new Terminal("PRINT_KW", "^print$", 0),
                new Terminal("L_QB", "^{$"),
                new Terminal("R_QB", "^}$"),
                new Terminal("L_B", "^\\($"),
                new Terminal("R_B", "^\\)$"),

                new Terminal("CH_SPACE", "^ $"),
                new Terminal("CH_LEFTLINE", "^\r$"),
                new Terminal("CH_NEWLINE", "^\n$"),
                new Terminal("CH_TAB", "^\t$")
            });

            /*
             * Правила стековой машины:
             * Все команды выполняются в постфиксной записи.
             * 
             * {value logical, value addr, "if", com1}
             * Где:
             * logical - если 0, то переход к com1. Ложь - переход к адресу addr.
             * addr - адрес перехода.
             * 
             * {value addr, "goto"}
             * Переход к адресу addr.
             * 
             */
             
            Nonterminal lang = new Nonterminal("lang",
                (List<string> commands, ActionInsert insert, int id) =>
                {
                    for(int i = 0; i < id; i++)
                        insert(i);
                }, ZERO_AND_MORE);
            Nonterminal value = new Nonterminal("value",
                (List<string> commands, ActionInsert insert, int id) =>
                {
                    insert();
                }, OR, "VAR", "DIGIT");
            Nonterminal stmt = new Nonterminal("stmt",
                (List<string> commands, ActionInsert insert, int id) =>
                {
                    insert(1);
                    insert(0);
                }, AND,
                value, new Nonterminal("(OP value)*",
                (List<string> commands, ActionInsert insert, int id) =>
                {
                    for (int i = 0; i < id; i++)
                        insert(i);
                }, ZERO_AND_MORE, new Nonterminal("OP & value",
                (List<string> commands, ActionInsert insert, int id) =>
                {
                    insert(1);
                    insert(0);
                }, AND, "OP", value)));
            Nonterminal assign_expr = new Nonterminal("assign_expr",
                    (List<string> commands, ActionInsert insert, int id) =>
                    {
                        insert(0);
                        insert(2);
                        insert(1);
                    }, AND,
                    "VAR", "ASSIGN_OP", stmt);
            Nonterminal while_expr = new Nonterminal("while_expr",
                // Нужно преобразовать в стековый код.
                (List<string> commands, ActionInsert insert, int id) =>
                {
                    insert(2);
                    commands.Add("?"); // Адрес с истиной.
                    int indexAddrTrue = commands.Count - 1;
                    commands.Add("if");
                    commands.Add("?"); // Адрес с ложью.
                    int indexAddrFalse = commands.Count - 1;
                    commands.Add("goto");
                    // Сюда надо попасть, если true. 
                    commands[indexAddrTrue] = commands.Count.ToString();
                    insert(5); // Тело while.
                    commands.Add(indexAddrTrue.ToString());
                    commands.Add("goto"); // Команда перехода в if к while.
                    // Надо выйти из цикла, если false:
                    commands[indexAddrFalse] = commands.Count.ToString();
                }, AND,
                "WHILE_KW", "L_B", stmt, "R_B", "L_QB", lang, "R_QB");
            Nonterminal expr = new Nonterminal("expr",
                // Нужно преобразовать в стековый код.
                (List<string> commands, ActionInsert insert, int id) =>
                {
                    switch (id)
                    {
                        case 0:
                            {
                                insert();
                            }
                            break;
                        case 1:
                            {
                                insert();
                            }
                            break;
                        case 2:
                            {
                                commands.Add("print");
                            }
                            break;
                    }
                }, OR,
                assign_expr, while_expr, "PRINT_KW");
            lang.Add(expr);
            EasyParserLang = new ParserLang(lang);

            SuperEasyParserLang = new ParserLang(new Nonterminal("easy lang",
                (List<string> commands, ActionInsert insert, int id) =>
                {
                    insert(0);
                    insert(2);
                    insert(1);
                }, AND, "VAR", "ASSIGN_OP", "DIGIT"));

            EasyStackLang = new MyEasyStackLang();
        }

        [TestMethod]
        public void TestMethod1()
        {
            List<Token> tokens = EasyLexerLang.SearchTokens(StringToStream(Resources.Stack_var_print));
            tokens.RemoveAll((s) => s.Type.Name.Contains("CH_"));
            tokens.WriteAll();
            Console.WriteLine(EasyParserLang.Check(tokens).Compile);
            List<string> StackCode = EasyParserLang.Compile(tokens);
            StackCode.WriteAll();
            CollectionAssert.AreEqual(new string[] { "a", "2", "=", "print" }, StackCode);
            Console.WriteLine("Выполнение стековой машины...");
            EasyStackLang.Execute(StackCode);
            Console.WriteLine("Выполнение стековой машины завершено.");
            Assert.AreEqual(1, EasyStackLang.Variables.Count);
        }

        [TestMethod]
        public void TestSuperEasyLang()
        {
            List<Token> tokens = EasyLexerLang.SearchTokens(StringToStream("a=2"));
            tokens.RemoveAll((s) => s.Type.Name.Contains("CH_"));
            tokens.WriteAll();
            Console.WriteLine(SuperEasyParserLang.Check(tokens).Compile);
            List<string> StackCode = SuperEasyParserLang.Compile(tokens);
            StackCode.WriteAll();
            CollectionAssert.AreEqual(new string[] { "a", "2", "=" }, StackCode);
            EasyStackLang.Execute(StackCode);
            Assert.AreEqual(2, EasyStackLang.Variables["a"], double.Epsilon);
        }

        public static StreamReader StringToStream(string resource)
        {
            return new StreamReader(
               new MemoryStream(
                   Encoding.UTF8.GetBytes(resource)
               ));
        }
    }

    internal class MyEasyStackLang : AbstractStackExecuteLang
    {        
        protected override void ExecuteCommand(string command)
        {
            switch (command)
            {
                case "print":
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var pair in Variables)
                        {
                            sb.Append(pair.Key);
                            sb.Append(" = ");
                            sb.Append(pair.Value);
                            sb.AppendLine();
                        }
                        Console.Write(sb.ToString());
                    }
                    break;
                case "goto":
                    {
                        InstructionPointer = (int)PopStk() - 1;
                    }
                    break;
                case "if":
                    {
                        int addr = (int)PopStk();
                        int logical = (int)PopStk();
                        if (logical != 0) // В нашем языке всё, что не 0 - true.
                            InstructionPointer = addr - 1;
                    }
                    break;
                case "=":
                    {
                        double stmt = PopStk();
                        string var = Stack.Pop();
                        Variables[var] = stmt;
                    }
                    break;
                case "+":
                    {
                        Stack.Push(
                            (PopStk() + PopStk())
                            .ToString());
                    }
                    break;
                case "-":
                    {
                        Stack.Push(
                            (PopStk() - PopStk())
                            .ToString());
                    }
                    break;
                case "*":
                    {
                        Stack.Push(
                            (PopStk() * PopStk())
                            .ToString());
                    }
                    break;
                case "/":
                    {
                        Stack.Push(
                            (PopStk() / PopStk())
                            .ToString());
                    }
                    break;
                default:
                    {
                        if (!Variables.ContainsKey(command) && !double.TryParse(command, out _))
                            Variables[command] = 0;
                        Stack.Push(command);
                    }
                    break;
            }
        }

        /// <summary>
        /// Получает с стека значение и вызывает <see cref="GetValueOfVarOrDigit(string)"/>.
        /// </summary>
        private double PopStk() => GetValueOfVarOrDigit(Stack.Pop());

        private double GetValueOfVarOrDigit(string VarOrDigit)
        {
            if (double.TryParse(VarOrDigit, out double result))
                return result;
            return Variables[VarOrDigit];
        }
    }

    public static class Writer
    {
        public static void WriteAll<T>(this IEnumerable<T> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var e in list)
                sb.AppendLine(e.ToString());
            if (sb.Length == 0)
                Console.WriteLine("length = 0");
            else
                Console.Write(sb.ToString());
        }
    }
}