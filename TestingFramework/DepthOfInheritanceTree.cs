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
    public class DepthOfInheritanceTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _treeDictionary = new Dictionary<string, SyntaxTree>();
            // Basic test case, there is only one path, therefore, the complexity is 1
            _treeDictionary.Add("0-CCTree1", CSharpSyntaxTree.ParseText(@"
                class Thing
                {
                    private bool exists;
                }
                
            "));

            _treeDictionary.Add("1-CCTree1", CSharpSyntaxTree.ParseText(@"
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
            "));

            _treeDictionary.Add("2-CCTree1", CSharpSyntaxTree.ParseText(@"
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
                
                class Tiger : Animal
                {
                    public int teeth;
                }
            "));

        }

        [DataTestMethod]
        [DataRow("0-CCTree1")]
        [DataRow("1-CCTree1")]
        [DataRow("2-CCTree1")]
        public void DITTester(string treeKey)
        {
            SyntaxTree tree = _treeDictionary[treeKey];

            int result = int.Parse(treeKey.Split('-')[0]);

            var compilation = CSharpCompilation.Create("Trivial")
                .AddReferences(
                    MetadataReference.CreateFromFile(
                        typeof(object).Assembly.Location))
                .AddSyntaxTrees(tree);

            SemanticModel model = compilation.GetSemanticModel(tree);

            var classDeclarations = tree.GetRoot().ChildNodes();

            foreach (ClassDeclarationSyntax classDeclaration in classDeclarations)
            {
                int cc = DepthOfInheritanceTree.GetCount((ClassDeclarationSyntax)classDeclarations.Last(), model);

                Assert.IsTrue(cc == result);
            }

            
        }
    }
}