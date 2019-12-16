using MyTypes;
using MyTypes.LinkedList;
using System;
using System.Collections.Generic;
using System.Text;
using static Lexer.ExampleLang;

namespace StackMachine
{
    public static class ExampleLang
    {
        public static readonly MyStackLang StackMachine = new MyStackLang();

        public class MyStackLang : AbstractStackExecuteLang
        {
            /// <summary>
            /// Создаёт новый экземпляр стековой машины.
            /// </summary>
            /// <param name="startVariables">Реализация таблицы переменных.</param>
            public MyStackLang(IDictionary<string, double> startVariables = null)
                : base(startVariables)
            { }

            protected readonly Dictionary<string, Action<MyStackLang>> commands = new Dictionary<string, Action<MyStackLang>>()
            {
                ["print"] = _ =>
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var pair in _.Variables)
                    {
                        sb.Append(pair.Key);
                        sb.Append(" = ");
                        sb.Append(pair.Value);
                        sb.AppendLine();
                    }
                    Console.Write(sb.ToString());
                },
                ["goto!"] = _ =>
                {
                    _.InstructionPointer = (int)_.PopStk() - 1;
                },
                ["!f"] = _ =>
                {
                    int addr = (int)_.PopStk();
                    int logical = (int)_.PopStk();
                    if (logical == 0) // Если ложь, то пропускаем body.
                        _.InstructionPointer = addr - 1;
                },
                ["="] = _ =>
                {
                    double stmt = _.PopStk();
                    string var = _.Stack.Pop();
                    if (IsNumber(var))
                        throw new KeyNotFoundException();
                    _.Variables[var] = stmt;
                },
                ["+"] = _ =>
                {
                    _.Stack.Push(
                        (_.PopStk() + _.PopStk())
                        .ToString());
                },
                ["-"] = _ =>
                {
                    double b = _.PopStk();
                    double a = _.PopStk();
                    _.Stack.Push(
                        (a - b)
                        .ToString());
                },
                ["*"] = _ =>
                {
                    _.Stack.Push(
                        (_.PopStk() * _.PopStk())
                        .ToString());
                },
                ["/"] = _ =>
                {
                    double b = _.PopStk();
                    double a = _.PopStk();
                    _.Stack.Push(
                        (a / b)
                        .ToString());
                },
                [">"] = _ =>
                {
                    double b = _.PopStk();
                    double a = _.PopStk();
                    _.Stack.Push(
                        (a > b)
                        ? "1" : "0");
                },
                ["<"] = _ =>
                {
                    double b = _.PopStk();
                    double a = _.PopStk();
                    _.Stack.Push(
                        (a < b)
                        ? "1" : "0");
                },
                ["=="] = _ =>
                {
                    _.Stack.Push(
                        (_.PopStk() == _.PopStk())
                        ? "1" : "0");
                },
                ["!="] = _ =>
                {
                    _.Stack.Push(
                        (_.PopStk() != _.PopStk())
                        ? "1" : "0");
                },
                [HASHSET_ADD.Name] = _ =>
                {
                    double buffer = _.PopStk();
                    _.Stack.Push(_.set.Add(buffer) ? "1" : "0");
                },
                [HASHSET_CONTAINS.Name] = _ =>
                {
                    double buffer = _.PopStk();
                    _.Stack.Push(_.set.Contains(buffer) ? "1" : "0");
                },
                [HASHSET_COUNT.Name] = _ =>
                {
                    _.Stack.Push(_.set.Count.ToString());
                },
                [HASHSET_REMOVE.Name] = _ =>
                {
                    double buffer = _.PopStk();
                    _.Stack.Push(_.set.Remove(buffer) ? "1" : "0");
                },
                ["?"] = _ =>
                {
                    throw new NotImplementedException();
                },
                [LIST_ADD.Name] = _ =>
                {
                    double buffer = _.PopStk();
                    _.list.Add(buffer);
                    _.Stack.Push("1");
                },
                [LIST_CONTAINS.Name] = _ =>
                {
                    double buffer = _.PopStk();
                    _.Stack.Push(_.list.Contains(buffer) ? "1" : "0");
                },
                [LIST_COUNT.Name] = _ =>
                {
                    _.Stack.Push(_.list.Count.ToString());
                },
                [LIST_REMOVE.Name] = _ =>
                {
                    double buffer = _.PopStk();
                    _.Stack.Push(_.list.Remove(buffer) ? "1" : "0");
                },
                ["$stackPopDrop"] = _ =>
                {
                    _.Stack.Pop();
                },
                ["$stackSwapLast2"] = _ => 
                {
                    string a = _.Stack.Pop();
                    string b = _.Stack.Pop();
                    _.Stack.Push(a);
                    _.Stack.Push(b);
                }
            };

            public readonly ICollection<double> list
                = new MyLinkedList<double>();
            public readonly ISet<double> set
                = new MyHashSet<double>();

            protected override void ExecuteCommand(string command)
            {
                commands.GetValueOrDefault(command, _ => 
                { // Объявлена новая переменная.
                    if (!_.Variables.ContainsKey(command) && !IsNumber(command))
                    {
                    }
                    _.Stack.Push(command);
                }).Invoke(this);
            }

            /// <summary>
            /// Получает с стека значение и вызывает <see cref="GetValueOfVarOrDigit(string)"/>.
            /// </summary>
            protected double PopStk() => GetValueOfVarOrDigit(Stack.Pop());

            protected static bool IsNumber(string str)
                => double.TryParse(str, out double _);

            protected double GetValueOfVarOrDigit(string VarOrDigit)
            {
                if (double.TryParse(VarOrDigit, out double result))
                    return result;
                return Variables[VarOrDigit];
            }
        }
    }
}
