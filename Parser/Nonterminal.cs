using Lexer;
using System;
using System.Collections;
using System.Collections.Generic;
using static Parser.RuleOperator;
using System.Text;
using MyTypes.Tree;

namespace Parser
{

    /// <summary>
    /// Делегат хранит функцию, которая вставляет
    /// значение терминала или вызывает
    /// формирование стек-кода у нетерминала.
    /// </summary>
    /// <param name="IndexToAdd">-1 используется для OR, так
    /// как он там единственный. Для остальных - порядковый номер
    /// начиная с 0 нужного нетерминала или терминала.</param>
    /// <returns>Возвращает true, если операция прошла успешна.
    /// Иначе - false.</returns>
    public delegate bool ActionInsert(int IndexToAdd = -1);

    /// <summary>
    /// Делегат, который представляет собой функцию, которая
    /// служит для преобразования жетонов в стек-код.
    /// </summary>
    /// <param name="commands">Сюда вставляются команды
    /// для стековой машины.</param>
    /// <param name="insert">Функция вызывает вставку либо значение жетона,
    /// либо вызывает функцию вставки кода входящего нетерминала.</param>
    /// <param name="helper">Используется для OR. Символизирует номер,
    /// начиная с 0, какой терминал или нетерминал был выбран.</param>
    public delegate void TransferToStackCode(List<string> commands, ActionInsert insert, int helper = 0);

    /// <summary>
    /// Хранит в себе одно правило грамматики.
    /// </summary>
    public class Nonterminal : IList, IList<object>
    {
        /// <summary>
        /// Лист содержит операторы нетерминалов или терминалы, образуя единый терминал.
        /// Тут должны быть типы:
        /// 1. <see cref="Terminal"/>;
        /// 2. <see cref="Nonterminal"/>.
        /// </summary>
        private readonly List<object> list = new List<object>();

        /// <summary>
        /// Указывает, какая реакция должна быть на истинность всех терминалов и нетерминалов.
        /// </summary>
        private readonly RuleOperator rule;

        /// <summary>
        /// Имя нетерминала.
        /// </summary>
        public string Name { get; } = null;

        /// <summary>
        /// Функция преобразования жетонов в стек-код.
        /// </summary>
        public readonly TransferToStackCode TransferToStackCode = null;

        /// <summary>
        /// Последний свободный идентификатор.
        /// </summary>
        private static ulong FreeId = 0;

        private readonly object sync = new object();

        /// <summary>
        /// Создание экземпляра нетерминала.
        /// </summary>
        /// <param name="Name">Устанавливает имя терминала.</param>
        /// <param name="rule">Указывает, какая реакция должна быть на истинность всех терминалов и нетерминалов.</param>
        /// <param name="terminalsOrNonterminals">Список терминалов и нетерминалов.</param>
        public Nonterminal(string Name, TransferToStackCode TransferToStackCode, RuleOperator rule, params object[] terminalsOrNonterminals)
            : this(Name, rule, terminalsOrNonterminals)
        {
            this.TransferToStackCode = TransferToStackCode;
        }

        /// <summary>
        /// Создание экземпляра нетерминала.
        /// </summary>
        /// <param name="Name">Устанавливает имя терминала.</param>
        /// <param name="rule">Указывает, какая реакция должна быть на истинность всех терминалов и нетерминалов.</param>
        /// <param name="terminalsOrNonterminals">Список терминалов и нетерминалов.</param>
        public Nonterminal(string Name, RuleOperator rule, params object[] terminalsOrNonterminals)
            : this(rule, terminalsOrNonterminals)
        {
            this.Name = Name;
        }

        /// <summary>
        /// Создание экземпляра нетерминала.
        /// </summary>
        /// <param name="rule">Указывает, какая реакция должна быть на истинность всех терминалов и нетерминалов.</param>
        /// <param name="terminalsOrNonterminals">Список терминалов и нетерминалов.</param>
        public Nonterminal(RuleOperator rule, params object[] terminalsOrNonterminals)
        {
            this.rule = rule;
            AddRange(terminalsOrNonterminals ?? throw new ArgumentNullException("Невероятная ошибка понимания синтаксиса C# достигнута."));
        }

        /// <summary>
        /// Проверяет, чтобы заданные жетоны соответствовали нетерминалу.
        /// </summary>
        /// <param name="tokens">Список жетонов, которые надо проверить.</param>
        /// <returns>Отчёт анализа кода.</returns>
        public ReportParser CheckRule(List<Token> tokens)
        {
            int a = 0, b = tokens.Count - 1;
            return CheckRule(tokens, ref a, ref b);
        }

        /// <summary>
        /// Проверяет, чтобы заданные жетоны соответствовали нетерминалу.
        /// </summary>
        /// <param name="tokens">Список жетонов, которые надо проверить.</param>
        /// <param name="begin">Первый доступный индекс в листе tokens.</param>
        /// <param name="end">Последний доступный индекс в листе tokens.</param>
        /// <param name="deep">Ограничение по поиску в глубину.</param>
        /// <returns>Отчёт анализа кода.</returns>
        public ReportParser CheckRule(IList<Token> tokens, ref int begin, ref int end, int deep = 500)
        {
            lock(sync)
            {
                try
                {
                    var output = PCheckRule(deep, tokens, ref begin, ref end);
                    return output;
                }
                finally
                {
                    FreeId = 0;
                }
            }
        }

        /// <summary>
        /// Проверяет, чтобы заданные жетоны соответствовали нетерминалу.
        /// </summary>
        /// <param name="tokens">Список жетонов, которые надо проверить.</param>
        /// <param name="begin">Первый доступный индекс в листе tokens.</param>
        /// <param name="end">Последний доступный индекс в листе tokens.</param>
        /// <returns>Отчёт анализа кода.</returns>
        private ReportParser PCheckRule(int deep, IList<Token> tokens, ref int begin, ref int end)
        {
            if (tokens == null)
                throw new ArgumentNullException("Список жетонов должен был инициализирован.");
            ReportParser output = new ReportParser();
            output.Info.AddInfo("Зашёл в нетерминал: " + ToString());
            if(--deep <= 0)
            {
                output.Info.Add(new ReportParserInfoLine("Обнаружена бесконечная рекурсия. Выход..."));
            }
            else switch (rule)
            {
                case AND:
                    output.Merge(RuleAND(deep, tokens, ref begin, ref end));
                    break;
                case ONE_AND_MORE:
                    output.Merge(RuleONE_AND_MORE(deep, tokens, ref begin, ref end));
                    break;
                case OR:
                    output.Merge(RuleOR(deep, tokens, ref begin, ref end));
                    break;
                case ZERO_AND_MORE:
                    output.Merge(RuleZERO_AND_MORE(deep, tokens, ref begin, ref end));
                    break;
                case NONE:
                    output.Info.Add(new ReportParserInfoLine("Игнорирую."));
                    break;
                default:
                    throw new NotImplementedException($"Оператор {Enum.GetName(typeof(RuleOperator), rule)} не реализован.");
            }
            output.Info.AddInfo("Состояние отчёта: " + output.IsSuccess + ", выхожу из нетерминала: " + ToString());
            return output;
        }

        private ReportParser RuleZERO_AND_MORE(int deep, IList<Token> tokens, ref int begin, ref int end)
        {
            ReportParserCompile compile = new ReportParserCompile(this, ZERO_AND_MORE, -1, FreeId++);
            ReportParser output = new ReportParser(compile);
            do
            {
                RuleMORE(deep, tokens, output, ref begin, ref end);
                compile.Helper++;
            }
            while (output.IsSuccess) ;
            output.Info.Success("Нетерминалы ZERO_AND_MORE всегда успешны. Текущий: " + ToString());
            return output;
        }

        private ReportParser RuleONE_AND_MORE(int deep, IList<Token> tokens, ref int begin, ref int end)
        {
            ReportParserCompile compile = new ReportParserCompile(this, ONE_AND_MORE, -1, FreeId++);
            ReportParser output = new ReportParser(compile);
            compile.Helper++;
            if (!RuleMORE(deep, tokens, output, ref begin, ref end))
            {
                output.CompileCancel();
                return output;
            }
            compile.Helper++;
            do
            {
                RuleMORE(deep, tokens, output, ref begin, ref end);
                compile.Helper++;
            }
            while (output.IsSuccess);
            output.Info.Success("Нетерминалы ZERO_AND_MORE всегда успешны. Текущий: " + ToString());
            return output;
        }

        private bool RuleMORE(int deep, IList<Token> tokens, ReportParser output, ref int begin, ref int end)
        {
            int b = begin;
            int e = end;
            if (Count != 1)
                throw new NotSupportedException($"Правило {rule} поддерживает только один элемент в нетерминале.");
            foreach (object o in this)
            {
                if (o is Terminal)
                {
                    if (begin > end)
                        output.Info.Add(new ReportParserInfoLine(
                            "Входные жетоны закончились", o, null, begin));
                    else if (!o.Equals(tokens[begin++].Type))
                        output.Info.Add(new ReportParserInfoLine(o, tokens[--begin], tokens, begin));
                    else
                        output.Compile.Add(tokens[begin - 1]);
                }
                else if (o is Nonterminal nonterminal)
                {
                    output.Merge(nonterminal.PCheckRule(deep, tokens, ref begin, ref end));
                    if (!output.IsSuccess)
                    {
                        output.Info.Add(new ReportParserInfoLine(o, null, tokens, begin));
                    }
                }
                if (!output.IsSuccess)
                {
                    begin = b;
                    end = e;
                    return output.IsSuccess;
                }
            }
            if (!output.IsSuccess)
            {
                begin = b;
                end = e;
            }
            return output.IsSuccess;
        }

        private ReportParser RuleAND(int deep, IList<Token> tokens, ref int begin, ref int end)
        {
            ITreeNode<object> compileTree = new TreeNode<object>(new ReportParserCompile(this, AND, Id: FreeId++));
            ReportParser output = new ReportParser(compileTree);
            int b = begin;
            int e = end;
            int i = -1;
            foreach(object o in this)
            {
                i++;
                if(o is Terminal)
                {
                    if (begin > end)
                        output.Info.Add(new ReportParserInfoLine(
                            "Входные жетоны закончились", o, null, begin));
                    else if (!o.Equals(tokens[begin++].Type))
                        output.Info.Add(new ReportParserInfoLine(o, tokens[--begin], tokens, begin));
                    else
                    {
                        compileTree.Add(tokens[begin - 1]);
                        output.Info.Success(tokens[begin - 1].ToString());
                    }
                }
                else if(o is Nonterminal nonterminal)
                {
                    output.Merge(nonterminal.PCheckRule(deep, tokens, ref begin, ref end));
                    if(!output.IsSuccess)
                    {
                        output.Info.Add(new ReportParserInfoLine(o, null, tokens, begin));
                    }
                }
                if (!output.IsSuccess)
                {
                    begin = b;
                    end = e;
                    output.CompileCancel();
                    return output;
                }
            }
            if(!output.IsSuccess)
            {
                begin = b;
                end = e;
                output.CompileCancel();
            }
            return output;
        }

        private ReportParser RuleOR(int deep, IList<Token> tokens, ref int begin, ref int end)
        {
            ReportParserCompile comp = new ReportParserCompile(this, OR, -1, FreeId++);
            ITreeNode<object> compileTree = new TreeNode<object>(comp);
            ReportParser output = new ReportParser(compileTree);
            foreach (object o in this)
            {
                comp.Helper++;
                if (o is Terminal)
                {
                    if (begin > end)
                        output.Info.Add(new ReportParserInfoLine(
                            "Входные жетоны закончились", o, null, begin));
                    else if (!o.Equals(tokens[begin++].Type))
                        output.Info.Add(new ReportParserInfoLine(o, tokens[--begin], tokens, begin));
                    else
                    {
                        compileTree.Add(tokens[begin - 1]);
                        output.Info.Success(tokens[begin - 1].ToString());
                        return output;
                    }
                }
                else if (o is Nonterminal nonterminal)
                {
                    ReportParser buffer = nonterminal.PCheckRule(deep, tokens, ref begin, ref end);
                    if (buffer.IsSuccess)
                    {
                        output.Merge(buffer);
                        output.Info.Success(o.ToString());
                        return output; // Да, этот нетерминал нам подходит.
                    }
                    else
                    {
                        buffer.CompileCancel();
                        output.Merge(buffer);
                    }
                }
                else
                    throw new Exception($"Unexpected type {o.GetType()} of {o} in list");
            }
            if(begin < tokens.Count)
                output.Info.Add(new ReportParserInfoLine("Для оператора OR не найдено ни одного истинного выражения.", this, tokens[begin], tokens, -1));
            output.CompileCancel();
            return output;
        }

        /// <summary>
        /// Проверяет, что элемент подходит для данного списка.
        /// Подходят объекты только Token и RuleOperator.
        /// </summary>
        /// <param name="value">Объект на проверку.</param>
        /// <returns>Обработанный объект.
        /// Например, преобразование <see cref="string"/> в <see cref="Terminal"/>.</returns>
        private object CheckSet(object value)
        {
            if (!IsCanAdd(value))
                throw new ArgumentException(
                    "Ожидался жетон или оператор. Фактически: " + value.ToString());
            return value is string @string ? new Terminal(@string)
                : value;
        }

        /// <summary>
        /// Определяет, может ли элемент быть добавлен в данное правило.
        /// </summary>
        /// <param name="value">Объект, рассмотреть который необходимо.</param>
        /// <returns>True, если это одно из:
        /// 1. <see cref="RuleOperator"/>;
        /// 2. <see cref="Terminal"/>;
        /// 3. <see cref="Nonterminal"/>;
        /// 4. Представление названия <see cref="Terminal"/> в виде <see cref="string"/>.
        /// В противном случае, False. Если value = null, то возвращает False.</returns>
        public bool IsCanAdd(object value)
         => value != null && 
            (value is Terminal
            || value is string
            || value is RuleOperator
            || value is Nonterminal
            || (value is object[] v && IsCanAddRange(v))
            );

        /// <summary>
        /// Вызывает <see cref="IsCanAdd(object)"/> для каждого элемента
        /// множества array.
        /// </summary>
        /// <returns>True, если все соответствуют. <see cref="IsCanAdd(object)"/>. Иначе - false.</returns>
        public bool IsCanAddRange(IEnumerable array)
        {
            foreach (object o in array)
                if (!IsCanAdd(o))
                    return false;
            return true;
        }

        /// <summary>
        /// Добавляет множество операторов или терминалов в правило.
        /// </summary>
        /// <param name="operatorsWithTerminals">Перечень объектов,
        /// соответствующих <see cref="IsCanAdd(object)"/>.</param>
        public void AddRange(IEnumerable<object> operatorsWithTerminals)
        { // Для безопасности необходимо добавлять каждый элемент последовательно.
            foreach (object opOrTer in operatorsWithTerminals)
                this.Add(opOrTer);
        }

        public override string ToString()
            => ToString(); // Как это работает?

        public string ToString(uint depth = 1)
        {
            if (Name != null)
                return $"name: {Name}";
            StringBuilder sb = new StringBuilder();
            sb.Append("rule: " + rule);
            sb.Append(", elements: ");
            if (list != null)
            {
                foreach (object o in this)
                {
                    if (o != null)
                    {
                        if (o is Nonterminal nonterminal)
                            if (depth != 0)
                                sb.Append("{ " + nonterminal.ToString(depth - 1) + " }");
                            else
                                sb.Append("{ ... }");
                        else
                            sb.Append("{ " + o.ToString() + " } ");
                    }
                    else
                        sb.Append("{ element is null }");
                }
            }
            else
            {
                sb.Append("list is null.");
            }
            return sb.ToString();
        }

        #region IList

        public object this[int index]
        {
            get => list[index];
            set
            {
                list[index] = CheckSet(value);
            }
        }

        public int Count => list.Count;

        public bool IsReadOnly => ((IList)list).IsReadOnly;

        public bool IsFixedSize => ((IList)list).IsFixedSize;

        public object SyncRoot => ((IList)list).SyncRoot;

        public bool IsSynchronized => ((IList)list).IsSynchronized;

        /// <summary>
        /// Дополнить правило нетерминала.
        /// </summary>
        /// <param name="item">Объект, соответствующий <see cref="IsCanAdd(object)"/>.</param>
        public void Add(object item)
        {
            if ((rule == ZERO_AND_MORE || rule == ONE_AND_MORE)
                && Count > 0)
                throw new NotSupportedException($"В нетерминалах с правилом {nameof(ZERO_AND_MORE)} или {nameof(ONE_AND_MORE)} можно добавить только один элемент.");
            list.Add(CheckSet(item));
        }

        public void Clear() => list.Clear();

        public bool Contains(object item) => list.Contains(item);

        public void CopyTo(object[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        public IEnumerator<object> GetEnumerator() => list.GetEnumerator();

        public int IndexOf(object item)
        {
            if (IsCanAdd(item))
                // Гарантируется, что все объекты в списке соответствуют критерию IsCanAdd(object)
                return list.IndexOf(item);
            else
                return -1;
        }

        public void Insert(int index, object item)
            => list.Insert(index, CheckSet(item));

        public bool Remove(object item)
            => list.Remove(item);

        public void RemoveAt(int index)
            => list.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator()
            => list.GetEnumerator();

        int IList.Add(object value)
            => ((IList)list).Add(CheckSet(value));

        void IList.Remove(object value)
            => ((IList)list).Remove(value);

        public void CopyTo(Array array, int index)
            => ((IList)list).CopyTo(array, index);
        #endregion
    }
}
