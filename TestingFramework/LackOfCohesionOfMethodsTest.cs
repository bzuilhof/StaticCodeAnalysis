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
    public class LackOfCohesionOfMethodsTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _treeDictionary = new Dictionary<string, SyntaxTree>();
            // Basic test case, there is only one path, therefore, the complexity is 1

            _treeDictionary.Add("3-LCOMTree1", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _a;
                    private int _b;
                    private int _c;

                    private void ChangeA()
                    {
                        _a = 1;
                    }

                    private void ChangeB()
                    {
                        _b = 1;
                    }

                    private void ChangeC()
                    {
                        _c = 1;
                    }
                }
            "));

            _treeDictionary.Add("0-LCOMTree0", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _a;
                    private int _b;
                    private int _c;

                    private void ChangeA()
                    {
                        _a = 1;
                    }

                    private void ChangeB()
                    {
                        _b = 1;
                    }

                    private void ChangeC()
                    {
                        _c = 1;
                    }

                    private void ChangeAB()
                    {
                        _a = 1;
                        _b = 1;
                    }

                    private void ChangeAC()
                    {
                        _a = 1;
                        _c = 1;
                    }
                }
            "));
            _treeDictionary.Add("0-LCOMTree1", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _a;
                    private int _b;
                    private int _c;

                    private void ChangeA()
                    {
                        _a = 1;
                    }

                    private void ChangeB()
                    {
                        _b = 1;
                    }

                    private void ChangeC()
                    {
                        _c = 1;
                    }

                    private void ChangeABC()
                    {
                        _a = 1;
                        _b = 1;
                        _c = 1;
                    }

                    private void ChangeAC()
                    {
                        _a = 1;
                        _c = 1;
                    }
                }
            "));

            
        }

        [DataTestMethod]
        [DataRow("0-LCOMTree0")]
        [DataRow("0-LCOMTree1")]
        [DataRow("3-LCOMTree1")]
        public void LcomTester(string treeKey)
        {
            SyntaxTree tree = _treeDictionary[treeKey];

            int result = int.Parse(treeKey.Split('-')[0]);

            var classDeclaration = (ClassDeclarationSyntax)tree.GetRoot().ChildNodes().Single();

            var compilation = CSharpCompilation.Create("Trivial")
                .AddReferences(
                    MetadataReference.CreateFromFile(
                        typeof(object).Assembly.Location))
                .AddSyntaxTrees(tree);

            SemanticModel semanticModel = compilation.GetSemanticModel(tree);

            int lcom = LackOfCohesionOfMethods.GetCount(classDeclaration, semanticModel);

            Assert.IsTrue(lcom == result);
        }
    }
}