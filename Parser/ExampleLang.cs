using System.Collections.Generic;
using static Lexer.ExampleLang;
using static Parser.RuleOperator;

namespace Parser
{
    public static class ExampleLang
    {
        private static void OrInserter(List<string> commands, ActionInsert insert, int helper)
            => insert();

        private static TransferToStackCode AndInserter(params int[] order)
        {
            return (List<string> commands, ActionInsert insert, int helper) =>
            {
                int a = 0;
                while (a < order.Length)
                    insert(order[a++]);
            };
        }
        private static void MoreInserter(List<string> commands, ActionInsert insert, int helper)
        {
            for(int i = 0; i < helper; i++)
                insert(i);
        }
        private static void MoreInserterInvert(List<string> commands, ActionInsert insert, int helper)
        {
            for(int i = helper - 1; i >= 0; i--)
                insert(i);
        }
        private static void WordAndValue(List<string> commands, ActionInsert insert, int helper)
        {
            insert(1);
            insert(0);
        }

        /// <summary>
        /// Главный нетерминал.
        /// Возможно, вам нужно использовать <see cref="Lang"/>.
        /// </summary>
        public static readonly Nonterminal lang = new Nonterminal(nameof(lang), MoreInserter, ZERO_AND_MORE),
            value = new Nonterminal(nameof(value), OrInserter, OR),
            command_hash_expr = new Nonterminal(nameof(command_hash_expr), OrInserter, OR,
               new Nonterminal($"{HASHSET_ADD.Name} {value.Name}", WordAndValue, AND, HASHSET_ADD, value),
               new Nonterminal($"{HASHSET_CONTAINS.Name} {value.Name}", WordAndValue, AND, HASHSET_CONTAINS, value),
               new Nonterminal($"{HASHSET_REMOVE.Name} {value.Name}", WordAndValue, AND, HASHSET_REMOVE, value),
               new Nonterminal($"{HASHSET_COUNT.Name}", AndInserter(0), AND, HASHSET_COUNT)),
            command_list_expr = new Nonterminal(nameof(command_list_expr), OrInserter, OR,
               new Nonterminal($"{LIST_ADD.Name} {value.Name}", WordAndValue, AND, LIST_ADD, value),
               new Nonterminal($"{LIST_CONTAINS.Name} {value.Name}", WordAndValue, AND, LIST_CONTAINS, value),
               new Nonterminal($"{LIST_REMOVE.Name} {value.Name}", WordAndValue, AND, LIST_REMOVE, value),
               new Nonterminal($"{LIST_COUNT.Name}", AndInserter(0), AND, LIST_COUNT)),
            stmt =
               new Nonterminal(nameof(stmt), OrInserter, OR,
                   new Nonterminal($"{value.Name} ({OP.Name} {value.Name})*", AndInserter(0, 1), AND,
                       value,
                       new Nonterminal($"({OP.Name} {value.Name})*", MoreInserter, ZERO_AND_MORE,
                           new Nonterminal($"{OP.Name} {value.Name}", AndInserter(1, 0), AND,
                               OP,
                               value
                           )
                       )
                   )
               ),
            VARAndComma = new Nonterminal(nameof(VARAndComma), AndInserter(0), RuleOperator.AND, VAR, COMMA),
            argInit_element = new Nonterminal(nameof(argInit_element), (commands, insert, helper) => {
                insert(); // Вставка переменной.
                commands.Add(CommandsList.StackSwapLast2);
                commands.Add(CommandsList.Assign);
            }, RuleOperator.OR, VAR, VARAndComma),
            argsInit = new Nonterminal(nameof(argsInit), MoreInserter, ZERO_AND_MORE, argInit_element),
            valueAndComma = new Nonterminal(nameof(valueAndComma), AndInserter(0), AND, value, COMMA),
            argCall_element = new Nonterminal(nameof(argCall_element), OrInserter, OR, value, valueAndComma),
            argsCall = new Nonterminal(nameof(argsCall), MoreInserterInvert, ZERO_AND_MORE, argCall_element),
            call_function_expr = new Nonterminal(nameof(call_function_expr), (commands, insert, helper) => {
                int idToReplace = commands.Count;
                commands.Add(CommandsList.NotImplement);
                insert(2);
                insert(0);
                commands.Add(CommandsList.Goto);
                commands[idToReplace] = commands.Count.ToString();
            }, RuleOperator.AND, VAR, L_B, argsCall, R_B),
            call_function_without_output_expr = new Nonterminal(nameof(call_function_without_output_expr), (commands, insert, helper) => {
                int idToReplace = commands.Count;
                commands.Add(CommandsList.NotImplement);
                insert(2);
                insert(0);
                commands.Add(CommandsList.Goto);
                commands[idToReplace] = commands.Count.ToString();
                commands.Add(CommandsList.StackPopDrop);
            }, AND, VAR, L_B, argsCall, R_B),
            body = new Nonterminal(nameof(body), AndInserter(1), AND, L_QB, lang, R_QB),
            initFunction_expr = new Nonterminal(nameof(initFunction_expr), (commands, insert, helper) => {
                int indexToSet = commands.Count;
                commands.Add(CommandsList.NotImplement);
                commands.Add(CommandsList.Goto);
                insert(1);
                insert(4);
                commands.Add(CommandsList.StackSwapLast2);
                commands.Add(CommandsList.Goto);
                commands[indexToSet] = commands.Count.ToString();
                commands.Add((indexToSet + 2).ToString());
            }, AND, L_B, argsInit, R_B, IMPLICATION, body),
            b_val_expr = new Nonterminal(nameof(b_val_expr), OrInserter, OR,
                initFunction_expr,
                new Nonterminal($"{L_B.Name} {stmt.Name} {R_B.Name}", AndInserter(1), AND, L_B, stmt, R_B),
                stmt),
            condition = new Nonterminal(nameof(condition), AndInserter(1), AND, L_B, stmt, R_B),
            for_condition = new Nonterminal(nameof(condition), AndInserter(0), AND, stmt),
            while_expr = new Nonterminal(nameof(while_expr),
               (List<string> commands, ActionInsert insert, int helper) =>
               {
                   int beginWhile = commands.Count;
                   insert(1); // condition
                   int indexAddrFalse = commands.Count;
                   commands.Add(CommandsList.NotImplement); // Адрес, который указывает на то место, куда надо перейти в случае лжи.
                   commands.Add(CommandsList.IfNotGoto);
                   insert(2); // true body
                   commands.Add(beginWhile.ToString());
                   commands.Add(CommandsList.Goto);
                   commands[indexAddrFalse] = commands.Count.ToString();
               }, AND, WHILE_KW, condition, body),
            do_while_expr = new Nonterminal(nameof(do_while_expr),
               (List<string> commands, ActionInsert insert, int helper) =>
               {
                   int beginDo = commands.Count;
                   insert(1);
                   insert(3);
                   int indexOfAddressExit = commands.Count;
                   commands.Add(CommandsList.NotImplement);
                   commands.Add(CommandsList.IfNotGoto);
                   commands.Add(beginDo.ToString()); // Если попали сюда, значит истина. И надо повторить.
                   commands.Add(CommandsList.Goto);
                   commands[indexOfAddressExit] = commands.Count.ToString();
               }
               , AND, DO_KW, body, WHILE_KW, condition),
            assign_expr = new Nonterminal(nameof(assign_expr), AndInserter(0, 2, 1), AND, VAR, ASSIGN_OP, b_val_expr),
            ifelse_expr = new Nonterminal(nameof(ifelse_expr),
               (List<string> commands, ActionInsert insert, int helper) =>
               {
                   insert(1); // condition
                   int indexAddrFalse = commands.Count;
                   commands.Add(CommandsList.NotImplement); // Адрес, который указывает на то место, куда надо перейти в случае лжи.
                   commands.Add(CommandsList.IfNotGoto);
                   insert(2); // true body
                   int indexAddrWriteToEndElse = commands.Count;
                   commands.Add(CommandsList.NotImplement); // Адрес, который указывает на конец body в else.
                   commands.Add(CommandsList.Goto);
                   commands[indexAddrFalse] = commands.Count.ToString();
                   insert(4);
                   commands[indexAddrWriteToEndElse] = commands.Count.ToString();
               }, AND, IF_KW, /*1*/ condition, /*2*/body, ELSE_KW, /*4*/body),
            if_expr = new Nonterminal(nameof(if_expr),
               (List<string> commands, ActionInsert insert, int helper) =>
               {
                   insert(1); // condition
                   int indexAddrFalse = commands.Count;
                   commands.Add(CommandsList.NotImplement); // Адрес, который указывает на то место, куда надо перейти в случае лжи.
                   commands.Add(CommandsList.IfNotGoto);
                   insert(2); // true body
                   commands[indexAddrFalse] = commands.Count.ToString();
               }, AND, IF_KW, /*1*/ condition, /*2*/body),
            if_expr_OR_ifelse_expr = new Nonterminal(nameof(if_expr_OR_ifelse_expr), OrInserter, OR, ifelse_expr, if_expr),
            for_expr = new Nonterminal(nameof(for_expr),
               (List<string> commands, ActionInsert insert, int helper) =>
               {
                   insert(2); // assign_expr
                   int indexCondition = commands.Count;
                   insert(4); // for_condition
                   int indexAddrFalse = commands.Count;
                   commands.Add(CommandsList.NotImplement); // Адрес, который указывает на то место, куда надо перейти в случае лжи.
                   commands.Add(CommandsList.IfNotGoto);
                   insert(8); // true body
                   insert(6); // assign_expr
                   commands.Add(indexCondition.ToString());
                   commands.Add(CommandsList.Goto);
                   commands[indexAddrFalse] = commands.Count.ToString();
               }, AND, FOR_KW, L_B, /*2*/assign_expr, COMMA, /*4*/for_condition, COMMA, /*6*/assign_expr, R_B, /*8*/ body),
            cycle_expr = new Nonterminal(nameof(cycle_expr), OrInserter, OR, while_expr, do_while_expr, for_expr),
            expr = new Nonterminal(nameof(expr), OrInserter, OR,
                call_function_without_output_expr,
                assign_expr,
                if_expr_OR_ifelse_expr,
                cycle_expr,
                command_hash_expr,
                command_list_expr,
                stmt);

        /// <summary>
        /// Свод правил языка.
        /// </summary>
        public static readonly ParserLang Lang = new ParserLang(lang);
        
        static ExampleLang()
        {
            lang.Add(expr);
            value.AddRange(new object[] { command_hash_expr, command_list_expr, call_function_expr, VAR, DIGIT });
        }

        public static class CommandsList
        {
            /// <summary>
            /// Постфиксная запись:
            /// адрес $g
            /// Команда безусловного перехода.
            /// Переход к "адрес".
            /// </summary>
            public static readonly string Goto = "goto!";
            /// <summary>
            /// Постфиксная запись:
            /// условие адрес <see cref="IfNotGoto"/>
            /// Команда условного перехода.
            /// Если условие ложь, то идёт переход к адресу.
            /// </summary>
            public static readonly string IfNotGoto = "!f";
            /// <summary>
            /// Постфиксная запись:
            /// переменная значение <see cref="Assign"/>
            /// Присваивает переменной значение.
            /// </summary>
            public static readonly string Assign = "=";
            /// <summary>
            /// Постфиксная запись:
            /// значение значение <see cref="Plus"/>
            /// Помещает в стек сумму двух значений.
            /// </summary>
            public static readonly string Plus = "+";
            public static readonly string Minus = "-";
            public static readonly string Multiply = "*";
            public static readonly string Divide = "/";
            public static readonly string More = ">";
            public static readonly string Less = "<";
            public static readonly string Equal = "==";
            public static readonly string NotEqual = "!=";
            public static readonly string HASHSET_ADD = Lexer.ExampleLang.HASHSET_ADD.Name;
            public static readonly string HASHSET_CONTAINS = Lexer.ExampleLang.HASHSET_CONTAINS.Name;
            public static readonly string HASHSET_COUNT = Lexer.ExampleLang.HASHSET_COUNT.Name;
            public static readonly string HASHSET_REMOVE = Lexer.ExampleLang.HASHSET_REMOVE.Name;
            /// <summary>
            /// Вызывает резкую остановку программы.
            /// Скорее всего данная ошибка связана из-за того, что компилятор не сумел правильно расставить адреса переходов.
            /// </summary>
            public static readonly string NotImplement = "?";
            public static readonly string LIST_ADD = Lexer.ExampleLang.LIST_ADD.Name;
            public static readonly string LIST_CONTAINS = Lexer.ExampleLang.LIST_CONTAINS.Name;
            public static readonly string LIST_COUNT = Lexer.ExampleLang.LIST_COUNT.Name;
            public static readonly string LIST_REMOVE = Lexer.ExampleLang.LIST_REMOVE.Name;
            /// <summary>
            /// Постфиксная запись:
            /// <see cref="StackPopDrop"/>
            /// Выбрасывает последний элемент из стека.
            /// </summary>
            public static readonly string StackPopDrop = "$stackPopDrop";
            /// <summary>
            /// Постфиксная запись:
            /// <see cref="StackSwapLast2"/>
            /// Меняет местами в стеке два последний элемента.
            /// </summary>
            public static readonly string StackSwapLast2 = "$stackSwapLast2";
        }
    }
}