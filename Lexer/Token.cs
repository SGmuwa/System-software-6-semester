namespace Lexer
{
    /// <summary>
    /// Класс, представляющий жетон.
    /// </summary>
    public class Token : Token<string>
    {
        public Token(Terminal Type, string Value, ulong Id) : base(Type, Value, Id) { }
    }

    public class Token<T>
    {
        /// <summary>
        /// Создание экземпляра жетона.
        /// </summary>
        /// <param name="Type">Транслятор, с помощью которого
        /// был найден жетон.</param>
        /// <param name="Value">Подстрока, которая была найдена
        /// транслятором.</param>
        /// <param name="Id">Желаемый идентификатор жетона.
        /// Влияет только на отображение ToString.</param>
        public Token(Terminal Type, T Value, ulong Id)
        {
            this.Type = Type;
            this.Value = Value;
            this.Id = Id;
        }

        /// <summary>
        /// Тип жетона.
        /// </summary>
        public Terminal Type { get; }

        /// <summary>
        /// Значение жетона.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Порядковый номер жетона.
        /// </summary>
        public ulong Id { get; }

        public bool Equals(object obj, bool idNeedEqual = true)
        {
            return obj is Token<T> t
                && (this.Id.Equals(t.Id) || !idNeedEqual)
                && this.Type.Equals(t.Type)
                && this.Value.Equals(t.Value);
        }

        public override bool Equals(object obj)
            => Equals(obj);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(Token<T>)}[TK{Id}] {Type}: \"{Value}\"";
        }
        
    }
}