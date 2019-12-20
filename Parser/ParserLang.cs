using System;
using System.Collections.Generic;
using Lexer;
using static Parser.RuleOperator;
using MyTypes.Tree;
using System.Linq;


namespace Parser
{
    /// <summary>
    /// Представляет собой класс, который реализует парсер.
    /// </summary>
    public class ParserLang
    {
        private readonly Nonterminal mainNonterminal;

        public ParserLang(Nonterminal mainNonterminal)
            => this.mainNonterminal = mainNonterminal ?? throw new ArgumentNullException(nameof(mainNonterminal));

        /// <summary>
        /// Проверяет, соответствует ли заданный язык программирования грамматике.
        /// </summary>
        /// <param name="tokens">Список терминалов входного файла.</param>
        /// <returns>Отчёт об ошибках.</returns>
        public ReportParser Check(IList<Token> tokens)
        {
            int begin = 0, end = tokens.Count - 1;
            ReportParser output = mainNonterminal.CheckRule(tokens, ref begin, ref end);
            if (output.IsSuccess && begin <= end)
                output.Info.Add(new ReportParserInfoLine($"Входной текст не полностью подходит к грамматике.", null, tokens, tokens, begin));
            return output;
        }

        public List<string> Compile(IList<Token> tokens, ReportParser report = null)
        {
            List<string> commands = new List<string>(tokens.Count());
            if (report == null)
                report = Check(tokens);
            if (!report.IsSuccess)
                throw new ArgumentException("Свойство report.IsSuccess вернуло false. Возможно, входной файл написан с ошибками. Он не может быть скомпилирован.");
            ITreeNode<object> compileTree = report.Compile;
            ParserToken currentComp = (ParserToken)compileTree.Current;
            currentComp.Source.TransferToStackCode(commands, (int i) => Inserter(i, compileTree, commands), currentComp.Helper);
            return commands;
        }

        /// <summary>
        /// Занимается вставкой в стековый код.
        /// </summary>
        /// <param name="i">Какой объект из <see cref="Nonterminal.list"/> просит вставить.</param>
        /// <param name="compileTree">Информация о компиляции, которая запрашивает вставку.</param>
        /// <param name="commands">Список команд, куда надо вставить команду.</param>
        /// <returns></returns>
        private bool Inserter(int i, ITreeNode<object> compileTree, List<string> commands)
        {
            ParserToken comp = (ParserToken)compileTree.Current;
            if (i >= 0)
            { // AND, MORE
                if (comp.CurrentRule == OR)
                    throw new NotSupportedException($"Возможно, неправильно настроены правила компиляции в нетерминале: {comp.Source}");
                if (i >= compileTree.Count)
                    throw new IndexOutOfRangeException($"Был запрошен индекс вне границ. Индекс: {i}, коллекция: {compileTree}");
                if (compileTree[i].Current is Token token)
                { // Это терминал.
                    commands.Add(token.Value);
                }
                else if (compileTree[i].Current is ParserToken deep)
                { // Это нетерминал.
                    deep.Source.TransferToStackCode(commands, (int j) => Inserter(j, compileTree[i], commands), deep.Helper);
                }
                else
                    throw new ArgumentException($"Не получилось определить, терминал ли это, или нетерминал в списке {compileTree} с id {i}: {compileTree[i].GetType()}");
            }
            else // if(i < 0)
            { // OR
                if (comp.CurrentRule != OR)
                    throw new NotSupportedException($"Возможно, неправильно настроены правила компиляции в нетерминале: {comp.Source}");
                if (compileTree.Count != 1)
                    throw new ArgumentException($"Ошибка при настройке компиляции. Найдено: {compileTree.Count} Ожидался только один правильный элемент в нетерминале: {compileTree}\n");
                if (compileTree[0].Current is Token token)
                { // Это терминал.
                    commands.Add(token.Value);
                }
                else if (compileTree[0].Current is ParserToken deep)
                { // Это нетерминал.
                    deep.Source.TransferToStackCode(commands, (j) => Inserter(j, compileTree[0], commands), deep.Helper);
                }
                else
                    throw new ArgumentException($"Не получилось определить, терминал ли это, или нетерминал в списке {compileTree} с id {0}: {compileTree[0].GetType()}");
            }
            return true;
        }

        public override string ToString()
            => $"{base.ToString()}: main nonterminal: {mainNonterminal}";
    }
}
