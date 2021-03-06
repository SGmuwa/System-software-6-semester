﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Lexer
{
    public class LexerLang
    {
        /// <summary>
        /// Создание экземпляра обработчика.
        /// </summary>
        /// <param name="availableTerminals">Набор разрешённых терминалов.</param>
        public LexerLang(IEnumerable<Terminal> availableTerminals)
        {
            this.availableTerminals = new List<Terminal>(availableTerminals ?? throw new ArgumentNullException());
        }

        /// <summary>
        /// Список поддерживаемых терминалов.
        /// </summary>
        private readonly List<Terminal> availableTerminals;

        /// <summary>
        /// Переобразование входного текста в лист жетонов на основе
        /// правил терминалов.
        /// Не забудьте после удалить все комментарии и CH_ жетоны, если используете <see cref="ExampleLang.Lang"/>!
        /// </summary>
        /// <param name="input">Входной поток текста.</param>
        /// <returns>Список найденных жетонов.</returns>
        public virtual List<Token> SearchTokens(StreamReader input)
        {
            if (input == null)
                throw new ArgumentNullException("BufferedStream input = null");
            List<Token> output = new List<Token>(); // Сюда запишем вывод.
            StringBuilder bufferList = new StringBuilder(); // Строка из файла.
            char[] buffer = new char[1]; // Сюда попадает символ перед тем, как попасть в строку.
            List<Terminal> termsFound; // Сюда помещаются подходящие терминалы к строке bufferList.

            // True, если последняя итерация была с добавлением элемента в output. Иначе - False.
            bool lastAdd = false;
            while (!input.EndOfStream || bufferList.Length != 0)
            {
                if (!lastAdd && !input.EndOfStream)
                {
                    input.Read(buffer, 0, 1); // Чтение символа.
                    bufferList.Append(buffer[0]); // Запись символа в строку.
                }
                lastAdd = false;
                // Получение списка подходящих терминалов:
                termsFound = SearchInTerminals(bufferList.ToString());

                // Ура, мы что-то, кажется, нашли.
                if (termsFound.Count <= 1 || input.EndOfStream)
                {
                    if (termsFound.Count == 1 && !input.EndOfStream)
                        // Это ещё не конец файла и есть 1 прецедент. Ищем дальше.
                        continue;
                    int last = char.MaxValue + 1;
                    if (termsFound.Count == 0)
                    {
                        last = bufferList[bufferList.Length - 1]; // Запоминаем последний символ.
                        bufferList.Length--; // Уменьшаем длину списка на 1.
                        termsFound = SearchInTerminals(bufferList.ToString()); // Теперь ищем терминалы.
                    }
                    if (termsFound.Count != 1) // Ой, должен был остаться только один.
                    {
                        if (termsFound.Count == 0)
                            throw new LexerException
                            ($"Количество подходящих терменалов не равно 1: {termsFound.Count}. Последние удачные: {string.Join(", ", output)}");
                        Terminal need = termsFound.First();
                        Terminal oldNeed = null;
                        bool unical = true; // True, если необходимый терминал имеет самый высокий приоритет.
                        for (int i = 1; i < termsFound.Count; i++)
                        {
                            if (termsFound[i] > need)
                            {
                                need = termsFound[i];
                                unical = true;
                            }
                            else if (Terminal.PriorityEquals(termsFound[i], need))
                            {
                                oldNeed = termsFound[i];
                                unical = false;
                            }
                        }
                        if (!unical)
                            throw new LexerException
                                ($"Количество подходящих терменалов не равно 1: {termsFound.Count}" +
                                $", возможно был конфликт между: {oldNeed} и {need}");
                        termsFound.Clear();
                        termsFound.Add(need);
                    }
                    // Всё идёт как надо
                    // Добавим в результаты
                    output.Add(
                        new Token(
                        termsFound.First(),
                        bufferList.ToString(),
                        (ulong)output.Count
                        ));
                    bufferList.Clear();
                    lastAdd = true;
                    if (last != char.MaxValue + 1)
                        bufferList.Append((char)last);
                }
            }
            return output;
        }

        private List<Terminal> SearchInTerminals(string expression)
        {
            List<Terminal> output = new List<Terminal>();
            foreach (Terminal ter in availableTerminals)
            {
                Match mat = ter.RegularExpression.Match(expression);
                if (mat.Length > 0 && mat.Value.Equals(expression))
                    output.Add(ter);
            }
            return output;
        }
    }
}
