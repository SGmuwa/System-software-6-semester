using System;
using Lexer;
using MyTypes.Tree;
using Parser;

namespace Optimizing
{
    internal static class StaticTools
    {
        public static ulong NextULong(this Random ran)
        {
            byte[] output = new byte[sizeof(ulong)];
            ran.NextBytes(output);
            return BitConverter.ToUInt64(output);
        }

        internal static ITreeNode<object> CloneCompileTree(this ITreeNode<object> CompileTree)
        {
            return CompileTree.DeepClone(obj =>
            {
                return obj switch
                {
                    Token t => t,//new Token(t.Type, t.Value);
                    ParserToken rpc => rpc,//new ReportParserCompile(rpc.Source, rpc.CurrentRule, rpc.Helper);
                    _ => throw new OptimizingException($"В дереве компиляции встретился неизвестный тип: {obj.GetType()}"),
                };
            });
        }
    }
}