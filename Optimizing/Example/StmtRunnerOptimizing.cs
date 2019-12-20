using Parser;
using MyTypes.Tree;
using StackMachine;

namespace Optimizing.Example
{
    /// <summary>
    /// Выполняет код.
    /// Находит все OP, которые использовались только один раз.
    /// 
    /// </summary>
    class OpOptimizing : IOptimizing
    {
        public static readonly OpOptimizing Instance = new OpOptimizing();

        private OpOptimizing(){}

        public ReportParser Optimize(ReportParser compiledCode)
        {
            if(!compiledCode.IsSuccess)
                throw new OptimizingException("Входное дерево компиляции построено не верно!");
            if(compiledCode.Compile == null)
                throw new OptimizingException("Вызовите compiledCode.Compile() перед началом.");
            ITreeNode<object> treeCompileForCheckVars = compiledCode.Compile.CloneCompileTree();

            throw new System.NotImplementedException();
        }

        private class OptimizingStackMachine : StackMachine.ExampleLang.MyMachineLang
        {
            public OptimizingStackMachine() : base(null) {}

            
        }
    }
}