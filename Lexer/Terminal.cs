﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lexer
{
    /// <summary>
    /// Представление терминала.
    /// </summary>
    public class Terminal
    {
        /// <summary>
        /// Создание экземпляра терминала.
        /// </summary>
        /// <param name="Name">Имя терминала.</param>
        /// <param name="RegularExpression">Регулярное
        /// выражение терминала.</param>
        public Terminal(string Name, string RegularExpression, uint priority = uint.MaxValue / 2)
            : this(Name, new Regex(RegularExpression, RegexOptions.Multiline)) { }

        /// <summary>
        /// Создание экземпляра терминала.
        /// </summary>
        /// <param name="Name">Имя терминала.</param>
        /// <param name="RegularExpression">Регулярное
        /// выражение терминала.</param>
        public Terminal(string Name, Regex RegularExpression, uint priority = uint.MaxValue / 2)
        {
            this.Name = Name;
            this.RegularExpression = RegularExpression;
            this.priority = priority;
        }

        /// <summary>
        /// Имя терминала.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Приоритет терминала.
        /// </summary>
        private readonly uint priority;

        /// <summary>
        /// Регулярное выражение, которое соответсвует данному
        /// терминалу.
        /// </summary>
        public readonly Regex RegularExpression;

        /// <summary>
        /// Определяет, является ли текущий объект эквивалентным входому.
        /// Стоит отметить, что идёт сравнение только по именам, так как
        /// предполагается, что имена уникальны.
        /// </summary>
        /// <param name="translator">Объект, который сравнивается
        /// с текущим. Если отправить null, то функция вернёт
        /// <code>false</code>.</param>
        /// <returns><code>true</code>, если объекты эквивалентны.
        /// Иначе - <code>false</code>.</returns>
        public override bool Equals(object translator)
        {
            if (translator == null)
                return false;
            if (!(translator is Terminal))
                return false;
            return Name.Equals(translator);
        }

        public override int GetHashCode()
        {
            // 539060726 - visual studio сгенерировала.
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public override string ToString()
        {
            return $"{Name}: \"{RegularExpression.ToString()}\"";
        }

        /// <summary>
        /// Сравнение приоритетов левого и правого терминала.
        /// </summary>
        /// <param name="left">Левый терминал.</param>
        /// <param name="right">Правый терминал.</param>
        /// <returns>True, если у левого приоритет выше (ближе к 0), чем у right.
        /// Иначе - false.</returns>
        public static bool operator >(Terminal left, Terminal right)
         => left.priority < right.priority;

        /// <summary>
        /// Сравнение приоритетов левого и правого терминала.
        /// </summary>
        /// <param name="left">Левый терминал.</param>
        /// <param name="right">Правый терминал.</param>
        /// <returns>True, если у левого приоритет ниже (дальше к 0), чем у right.
        /// Иначе - false.</returns>
        public static bool operator <(Terminal left, Terminal right)
         => left.priority > right.priority;

        /// <summary>
        /// Сравнение приоритетов левого и правого терминала.
        /// </summary>
        /// <param name="left">Левый терминал.</param>
        /// <param name="right">Правый терминал.</param>
        /// <returns>True, если у левого приоритет такой же, как у right.
        /// Иначе - false.</returns>
        public static bool PriorityEquals(Terminal left, Terminal right)
         => left.priority == right.priority;

        /// <summary>
        /// Сравнение приоритетов левого и правого терминала.
        /// </summary>
        /// <param name="left">Левый терминал.</param>
        /// <param name="right">Правый терминал.</param>
        /// <returns>True, если у левого приоритет отличен от right.
        /// Иначе - false.</returns>
        public static bool PriorityNotEquals(Terminal left, Terminal right)
         => left.priority == right.priority;

    }
}