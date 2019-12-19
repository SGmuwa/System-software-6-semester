using Lexer;
using Parser;
using System;
using System.Collections.Generic;
using System.IO;

namespace StackMachine
{
    public class Program
    {
        static int Main(string[] args)
        {
            (IList<Token>, ReportParser) tokensReport = GetParserReportFromUser(args);
            CompileAndExecute(tokensReport);
            return 0;
        }

        public static (IList<Token>, ReportParser) GetParserReportFromUser(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            StreamReader stream = OpenFileVisualInterface(args);
            List<Token> tokens;
            try
            {
                // Получаем жетоны.
                tokens = Lexer.ExampleLang.Lang.SearchTokens(stream);
            }
            catch (LexerException e)
            {
                // Ошибка.
                Console.WriteLine(e + "\n" + e.StackTrace);
                Console.ReadKey();
                throw;
            }
            stream.Close();
            tokens.RemoveAll((Token t) => t.Type.Name.Contains("CH_"));
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (Token token in tokens)
                // Печатаем жетоны.
                Console.WriteLine(token);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-----\nОтчёт компиляции:\n-----");
            var report = Parser.ExampleLang.Lang.Check(tokens);
            Console.WriteLine(string.Join("\n", report.Info));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-----\nДерево компиляции:\n-----");
            Console.WriteLine(report.Compile);
            if (!report.IsSuccess)
            {
                Console.WriteLine("Не могу запустить...");
                Console.ReadKey();
                throw new Exception("Код не может быть запущен.");
            }
            return (tokens, report);
        }

        public static void CompileAndExecute((IList<Token>, ReportParser) info)
        {
            List<string> commands = Parser.ExampleLang.Lang.Compile(info.Item1, info.Item2);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("-----\nПольская запись:\n-----");
            Console.WriteLine(string.Join(", ", commands));
            Console.ForegroundColor = ConsoleColor.White;
            new StackMachine.ExampleLang.MyMachineLang().Execute(commands);
            Console.Write("Press eny key...");
            Console.ReadLine();
        }

        /// <summary>
        /// Пытается открыть файл из аргумента командной строки.
        /// Если не получится, спрашивает у входящего потока.
        /// </summary>
        /// <param name="args">Аргументы командной строки.</param>
        /// <returns>Открытый файл для чтения.</returns>
        static StreamReader OpenFileVisualInterface(string[] args)
        {
            FileInfo input;
            if (args.Length != 1)
            { // Если файл из аргументов программы не взят
                Console.Write("Name file: ");
                input = new FileInfo(Console.ReadLine());
            }
            else
            { // Если есть аргументы, то берём из аргументов.
                input = new FileInfo(args[0]);
            }
            return input.OpenText();
        }
    }
}
