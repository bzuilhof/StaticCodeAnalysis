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
    public class CouplingBetweenObjectsTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _treeDictionary = new Dictionary<string, SyntaxTree>();

            _treeDictionary.Add("1-CBOTree1", CSharpSyntaxTree.ParseText(@"
                namespace Something
                {
                    class Animal
                    {
                        private int _legs;
                        private bool _canSwim;
                        private bool _canFly;
                        private Guy owner;
                        public Animal(){
                            owner = new Guy();
                        }
                    }
                    
                    class Guy
                    {
                        public int teeth;
                    }
                }
            "));

            _treeDictionary.Add("2-CBOTree1", CSharpSyntaxTree.ParseText(@"
                namespace Something
                {
                    class Animal
                    {
                        private int _legs;
                        private bool _canSwim;
                        private bool _canFly;
                        private Guy owner;
                        public Animal(){
                            Helper.WriteSomething();

                        }
                    }
                    
                    static class Helper
                    {
                        public static void WriteSomething()
                        {
                            Console.WriteLine(""Something"");
                        }
                    }

                    class Guy
                    {
                        public int teeth;
                    }
                }
            "));

        }
        [DataTestMethod]
        [DataRow("2-CBOTree1")]
        [DataRow("1-CBOTree1")]
        public void CBOTester(string treeKey)
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

            var outgoingCouplings = CouplingBetweenObjects.CalculateCouplings(new List<SyntaxTree> {tree}, compilation);
             
            int cbo = CouplingBetweenObjects.GetCount(classDeclarations.First(), model, outgoingCouplings);

            Assert.IsTrue(cbo == result);
        }
    }
}