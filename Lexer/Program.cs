using System;
using System.Collections.Generic;
using System.IO;

namespace Lexer
{
    class Program
    {
        static int Main(string[] args)
        {
            StreamReader stream = OpenFileVisualInterface(args);
            List<Token> tokens;
            try
            {
                // Получаем жетоны.
                tokens = ExampleLang.Lang.SearchTokens(stream);
            }
            catch(LexerException e)
            {
                // Ошибка.
                Console.WriteLine(e + "\n" + e.StackTrace);
                return 5;
            }
            finally
            {
                stream.Close();
            }
            foreach (Token token in tokens)
                // Печатаем жетоны.
                Console.WriteLine(token);
            Console.Write("Press any key...");
            Console.ReadKey();
            return 0;
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
