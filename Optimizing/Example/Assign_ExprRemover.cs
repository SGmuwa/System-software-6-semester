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
                t.Value = t.Id + " =";

            IList<string> commands = Parser.ExampleLang.Lang.Compile((from a in treeCompileForCheckVars where a.Current is Token t select (Token)a.Current).ToList(), new ReportParser(treeCompileForCheckVars));
            
            foreach(Token t in assignOpTokens)
                t.Value = "=";

            
            HashSet<ulong> TokensToRemove = new OptimizingStackMachine().MyExecute(commands);

            ITreeNode<object> output = compiledCode.Compile.CloneCompileTree();

            HashSet<ITreeNode<object>> RPCToRemove = new HashSet<ITreeNode<object>>(
                from a in output
                where a.Current is ParserToken
                    && a.Count == 3
                    && a[1].Current is Token token
                    && TokensToRemove.Contains(token.Id)
                select a);
            Console.WriteLine($"Remove need:\n{string.Join("\n", RPCToRemove)}");
            ParserToken none = new ParserToken(new Nonterminal("none", (a, b, c) => {}, RuleOperator.NONE), RuleOperator.NONE);
            output.ReplaceWhere(
                (a) => RPCToRemove.Contains(a),
                () => new TreeNode<object>(none));

            return new ReportParser(output);
        }

        /// <summary>
        /// Класс с реализацией стековой машины, которая создана для оптимизации кода.
        /// </summary>
        class OptimizingStackMachine : StackMachine.ExampleLang.MyMachineLang
        {
            readonly DictionaryHooker myDic;
            readonly HashSet<ulong> ToRemove = new HashSet<ulong>();
            /// <summary>
            /// Хранит пары InstructionPointer и ID Token assign_op.
            /// </summary>
            public readonly IDictionary<int, ulong> assignOpIds = new Dictionary<int, ulong>();
            /// <summary>
            /// Получает IP последней команды "="
            /// </summary>
            public int LastIPAssignOp { get; private set; }
            /// <summary>
            /// Получает IP последней переменной.
            /// </summary>
            public int LastIPVar { get; private set; }
            public OptimizingStackMachine(IDictionary<string, double> startVariables = null) : base(null)
            {
                myDic = new DictionaryHooker(this, startVariables);
                base.Variables = myDic;
            }

            public HashSet<ulong> MyExecute(IList<string> code)
            {
                base.Execute(code);
                ToRemove.UnionWith(GetIdsOfLastSet(code));
                return ToRemove;
            }

            protected override void ExecuteCommand(string command)
            {
                if(command.Contains(" ="))
                {
                    assignOpIds[InstructionPointer] = ulong.Parse(command.Substring(0, command.IndexOf(' ')));
                    LastIPAssignOp = InstructionPointer;
                    base.ExecuteCommand("=");
                    return;
                }
                if(command == "=")
                {
                    base.ExecuteCommand(command);
                    return;
                }
                if(!base.commands.ContainsKey(command))
                { // Это переменная.
                    LastIPVar = InstructionPointer;
                }
                base.ExecuteCommand(command);
            }

            public IEnumerable<ulong> GetIdsOfLastSet(IList<string> code)
            {
                foreach(var var in myDic.IsNotUsedIndexes)
                    yield return assignOpIds[var.Item1];
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
                        RemoveFromIndex(Machine.LastIPAssignOp, key);
                        return Source[key];
                    }
                    set
                    {
                        AddToIndex(Machine.LastIPAssignOp, key);
                        Source[key] = value;
                    }
                }

                private void RemoveFromIndex(int ofLeft, string var)
                {
                    int? indexMax = null;
                    for(int i = 0; i < IsNotUsedIndexes.Count; i++)
                    {
                        (int IPSet, string VarSet) = IsNotUsedIndexes[i];
                        if(var == VarSet && IPSet <= ofLeft)
                        {
                            if(!indexMax.HasValue)
                                indexMax = i;
                            else
                            {
                                var MaxTuple = IsNotUsedIndexes[indexMax.Value];
                                if(IPSet > MaxTuple.Item1)
                                    indexMax = i;
                            }
                        }
                    }
                    if(indexMax.HasValue)
                        IsNotUsedIndexes.RemoveAt(indexMax.Value);
                }

                private void AddToIndex(int ofLeft, string var)
                {
                    IsNotUsedIndexes.Add((ofLeft, var));
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
                        RemoveFromIndex(Machine.InstructionPointer, pair.Key);
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