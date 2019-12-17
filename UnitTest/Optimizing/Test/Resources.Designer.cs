﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Optimizing.Test {
    
    /// <summary>
    /// Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [DebuggerNonUserCodeAttribute()]
    [CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static ResourceManager resourceMan;
        
        [SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    ResourceManager temp = new ResourceManager("UnitTest.Optimizing.Test.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture { get; set; }

        internal static string GetString(string ResourceName, CultureInfo ci = null) => ResourceManager.GetString(ResourceName, ci == null ? Culture : ci);
        
        /// <summary>
        /// Hey! It is work!
        /// </summary>
        internal static string ResxTest => GetString("ResxTest", Culture);
        /// <summary>
        /// a = 1 + 1
        /// </summary>
        internal static string OptimizeFirst => GetString("OptimizeFirst", Culture);
        /// <summary>
        /// a = 1 + 2
        /// b = a * 2
        /// </summary>
        internal static string VarInVar => GetString("VarInVar", Culture);
        /// <summary>
        /// a = 3
        /// a = 7
        /// b = a * 2
        /// </summary>
        internal static string VarVarInVar => GetString("VarVarInVar", Culture);
        /// <summary>
        /// if(1 + 2 == 3)
        /// {
        ///     a = 1;
        /// }
        /// b = 2;
        /// </summary>
        internal static string If => GetString("If", Culture);
        internal static string LangExample => GetString("LangExample", Culture);
        internal static string FunctionCalculate => GetString("FunctionCalculate", Culture);
    }
}
