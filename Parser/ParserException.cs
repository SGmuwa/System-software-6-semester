﻿using System;

namespace Parser
{
    public class ParserException : Exception
    {
        public ParserException(string messsage, int IndexTerminalError = -1)
            : base(messsage + $"IndexTerminalError = {IndexTerminalError}") => this.IndexTerminalError = IndexTerminalError;
        public ParserException()
            : base() { }

        public int IndexTerminalError { get; }
        //public List<Terminal> Expected = ;
    }
}