using System;
using System.Collections.Generic;
using System.Linq;
using CodeQualityAnalyzer.CodeMetrics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestingFramework
{
    [TestClass]
    public class NumberOfChildrenTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _treeDictionary = new Dictionary<string, SyntaxTree>();

            _treeDictionary.Add("3-NocTree1", CSharpSyntaxTree.ParseText(@"
                namespace Something
                {
                    class Thing
                    {
                        private bool exists;
                    }

                    class Animal : Thing
                    {
                        private int _legs;
                        private bool _canSwim;
                        private bool _canFly;
                    }
                    
                    class Tiger : Thing
                    {
                        public int teeth;
                    }

                    class Guy : Thing
                    {
                        public int teeth;
                    }
                }
            "));

        }
        [DataTestMethod]
        [DataRow("3-NocTree1")]
        public void NocTester(string treeKey)
        {
            SyntaxTree tree = _treeDictionary[treeKey];

            int result = int.Parse(treeKey.Split('-')[0]);

            CSharpCompilation compilation = CSharpCompilation.Create("Trivial")
                .AddReferences(
                    MetadataReference.CreateFromFile(
                        typeof(object).Assembly.Location))
                .AddSyntaxTrees(tree);

            SemanticModel model = compilation.GetSemanticModel(tree);

            IEnumerable<ClassDeclarationSyntax> classDeclarations =  MetricRunner.GetClassesFromRoot(tree.GetRoot());

            Dictionary<INamedTypeSymbol, int> classExtensions = NumberOfChildren.GetClassExtensions(new List<SyntaxTree> {tree}, compilation);
             
            int noc = NumberOfChildren.GetCount(classDeclarations.First(), model, classExtensions);

            Assert.IsTrue(noc == result);
        }
    }
}