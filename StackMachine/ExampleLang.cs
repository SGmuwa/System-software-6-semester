using MyTypes;
using MyTypes.LinkedList;
using System;
using System.Collections.Generic;
using System.Text;
using static Lexer.ExampleLang;
using static Parser.ExampleLang;
using static Parser.ExampleLang.CommandsList;

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
                [Goto] = _ =>
                {
                    string address = _.Stack.Peek();
                    if(address == "print")
                    {
                        _.Stack.Pop();
                        int output = _.Print(_.PopStk());
                        int addressToGoto = (int)_.PopStk() - 1;
                        _.Stack.Push(output.ToString());
                        _.InstructionPointer = addressToGoto;
                    }
                    else
                        _.InstructionPointer = (int)_.PopStk() - 1;
                },
                [IfNotGoto] = _ =>
                {
                    int addr = (int)_.PopStk();
                    int logical = (int)_.PopStk();
                    if (logical == 0) // Если ложь, то пропускаем body.
                        _.InstructionPointer = addr - 1;
                },
                [Assign] = _ =>
                {
                    double stmt = _.PopStk();
                    string var = _.Stack.Pop();
                    if (IsNumber(var))
                        throw new KeyNotFoundException();
                    _.Variables[var] = stmt;
                },
                [Plus] = _ =>
                {
                    _.Stack.Push(
                        (_.PopStk() + _.PopStk())
                        .ToString());
                },
                [Minus] = _ =>
                {
                    double b = _.PopStk();
                    double a = _.PopStk();
                    _.Stack.Push(
                        (a - b)
                        .ToString());
                },
                [Multiply] = _ =>
                {
                    _.Stack.Push(
                        (_.PopStk() * _.PopStk())
                        .ToString());
                },
                [Divide] = _ =>
                {
                    double b = _.PopStk();
                    double a = _.PopStk();
                    _.Stack.Push(
                        (a / b)
                        .ToString());
                },
                [More] = _ =>
                {
                    double b = _.PopStk();
                    double a = _.PopStk();
                    _.Stack.Push(
                        (a > b)
                        ? "1" : "0");
                },
                [Less] = _ =>
                {
                    double b = _.PopStk();
                    double a = _.PopStk();
                    _.Stack.Push(
                        (a < b)
                        ? "1" : "0");
                },
                [Equal] = _ =>
                {
                    _.Stack.Push(
                        (_.PopStk() == _.PopStk())
                        ? "1" : "0");
                },
                [NotEqual] = _ =>
                {
                    _.Stack.Push(
                        (_.PopStk() != _.PopStk())
                        ? "1" : "0");
                },
                [CommandsList.HASHSET_ADD] = _ =>
                {
                    double buffer = _.PopStk();
                    _.Stack.Push(_.set.Add(buffer) ? "1" : "0");
                },
                [CommandsList.HASHSET_CONTAINS] = _ =>
                {
                    double buffer = _.PopStk();
                    _.Stack.Push(_.set.Contains(buffer) ? "1" : "0");
                },
                [CommandsList.HASHSET_COUNT] = _ =>
                {
                    _.Stack.Push(_.set.Count.ToString());
                },
                [CommandsList.HASHSET_REMOVE] = _ =>
                {
                    double buffer = _.PopStk();
                    _.Stack.Push(_.set.Remove(buffer) ? "1" : "0");
                },
                [NotImplement] = _ =>
                {
                    throw new NotImplementedException();
                },
                [CommandsList.LIST_ADD] = _ =>
                {
                    double buffer = _.PopStk();
                    _.list.Add(buffer);
                    _.Stack.Push("1");
                },
                [CommandsList.LIST_CONTAINS] = _ =>
                {
                    double buffer = _.PopStk();
                    _.Stack.Push(_.list.Contains(buffer) ? "1" : "0");
                },
                [CommandsList.LIST_COUNT] = _ =>
                {
                    _.Stack.Push(_.list.Count.ToString());
                },
                [CommandsList.LIST_REMOVE] = _ =>
                {
                    double buffer = _.PopStk();
                    _.Stack.Push(_.list.Remove(buffer) ? "1" : "0");
                },
                [StackPopDrop] = _ =>
                {
                    _.Stack.Pop();
                },
                [StackSwapLast2] = _ => 
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
                    _.Stack.Push(command);
                }).Invoke(this);
            }

            protected virtual int Print(double value)
            {
                string toWrite = value.ToString();
                Console.WriteLine(toWrite);
                return toWrite.Length;
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
