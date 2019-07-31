using System;
using System.Collections.Generic;
using System.Linq;
using CodeQualityAnalyzer.CodeMetrics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace TestFramework
{
    public class CouplingBetweenObjectsTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;

        [SetUp]
        public void Setup()
        {
            _treeDictionary = new Dictionary<string, SyntaxTree>();

            _treeDictionary.Add("1-CCTree1", CSharpSyntaxTree.ParseText(@"
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

            _treeDictionary.Add("2-CCTree1", CSharpSyntaxTree.ParseText(@"
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

        [TestCase("2-CCTree1")]
        [TestCase("1-CCTree1")]
        public void NOCTester(string treeKey)
        {
            SyntaxTree tree = _treeDictionary[treeKey];

            int result = int.Parse(treeKey.Split("-")[0]);

            CSharpCompilation compilation = CSharpCompilation.Create("Trivial")
                .AddReferences(
                    MetadataReference.CreateFromFile(
                        typeof(object).Assembly.Location))
                .AddSyntaxTrees(tree);

            SemanticModel model = compilation.GetSemanticModel(tree);

            IEnumerable<ClassDeclarationSyntax> classDeclarations =  MetricRunner.GetClassesFromRoot(tree.GetRoot());

            var outgoingCouplings = CouplingBetweenObjects.CalculateCouplings(new List<SyntaxTree> {tree}, compilation);

             
            int noc = CouplingBetweenObjects.GetCount(classDeclarations.First(), model, outgoingCouplings);

            Assert.IsTrue(noc == result);
        }
    }
}