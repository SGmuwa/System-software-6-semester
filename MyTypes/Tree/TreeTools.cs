using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MyTypes.Tree
{
    public static class TreeTools
    {
        private static int maxDeepCount = 1000;

        /// <summary>
        /// Возвращает количество всех потомков, включая текущий узел.
        /// Прошу заметить, что в случае, если потомков 0, то вернётся 1.
        /// </summary>
        /// <exception cref="StackOverflowException">Найдена рекурсия или достигнут лимит размера дерева.</exception>
        public static long GetCountAll<T>(this ITreeNode<T> tree)
        {
            if(--maxDeepCount <= 0)
            {
                maxDeepCount = 1000;
                throw new StackOverflowException();
            }
            long count = 1;
            foreach(ITreeNode<T> child in tree.GetEnumerableOnlyNeighbors())
            {
                count += child.GetCountAll();
            }
            maxDeepCount++;
            return count;
        }

        private readonly static ISet<object> nodesWriter = new MyHashSet<object>();

        public static string ChildrenToString<T>(this ITreeNode<T> root, StringFormat sf = StringFormat.Default, string separator = ", ")
        {
            StringBuilder sb = new StringBuilder();
            foreach(ITreeNode<T> n in root.GetEnumerableOnlyNeighbors())
            {
                sb.Append(n.ToString(sf));
                sb.Append(separator);
            }
            if (sb.Length >= separator.Length)
                sb.Length -= separator.Length;
            return sb.ToString();
        }

        public static TreeNode<T> DeepClone<T>(this ITreeNode<T> root, Func<T, T> CloneCurrent)
        {
            TreeNode<T> output = new TreeNode<T>(CloneCurrent(root.Current));
            foreach(ITreeNode<T> node in root.GetEnumerableOnlyNeighbors())
                output.AddTreeNode(node.DeepClone(CloneCurrent));
            return output;
        }

        /// <summary>
        /// Заменяет все экземпляры в узлах деревьев на новые элементы
        /// по указанному предикату.
        /// </summary>
        /// <param name="root">Начало дерева, его корень.</param>
        /// <param name="match">Метод, который определяет, какие элементы должны быть заменены.</param>
        /// <param name="replaceGenerator">Генератор объектов, которые будут помещены в дерево.</param>
        public static void ReplaceWhere<T>(this ITreeNode<T> root, Predicate<T> match, Func<T> replaceGenerator)
        {
            foreach(ITreeNode<T> node in root)
                if(match(node.Current))
                    node.Current = replaceGenerator();
        }

        /// <summary>
        /// Заменяет все экземпляры в узлах деревьев на новые элементы
        /// по указанному предикату.
        /// Замена корня не поддерживается.
        /// </summary>
        /// <param name="root">Начало дерева, его корень.</param>
        /// <param name="match">Метод, который определяет, какие элементы должны быть заменены.</param>
        /// <param name="replaceGenerator">Генератор объектов, которые будут помещены в дерево.</param>
        public static void ReplaceWhere<T>(this ITreeNode<T> root, Predicate<ITreeNode<T>> match, Func<ITreeNode<T>> replaceGenerator)
        {
            for(int i = 0; i < root.Count; i++)
                if(match(root[i]))
                    root[i] = replaceGenerator();
                else
                    root[i].ReplaceWhere(match, replaceGenerator);
        }

        public static string ToString<T>(this ITreeNode<T> root, StringFormat sf)
        {
            if (root == null)
                return null;
            if (!nodesWriter.Add(root))
                return $"cur: {root.Current}, deep: ...";
            string output;
            try
            {
                output = root.ToStringUnsafe(sf);
            }
            catch
            {
                nodesWriter.Remove(root);
                throw;
            }
            nodesWriter.Remove(root);
            return output;
        }

        private static string ToStringUnsafe<T>(this ITreeNode<T> root, StringFormat sf)
        {
            switch (sf)
            {
                case StringFormat.Default:
                    return $"{root.GetType()}: cur: {root.Current}, deep: [{root.ChildrenToString(sf, ", ")}]";
                case StringFormat.NewLine:
                    {
                        string[] lines = $"{root.Current}{(root.Count > 0 ? ":" : "")}\n<NEEDTAB_TreeNode>{root.ChildrenToString(sf, "\n")}\n</NEEDTAB_TreeNode>".Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        int count = 0, countOpen, countClose;
                        Regex regOpen = new Regex("<NEEDTAB_TreeNode>");
                        Regex regClose = new Regex("</NEEDTAB_TreeNode>");
                        for (long i = 0; i < lines.LongLength; i++)
                        {
                            countOpen = regOpen.Matches(lines[i]).Count;
                            countClose = regClose.Matches(lines[i]).Count;
                            count += countOpen - countClose;
                            if (count > 0)
                                lines[i] = lines[i].Insert(0, new string('\t', count));
                            if (countOpen != 0)
                                lines[i] = lines[i].Replace("<NEEDTAB_TreeNode>", "");
                            if (countClose != 0)
                                lines[i] = lines[i].Replace("</NEEDTAB_TreeNode>", "");
                        }
                        Regex needRemove = new Regex("^[\\t| ]+$");
                        List<string> LinesWithoutSpace = new List<string>(lines);
                        LinesWithoutSpace.RemoveAll(str => needRemove.Match(str).Success || str.Length == 0);
                        return string.Join("\n", LinesWithoutSpace);
                    }
                case StringFormat.Base:
                    return root.GetType().ToString();
                default:
                    throw new NotSupportedException($"Формат {sf} не поддерживается.");
            }
        }
    }
}
