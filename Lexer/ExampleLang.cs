using System.Collections.Generic;

namespace Lexer
{
    public static class ExampleLang
    {
        public readonly static Terminal
                ASSIGN_OP        = new Terminal(nameof(ASSIGN_OP),        "^=$"                     ),
                VAR              = new Terminal(nameof(VAR),              "^[a-zA-Z][a-zA-Z0-9]*$"  , uint.MaxValue - 1),
                DIGIT            = new Terminal(nameof(DIGIT),            "^0|([1-9][0-9]*)$"       ),
                OP               = new Terminal(nameof(OP),               "^\\+|-|\\*|/|>|<|>=|<=|==$"),
                WHILE_KW         = new Terminal(nameof(WHILE_KW),         "^while$"                 , 0),
                DO_KW            = new Terminal(nameof(DO_KW),            "^do$"                    , 0),
                PRINT_KW         = new Terminal(nameof(PRINT_KW),         "^print$"                 , 0),
                FOR_KW           = new Terminal(nameof(FOR_KW),           "^for$"                   , 0),
                IF_KW            = new Terminal(nameof(IF_KW),            "^if$"                    , 0),
                ELSE_KW          = new Terminal(nameof(ELSE_KW),          "^else$"                  , 0),
                L_QB             = new Terminal(nameof(L_QB),             "^{$"                     ),
                R_QB             = new Terminal(nameof(R_QB),             "^}$"                     ),
                L_B              = new Terminal(nameof(L_B),              "^\\($"                   ),
                R_B              = new Terminal(nameof(R_B),              "^\\)$"                   ),
                COMMA            = new Terminal(nameof(COMMA),            "^;$"                     ),
                COM              = new Terminal(nameof(COM),              "^,$"                     ),
                HASHSET_ADD      = new Terminal(nameof(HASHSET_ADD),      "^HASHSET_ADD$"           ),
                HASHSET_CONTAINS = new Terminal(nameof(HASHSET_CONTAINS), "^HASHSET_CONTAINS$"      ),
                HASHSET_REMOVE   = new Terminal(nameof(HASHSET_REMOVE),   "^HASHSET_REMOVE$"        ),
                HASHSET_COUNT    = new Terminal(nameof(HASHSET_COUNT),    "^HASHSET_COUNT$"         ),
                LIST_ADD         = new Terminal(nameof(LIST_ADD),         "^LIST_ADD$"              ),
                LIST_CONTAINS    = new Terminal(nameof(LIST_CONTAINS),    "^LIST_CONTAINS$"         ),
                LIST_REMOVE      = new Terminal(nameof(LIST_REMOVE),      "^LIST_REMOVE$"           ),
                LIST_COUNT       = new Terminal(nameof(LIST_COUNT),       "^LIST_COUNT$"            ),
                IMPLICATION      = new Terminal(nameof(IMPLICATION),      "^=>$"                    ),
                /*                                                                                 
                 Те терминалы, которые ниже, по-сути нужны парсеру.                                
                 Для того, чтобы проанализировать выражение:                                       
                 a = nameof(Привет) мир!",                                                         
                 чтобы не было так:                                                                
                 a=nameof(Привет)мир!".                                                            
                 */                                                                                
                CH_LISTORSETMAYBE= new Terminal(nameof(CH_LISTORSETMAYBE), "^[a-zA-Z_]+$"           , uint.MaxValue),
                CH_COMMENT       = new Terminal(nameof(CH_COMMENT),       "^\\/\\/.*$"              ),
                CH_SPACE         = new Terminal(nameof(CH_SPACE),         "^ $"                     ),
                CH_LEFTLINE      = new Terminal(nameof(CH_LEFTLINE),      "^\r$"                    ),
                CH_NEWLINE       = new Terminal(nameof(CH_NEWLINE),       "^\n$"                    ),
                CH_TAB           = new Terminal(nameof(CH_TAB),           "^\t$"                    );
        
        public static readonly LexerLang Lang = new LexerLang(new List<Terminal>()
            {
                ASSIGN_OP, VAR, DIGIT, OP, WHILE_KW, DO_KW, PRINT_KW,
                FOR_KW, IF_KW, ELSE_KW, L_QB, R_QB, L_B, R_B, COMMA, COM,
                HASHSET_ADD, HASHSET_CONTAINS, HASHSET_REMOVE, HASHSET_COUNT, LIST_ADD, LIST_CONTAINS,
                LIST_REMOVE, LIST_COUNT, IMPLICATION, CH_LISTORSETMAYBE, CH_COMMENT, CH_SPACE, CH_LEFTLINE, CH_NEWLINE, CH_TAB
            });
    }
}
