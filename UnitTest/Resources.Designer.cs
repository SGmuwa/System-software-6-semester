﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UnitTest {
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("UnitTest.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на a=0
        ///while(a==0){a=a+1}.
        /// </summary>
        internal static string _while => ResourceManager.GetString("_while", resourceCulture);
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на a = 2.
        /// </summary>
        internal static string assign_op => ResourceManager.GetString("assign_op", resourceCulture);
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Abs = 3
        ///Tri = 4
        ///.
        /// </summary>
        internal static string assign_op_multiline => ResourceManager.GetString("assign_op_multiline", resourceCulture);
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на a=0
        ///b=0
        ///if(a==0) {b=b+1}
        ///else {b=b-2}.
        /// </summary>
        internal static string condition => ResourceManager.GetString("condition", resourceCulture);
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на n=2
        ///b=0
        ///for(a=0;a&lt;n;a=a+1 ){b=b+1}.
        /// </summary>
        internal static string cycle_for => ResourceManager.GetString("cycle_for", resourceCulture);
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на a = 0
        ///while(a &lt; 10)
        ///{
        ///	HASHSET_ADD a // Добавление в hashset элементов по-одному.
        ///	a = a + 1
        ///}
        ///test1 = 0
        ///if(HASHSET_CONTAINS 2) // Проверяем, что элемент 2 добавлен.
        ///{
        ///	test1 = 1 // Тест пройден.
        ///}
        ///// Проверяем удаление из hashset:
        ///HASHSET_REMOVE 2
        ///test2 = 0
        ///if((HASHSET_CONTAINS 2) == 0) // Если в hashset отсутствует элемент &quot;2&quot;.
        ///{
        ///	test2 = 1 // тест пройден.
        ///}
        ///// 
        ///// Проверяем на list.
        ///// 
        ///for(i = 0; i &lt; 10; i = i + 1)
        ///{
        ///	LIST_ADD i
        ///}
        ///i = 0
        ///test3 = 1
        ///do
        ///{
        ///	if((LIST_CONTAINS i) = [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string LangExample => ResourceManager.GetString("LangExample", resourceCulture);
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на a = 1 + 2 * b / s - 2.
        /// </summary>
        internal static string op => ResourceManager.GetString("op", resourceCulture);
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на abs = 3
        ///bsa = 3 + 2
        ///aaa = aaa + aaa - 2 + aaa.
        /// </summary>
        internal static string Parser_assign_op_full => ResourceManager.GetString("Parser_assign_op_full", resourceCulture);
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на do
        ///{
        ///	print
        ///}
        ///while(a &lt; 2).
        /// </summary>
        internal static string Parser_do_while => ResourceManager.GetString("Parser_do_while", resourceCulture);
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на for(a = 0; (a &lt; 2); a = a + 1)
        ///{
        ///	print
        ///}
        ///.
        /// </summary>
        internal static string Parser_for => ResourceManager.GetString("Parser_for", resourceCulture);
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на basd = 0
        ///while(basd &lt; 10)
        ///{
        ///	print
        ///	basd = 10
        ///}
        ///basd = 0.
        /// </summary>
        internal static string Parser_var_op_while_print_var => ResourceManager.GetString("Parser_var_op_while_print_var", resourceCulture);
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на = a = b b = a = a bsdd asdqwf bvrekb =grge=g=g=hhre=.
        /// </summary>
        internal static string ParserONE_AND_MORE_OR__ASSIGN_OP__VAR => ResourceManager.GetString("ParserONE_AND_MORE_OR__ASSIGN_OP__VAR", resourceCulture);
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на =.
        /// </summary>
        internal static string ParserOR_assign_op => ResourceManager.GetString("ParserOR_assign_op", resourceCulture);
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на print.
        /// </summary>
        internal static string print_kw => ResourceManager.GetString("print_kw", resourceCulture);
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на a = 2
        ///print.
        /// </summary>
        internal static string Stack_var_print => ResourceManager.GetString("Stack_var_print", resourceCulture);

        internal static string LangExampleJson => ResourceManager.GetString("LangExampleJson", resourceCulture);

        internal static string function => ResourceManager.GetString("function", resourceCulture);
    }
}
