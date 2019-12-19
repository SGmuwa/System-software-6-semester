using MyTypes.Tree;
using System;

namespace Parser
{
    public class ReportParser
    {
        public ReportParser(ParserToken Compile = null)
        {
            this.Compile = new TreeNode<object>(Compile);
        }

        public ReportParser(ITreeNode<object> Compile)
        {
            this.Compile = Compile;
        }

        /// <summary>
        /// Возвращает true, если ошибки не найдены.
        /// </summary>
        public bool IsSuccess =>
            Info.IsSuccess;

        /// <summary>
        /// Представляет информацию о компиляции.
        /// </summary>
        public ITreeNode<object> Compile { get; protected set; }

        /// <summary>
        /// Представляет информацию о ходе поиска ошибок в коде.
        /// </summary>
        public ReportParserInfo Info { get; } = new ReportParserInfo();

        public void Merge(ReportParser reportParser)
        {
            if (Compile != null)
            {
                if (reportParser.Compile != null)
                {
                    if (Compile.Current == null)
                        Compile = reportParser.Compile;
                    else if (reportParser.Compile.Current != null)
                        Compile.Add(reportParser.Compile);
                    else
                        Info.AddInfo($"Информацию компиляции пропущена при добавлении к информации: {Compile}");
                }
                else
                    Info.AddInfo($"Не удалось добавить информацию компиляции к информации: {Compile}");
            }
            else
                Compile = reportParser.Compile;
            Info.AddRange(reportParser.Info);
            if(IsSuccess)
                CheckTreeOf_RuleOr_Error(Compile);
        }

        private void CheckTreeOf_RuleOr_Error(ITreeNode<object> compile)
        {
            if (compile != null)
                foreach (ITreeNode<object> o in compile)
                {
                    if (o.Current is ParserToken nonterminal)
                    {
                        if (nonterminal.CurrentRule == RuleOperator.OR
                            && o.Count > 1)
                            throw new InvalidOperationException($"Можно добавить только 1 OR. Подробнее: {o}");
                    }
                    if (o.Current == null)
                        throw new NullReferenceException($"В дереве найден элемент null: {o}");
                }
        }

        /// <summary>
        /// Удаляет или отменяет все заметки о компиляции в данном отчёте.
        /// </summary>
        public void CompileCancel()
        {
            Compile = null;
        }

        public override string ToString()
            => $"{nameof(ReportParser)}: {{Compile: {Compile}, Info: {Info}, IsSuccess: {IsSuccess}}}";
    }
}
