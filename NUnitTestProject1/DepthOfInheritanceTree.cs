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
    public class DepthOfInheritanceTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;

        [SetUp]
        public void Setup()
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

        [TestCase("0-CCTree1")]
        [TestCase("1-CCTree1")]
        [TestCase("2-CCTree1")]
        public void DITTester(string treeKey)
        {
            SyntaxTree tree = _treeDictionary[treeKey];

            int result = int.Parse(treeKey.Split("-")[0]);

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