using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StackMachine
{
    public abstract class AbstractStackExecuteLang : IExecuteLang
    {

        /// <summary>
        /// Создаёт новый экземпляр абстрактной стековой машины.
        /// </summary>
        /// <param name="startVariables">Реализация таблицы переменных.</param>
        protected AbstractStackExecuteLang(IDictionary<string, double> startVariables = null)
        {
            if(startVariables != null)
                Variables = startVariables;
            else
                Variables = Variables = new Dictionary<string, double>();
        }

        /// <summary>
        /// Таблица переменных для стековой машины.
        /// </summary>
        public IDictionary<string, double> Variables { get; protected set; }

        /// <summary>
        /// Стек, который хранит в себе исполняемый код.
        /// </summary>
        protected readonly Stack<string> Stack
            = new Stack<string>();

        /// <summary>
        /// Указатель на текущую выполняемую операцию.
        /// </summary>
        public int InstructionPointer { get; protected set; } = -1;

        public virtual void Execute(IEnumerable<string> code)
        {
            if (code == null)
                throw new ArgumentNullException();
            else if (code is LinkedList<string> list)
                Execute(list);
            else if (code is IList<string> list1)
                ((IExecuteLang)this).Execute(list1);
            else
                ExecuteIEnumerable(code);
        }

        void IExecuteLang.Execute(IList<string> code)
        {
            while (++InstructionPointer < code.Count)
                ExecuteCommand(code[InstructionPointer]);
        }

        private void Execute(LinkedList<string> code)
        {
            if (code.Count < 1)
                return;
            int i = -1;
            LinkedListNode<string> command = code.First;
            while (command != null)
            {
                if (i == InstructionPointer)
                {
                    i++; InstructionPointer++;
                    ExecuteCommand(command.Value);
                    command = command.Next;
                }
                else if (i < InstructionPointer)
                {
                    i++;
                    command = command.Next;
                }
                else
                {
                    i--;
                    command = command.Previous;
                }
            }
        }

        private void ExecuteIEnumerable(IEnumerable<string> code)
        {
            int i = -1;
            IEnumerator<string> command = code.GetEnumerator();
            while (command.MoveNext())
            {
                i++;
                if (i - 1 == InstructionPointer)
                {
                    InstructionPointer++;
                    ExecuteCommand(command.Current);
                }
                else if (i < InstructionPointer)
                {

                }
                else
                {
                    i = -1;
                    command.Reset();
                }
            }
        }

        protected abstract void ExecuteCommand(string command);
    }
}
