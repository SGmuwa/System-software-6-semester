using Parser;
using System.Collections.Generic;

namespace Optimizing.Example
{
    /// <summary>
    /// Предназначен для применения всех оптимизаций из пространства имён Optimizing.Example.
    /// </summary>
    public class AllOptimizing : IOptimizing
    {
        public static readonly AllOptimizing Instance = new AllOptimizing();

        private static readonly IEnumerable<IOptimizing> optimizations = new IOptimizing[]
        {
            Assign_ExprStmtOptimizing.Instance,
            StmtOptimizing.Instance,
            //OpOptimizing.Instance,
            Assign_ExprRemover.Instance
        };

        private AllOptimizing(){}

        public ReportParser Optimize(ReportParser compiledCode)
        {
            foreach(var o in optimizations)
                compiledCode = o.Optimize(compiledCode);
            return compiledCode;
        }
    }
}