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
               new Nonterminal("HASHSET_ADD & value", WordAndValue, AND, HASHSET_ADD, value),
               new Nonterminal("HASHSET_CONTAINS & value", WordAndValue, AND, HASHSET_CONTAINS, value),
               new Nonterminal("HASHSET_REMOVE & value", WordAndValue, AND, HASHSET_REMOVE, value),
               new Nonterminal("HASHSET_COUNT", AndInserter(0), AND, HASHSET_COUNT)),
            command_list_expr = new Nonterminal(nameof(command_list_expr), OrInserter, OR,
               new Nonterminal("LIST_ADD & value", WordAndValue, AND, LIST_ADD, value),
               new Nonterminal("LIST_CONTAINS & value", WordAndValue, AND, LIST_CONTAINS, value),
               new Nonterminal("LIST_REMOVE & value", WordAndValue, AND, LIST_REMOVE, value),
               new Nonterminal("LIST_COUNT", AndInserter(0), AND, LIST_COUNT)),
            stmt =
               new Nonterminal(nameof(stmt), OrInserter, OR,
                   new Nonterminal("value (OP value)*", AndInserter(0, 1), AND,
                       value,
                       new Nonterminal("(OP value)*", MoreInserter, ZERO_AND_MORE,
                           new Nonterminal("OP & value", AndInserter(1, 0), AND,
                               "OP",
                               value
                           )
                       )
                   )
               ),
            function_expr = new Nonterminal(nameof(function_expr), AndInserter(), L_B, args, R_B),
            b_val_expr = new Nonterminal(nameof(b_val_expr), OrInserter, OR,
                function_expr,
                new Nonterminal("L_B stmt R_B", AndInserter(1), AND, L_B, stmt, R_B),
                stmt),
            body = new Nonterminal(nameof(body), AndInserter(1), AND, "L_QB", lang, "R_QB"),
            condition = new Nonterminal(nameof(condition), AndInserter(1), AND, L_B, stmt, R_B),
            for_condition = new Nonterminal(nameof(condition), AndInserter(0), AND, stmt),
            while_expr = new Nonterminal(nameof(while_expr),
               (List<string> commands, ActionInsert insert, int helper) =>
               {
                   int beginWhile = commands.Count;
                   insert(1); // condition
                   int indexAddrFalse = commands.Count;
                   commands.Add("?"); // Адрес, который указывает на то место, куда надо перейти в случае лжи.
                   commands.Add("!f");
                   insert(2); // true body
                   commands.Add(beginWhile.ToString());
                   commands.Add("goto!");
                   commands[indexAddrFalse] = commands.Count.ToString();
               }, AND, "WHILE_KW", condition, body),
            do_while_expr = new Nonterminal(nameof(do_while_expr),
               (List<string> commands, ActionInsert insert, int helper) =>
               {
                   int beginDo = commands.Count;
                   insert(1);
                   insert(3);
                   int indexOfAddressExit = commands.Count;
                   commands.Add("?");
                   commands.Add("!f");
                   commands.Add(beginDo.ToString()); // Если попали сюда, значит истина. И надо повторить.
                   commands.Add("goto!");
                   commands[indexOfAddressExit] = commands.Count.ToString();
               }
               , AND, DO_KW, body, WHILE_KW, condition),
            assign_expr = new Nonterminal(nameof(assign_expr), AndInserter(0, 2, 1), AND, VAR, ASSIGN_OP, b_val_expr),
            ifelse_expr = new Nonterminal(nameof(ifelse_expr),
               (List<string> commands, ActionInsert insert, int helper) =>
               {
                   insert(1); // condition
                   int indexAddrFalse = commands.Count;
                   commands.Add("?"); // Адрес, который указывает на то место, куда надо перейти в случае лжи.
                   commands.Add("!f");
                   insert(2); // true body
                   int indexAddrWriteToEndElse = commands.Count;
                   commands.Add("?"); // Адрес, который указывает на конец body в else.
                   commands.Add("goto!");
                   commands[indexAddrFalse] = commands.Count.ToString();
                   insert(4);
                   commands[indexAddrWriteToEndElse] = commands.Count.ToString();
               }, AND, IF_KW, /*1*/ condition, /*2*/body, ELSE_KW, /*4*/body),
            if_expr = new Nonterminal(nameof(if_expr),
               (List<string> commands, ActionInsert insert, int helper) =>
               {
                   insert(1); // condition
                   int indexAddrFalse = commands.Count;
                   commands.Add("?"); // Адрес, который указывает на то место, куда надо перейти в случае лжи.
                   commands.Add("!f");
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
                   commands.Add("?"); // Адрес, который указывает на то место, куда надо перейти в случае лжи.
                   commands.Add("!f");
                   insert(8); // true body
                   insert(6); // assign_expr
                   commands.Add(indexCondition.ToString());
                   commands.Add("goto!");
                   commands[indexAddrFalse] = commands.Count.ToString();
               }, AND, FOR_KW, L_B, /*2*/assign_expr, COMMA, /*4*/for_condition, COMMA, /*6*/assign_expr, R_B, /*8*/ body),
            cycle_expr = new Nonterminal(nameof(cycle_expr), OrInserter, OR, while_expr, do_while_expr, for_expr),
            expr = new Nonterminal(nameof(expr), OrInserter, OR, PRINT_KW, assign_expr, if_expr_OR_ifelse_expr, cycle_expr, command_hash_expr, command_list_expr);

        /// <summary>
        /// Свод правил языка.
        /// </summary>
        public static readonly ParserLang Lang = new ParserLang(lang);
        
        static ExampleLang()
        {
            lang.Add(expr);
            value.AddRange(new object[] { command_hash_expr, command_list_expr, VAR, DIGIT, b_val_expr });
        }
    }
}