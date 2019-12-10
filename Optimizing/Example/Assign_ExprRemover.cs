using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MyTypes.Tree;
using Parser;
using Lexer;
using System.Linq;
using System;

namespace Optimizing.Example
{
    /// <summary>
    /// Реализация оптимизации удаления лишних присваиваний.
    /// </summary>
    public class Assign_ExprRemover : IOptimizing
    {
        public static readonly Assign_ExprRemover Instance = new Assign_ExprRemover();

        private Assign_ExprRemover() {}

        public readonly Terminal EMPTY = new Terminal(nameof(EMPTY));

        public ReportParser Optimize(ReportParser compiledCode)
        {
            if(!compiledCode.IsSuccess)
                throw new OptimizingException("Входное дерево компиляции построено не верно!");
            if(compiledCode.Compile == null)
                throw new OptimizingException("Вызовите compiledCode.Compile() перед началом.");
            ITreeNode<object> treeCompileForCheckVars = compiledCode.Compile.CloneCompileTree();

            var assignOpTokens = from a in treeCompileForCheckVars
            where a.Current is Token token && token.Type == Lexer.ExampleLang.ASSIGN_OP
            select (Token)a.Current;

            foreach(Token t in assignOpTokens)
            {
                t.Value = t.Id + " =";
            }

            IList<string> commands = string.Join(' ', Parser.ExampleLang.Lang.Compile((from a in treeCompileForCheckVars where a.Current is Token t select (Token)a.Current).ToList(), new ReportParser(treeCompileForCheckVars))).Split(' ');
            HashSet<ulong> TokensToRemove = new OptimizingStackMachine().MyExecute(commands);

            foreach(Token t in assignOpTokens)
            {
                t.Value = "=";
            }

            ITreeNode<object> output = compiledCode.Compile.CloneCompileTree();

            var RPCToRemove = from a in output where a.Current is ReportParserCompile && a.Count == 3 && a[1].Current is Token token && TokensToRemove.Contains(token.Id)
            select a;
            foreach(var rpc in RPCToRemove)
            {
                rpc.Clear();
                ((ReportParserCompile)rpc.Current).CurrentRule = RuleOperator.NONE;
            }

            return new ReportParser(output);
        }

        class OptimizingStackMachine : StackMachine.ExampleLang.MyStackLang
        {
            readonly DictionaryHooker myDic;
            readonly HashSet<ulong> ToRemove = new HashSet<ulong>();
            public OptimizingStackMachine(IDictionary<string, double> startVariables = null) : base(null)
            {
                myDic = new DictionaryHooker(this, startVariables);
                base.Variables = myDic;
                base.commands["="] = _ =>
                {
                    ulong id = ulong.Parse(Stack.Pop());
                    double stmt = PopStk();
                    string var = Stack.Pop();
                    if (IsNumber(var))
                        throw new KeyNotFoundException();
                    Variables[var] = stmt;
                };
            }

            public HashSet<ulong> MyExecute(IList<string> code)
            {
                base.Execute(code);
                ToRemove.UnionWith(GetIdsOfLastSet(code));
                return ToRemove;
            }

            public IEnumerable<ulong> GetIdsOfLastSet(IList<string> code)
            {
                foreach(var var in myDic.IsNotUsedIndexes)
                {
                    yield return ulong.Parse(code[var.Item1]);
                }
            }

            class DictionaryHooker : IDictionary<string, double>
            {
                private readonly OptimizingStackMachine Machine;

                public DictionaryHooker(OptimizingStackMachine machine, IDictionary<string, double> source = null)
                {
                    Machine = machine;
                    if(source != null)
                        Source = source;
                    else
                        Source = new Dictionary<string, double>();
                }

                public IDictionary<string, double> Source;
                /// <summary>
                /// Переменные, которые использовались для чтения.
                /// </summary>
                public List<(int, string)> IsNotUsedIndexes = new List<(int, string)>();

                public double this[string key]
                {
                    get
                    {
                        RemoveFromIndex(key, Machine.InstructionPointer);
                        return Source[key];
                    }
                    set
                    {
                        IsNotUsedIndexes.Add((Machine.InstructionPointer - 1, key));
                        Source[key] = value;
                    }
                }

                private void RemoveFromIndex(string var, int ofLeft)
                {
                    int? indexMax = null;
                    for(int i = 0; i < IsNotUsedIndexes.Count; i++)
                    {
                        if(var == IsNotUsedIndexes[i].Item2 && IsNotUsedIndexes[i].Item1 < ofLeft)
                        {
                            if(!indexMax.HasValue || IsNotUsedIndexes[i].Item1 > IsNotUsedIndexes[indexMax.Value].Item1)
                                indexMax = i;
                        }
                    }
                    if(indexMax.HasValue)
                    {
                        IsNotUsedIndexes.RemoveAt(indexMax.Value);
                    }
                }

                public ICollection<string> Keys => Source.Keys;

                public ICollection<double> Values => Source.Values;

                public int Count => Source.Count;

                public bool IsReadOnly => Source.IsReadOnly;

                public void Add(string key, double value)
                {
                    Source.Add(key, value);
                }

                public void Add(KeyValuePair<string, double> item)
                {
                    Source.Add(item);
                }

                public void Clear()
                {
                    Source.Clear();
                }

                public bool Contains(KeyValuePair<string, double> item)
                {
                    return Source.Contains(item);
                }

                public bool ContainsKey(string key)
                {
                    return Source.ContainsKey(key);
                }

                public void CopyTo(KeyValuePair<string, double>[] array, int arrayIndex)
                {
                    Source.CopyTo(array, arrayIndex);
                }

                public IEnumerator<KeyValuePair<string, double>> GetEnumerator()
                {
                    foreach(var pair in Source)
                    {
                        RemoveFromIndex(pair.Key, Machine.InstructionPointer);
                        yield return pair;
                    }
                }

                public bool Remove(string key)
                {
                    return Source.Remove(key);
                }

                public bool Remove(KeyValuePair<string, double> item)
                {
                    return Source.Remove(item);
                }

                public bool TryGetValue(string key, [MaybeNullWhen(false)] out double value)
                {
                    return Source.TryGetValue(key, out value);
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return Source.GetEnumerator();
                }
            }
        }
    }
}