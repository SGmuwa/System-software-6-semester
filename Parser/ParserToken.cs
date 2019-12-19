using System;

namespace Parser
{
    public class ParserToken
    {
        /// <summary>
        /// Кто добавил?
        /// </summary>
        public Nonterminal Source;
        /// <summary>
        /// Конкретное правило, которое использует нетерминал.
        /// Несмотря на то, что у нетерминала присваивается одно правило,
        /// использовать он может несколько.
        /// Так, при использовании <see cref="RuleOperator.ONE_AND_MORE"/> также используются правила:
        /// <see cref="RuleOperator.ZERO_AND_MORE"/> и <see cref="RuleOperator.AND"/>.
        /// </summary>
        public readonly RuleOperator CurrentRule;
        /// <summary>
        /// Для AND не нужен.
        /// Для OR - идентификатор следующего шага.
        /// Для MORE - количество повторов.
        /// </summary>
        public int Helper;

        /// <summary>
        /// Случайное число.
        /// </summary>
        public ulong Id { get; }

        private readonly static Random ran = new Random();

        /// <summary>
        /// Создание нового экземпляра инструкции компилятору.
        /// </summary>
        /// <param name="Source">Источник, кто добавил в компилятор запись.</param>
        /// <param name="Helper">Дополнительная информация о результатах парсера.</param>
        public ParserToken(Nonterminal Source, RuleOperator CurrentRule, int Helper = int.MinValue, ulong Id = ulong.MaxValue)
        {
            this.Source = Source;
            this.CurrentRule = CurrentRule;
            this.Helper = Helper;
            if(Id != ulong.MaxValue)
                this.Id = Id;
            else
                this.Id = ran.NextULong();
        }

        public override string ToString()
            => $"{nameof(ParserToken)}: {{ Id: RPC{Id}, {(Helper == int.MinValue ? "" : $"h: {Helper}, ")}rule: {CurrentRule}, src: {Source}}}";
    }
}
