using System;
using System.Collections.Generic;
using System.Collections;

namespace MyTypes.LinkedList
{
    public class MyLinkedList<T> : ICollection<T>, ICollection, IEnumerable<MyLinkedListNode<T>>
    {
        /// <summary>
        /// Возвращает последний узел <see cref="MyLinkedListNode{T}"/> из <see cref="MyLinkedList{T}"/>.
        /// </summary>
        public MyLinkedListNode<T> Last { get; private set; }

        /// <summary>
        /// Возвращает первый <see cref="MyLinkedListNode{T}"/> из <see cref="MyLinkedList{T}"/>.
        /// </summary>
        public MyLinkedListNode<T> First { get; private set; }

        /// <summary>
        /// Возвращает количество узлов, содержащихся в <see cref="MyLinkedList{T}"/>.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Последнее изменение этого листа.
        /// </summary>
        private DateTime lastModifed = DateTime.Now;

        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// Получает объект, с помощью которого можно синхронизировать доступ к коллекции <see cref="MyLinkedList{T}"/>.
        /// </summary>
        object ICollection.SyncRoot => this;

        /// <summary>
        /// Возвращает значение, показывающее, является ли доступ к коллекции <see cref="ICollection"/>
        /// синхронизированным (потокобезопасным).
        /// </summary>
        /// <returns>true, если доступ к классу <see cref="ICollection"/> является синхронизированным
        /// (потокобезопасным); в противном случае — false.</returns>
        public virtual bool IsSynchronized => false;

        void ICollection<T>.Add(T item)
            => AddLast(item);
        
        /// <summary>
        /// Добавляет новый узел, содержащий указанное значение после указанного существующего
        /// узла в <see cref="MyLinkedList{T}"/>.
        /// </summary>
        /// <param name="node"><see cref="MyLinkedListNode{T}"/>, после которого необходимо
        /// вставить новый <see cref="MyLinkedListNode{T}"/> содержащий value.</param>
        /// <param name="value">Значение для добавления <see cref="MyLinkedList{T}"/>.</param>
        /// <returns>Новый <see cref="MyLinkedListNode{T}"/> содержащий value.</returns>
        /// <exception cref="ArgumentNullException">Свойство node имеет значение null.</exception>
        /// <exception cref="InvalidOperationException">node не в текущем <see cref="MyLinkedList{T}"/>.</exception>
        public MyLinkedListNode<T> AddAfter(MyLinkedListNode<T> node, T value)
        {
            lastModifed = DateTime.Now;
            if (node == null)
                throw new ArgumentNullException("Свойство node имеет значение null.");
            if (node.List != this)
                throw new InvalidOperationException("node не в текущем list.");
            Count++;
            MyLinkedListNode<T> toAdd = new MyLinkedListNode<T>(this, node, node.Next, value);
            if(node.Next != null)
            {
                node.Next.Previous = toAdd;
            }
            else
            {
                Last = toAdd;
            }
            node.Next = toAdd;
            return toAdd;
        }

        /// <summary>
        /// Добавляет новый узел, содержащий указанное значение перед указанным узлом, существующие
        /// в <see cref="MyLinkedList{T}"/>.
        /// </summary>
        /// <param name="node"><see cref="MyLinkedListNode{T}"/> перед которым необходимо вставить
        /// новый <see cref="MyLinkedListNode{T}"/> содержащий value.</param>
        /// <param name="value">Значение для добавления <see cref="MyLinkedList{T}"/>.</param>
        /// <returns>Новый <see cref="MyLinkedListNode{T}"/> содержащий value.</returns>
        /// <exception cref="ArgumentNullException">Свойство node имеет значение null.</exception>
        /// <exception cref="InvalidOperationException">node не в текущем <see cref="MyLinkedList{T}"/>.</exception>
        public MyLinkedListNode<T> AddBefore(MyLinkedListNode<T> node, T value)
        {
            lastModifed = DateTime.Now;
            if (node == null)
                throw new ArgumentNullException("Свойство node имеет значение null.");
            if (node.List != this)
                throw new InvalidOperationException("node не в текущем list.");
            Count++;
            MyLinkedListNode<T> toAdd = new MyLinkedListNode<T>(this, node.Previous, node, value);
            if (node.Previous != null)
            {
                node.Previous.Next = toAdd;
            }
            else
            {
                First = toAdd;
            }
            node.Previous = toAdd;
            return toAdd;
        }

        /// <summary>
        /// Добавляет новый узел, содержащий указанное значение в начале <see cref="MyLinkedList{T}"/>.
        /// </summary>
        /// <param name="value">Добавляемое значение в начало <see cref="MyLinkedList{T}"/>.</param>
        /// <returns>Новый <see cref="MyLinkedListNode{T}"/> содержащий value.</returns>
        public MyLinkedListNode<T> AddFirst(T value)
        {
            lastModifed = DateTime.Now;
            Count++;
            if (First != null)
            {
                First.Previous = new MyLinkedListNode<T>(this, null, First, value);
                First = First.Previous;
            }
            else
            {
                First = Last = new MyLinkedListNode<T>(this, null, First, value);
            }
            return First;
        }

        /// <summary>
        /// Добавляет новый узел, содержащий указанное значение в конце <see cref="MyLinkedList{T}"/>.
        /// </summary>
        /// <param name="value">Значение, добавляемое в конце <see cref="MyLinkedList{T}"/>.</param>
        /// <returns>Новый <see cref="MyLinkedListNode{T}"/> содержащий value.</returns>
        public MyLinkedListNode<T> AddLast(T value)
        {
            lastModifed = DateTime.Now;
            Count++;
            if (Last != null)
            {
                Last.Next = new MyLinkedListNode<T>(this, Last, null, value);
                Last = Last.Next;
            }
            else
            {
                Last = First = new MyLinkedListNode<T>(this, Last, null, value);
            }
            return Last;
        }

        /// <summary>
        /// Очистить список.
        /// </summary>
        public void Clear()
        {
            lastModifed = DateTime.Now;
            Last = null;
            First = null;
            Count = 0;
        }

        /// <summary>
        /// Определяет, содержится ли объект в листе.
        /// </summary>
        /// <param name="item">Искомый объект.</param>
        /// <returns>True, если содержится. False - если нет.</returns>
        public bool Contains(T item)
            => Find(item) != null;

        /// <summary>
        /// Находит узел с указанным объектом.
        /// </summary>
        /// <param name="item">Искомый объект.</param>
        /// <returns>Узел листа с искомым объектом.</returns>
        public MyLinkedListNode<T> Find(T item)
        {
            foreach (MyLinkedListNode<T> elm in (IEnumerable<MyLinkedListNode<T>>)this)
            {
                if (elm.Value.Equals(item))
                    return elm;
            }
            return null;
        }

        /// <summary>
        /// Находит узел с указанным объектом.
        /// Поиск начинается с конца.
        /// </summary>
        /// <param name="item">Искомый объект.</param>
        /// <returns>Узел листа с искомым объектом.</returns>
        public MyLinkedListNode<T> FindLast(T item)
        {
            IEnumerator<MyLinkedListNode<T>> enume = GetEnumeratorNode(true);
            while(enume.MoveNext())
            {
                if (enume.Current.Value.Equals(item))
                    return enume.Current;
            }
            return null;
        }

        public void CopyTo(T[] array, int arrayIndex)
            => CopyTo((Array)array, arrayIndex);

        public void CopyTo(Array array, int index)
        {
            foreach (T elm in this)
            {
                array.SetValue(elm, index++);
            }
        }

        public IEnumerator<T> GetEnumerator()
            => new Enumerator(this);

        /// <summary>
        /// Удаляет объект из листа.
        /// </summary>
        /// <param name="item">объект, который надо удалить.</param>
        /// <returns>True, если объект находился в списке. Иначе - false.</returns>
        public bool Remove(T item)
        {
            lastModifed = DateTime.Now;
            MyLinkedListNode<T> ToRemove = Find(item);
            if (ToRemove == null)
                return false;
            Remove(ToRemove);
            return true;
        }

        /// <summary>
        /// Удаляет узел из листа.
        /// </summary>
        /// <param name="node">Узел, который надо удалить.</param>
        public void Remove(MyLinkedListNode<T> node)
        {
            if (node == null)
                throw new ArgumentNullException("Свойство node имеет значение null.");
            if (node.List != this)
                throw new InvalidOperationException("node не в текущем list.");
            Count--;
            if (node.Previous != null)
                node.Previous.Next = node.Next;
            else
                First = node.Next;
            if (node.Next != null)
                node.Next.Previous = node.Previous;
            else
                Last = node.Previous;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        IEnumerator<MyLinkedListNode<T>> GetEnumeratorNode(bool IsReverse = false)
            => new Enumerator(this, IsReverse);

        IEnumerator<MyLinkedListNode<T>> IEnumerable<MyLinkedListNode<T>>.GetEnumerator()
            => GetEnumeratorNode();

        public struct Enumerator : IEnumerator<T>, IEnumerator, IEnumerator<MyLinkedListNode<T>>
        {
            public Enumerator(MyLinkedList<T> baseList, bool isReverse = false)
            {
                Base = baseList;
                lastModifed = baseList.lastModifed;
                IsMoved = false;
                CurrentNode = null;
                IsReverse = isReverse;
            }

            /// <summary>
            /// Основной лист.
            /// </summary>
            private MyLinkedList<T> Base;

            /// <summary>
            /// Текущий узел.
            /// </summary>
            private MyLinkedListNode<T> CurrentNode { get; set; }

            /// <summary>
            /// True, если двигался. False, если нет.
            /// </summary>
            private bool IsMoved;

            /// <summary>
            /// Последнее изменение.
            /// </summary>
            private readonly DateTime lastModifed;

            public bool IsReverse { get; }

            public T Current => CurrentNode.Value;

            object IEnumerator.Current => Current;

            MyLinkedListNode<T> IEnumerator<MyLinkedListNode<T>>.Current => CurrentNode;

            public void Dispose()
            {
                Base = null;
                CurrentNode = null;
            }

            public bool MoveNext()
            {
                if (Base.lastModifed != lastModifed)
                    throw new InvalidOperationException("Коллекция была изменена после создания перечислителя.");
                if (CurrentNode == null)
                {
                    if (IsMoved)
                        return false;
                    IsMoved = true;
                    if (IsReverse)
                        CurrentNode = Base.Last;
                    else
                        CurrentNode = Base.First;
                    return CurrentNode != null;
                }
                if (IsReverse)
                    CurrentNode = CurrentNode.Previous;
                else
                    CurrentNode = CurrentNode.Next;
                return CurrentNode != null;
            }

            public void Reset()
            {
                IsMoved = false;
                CurrentNode = null;
            }
        }
    }
}
