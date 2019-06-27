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
    public class SourceLinesOfLambdaTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _treeDictionary = new Dictionary<string, SyntaxTree>();
            // Basic test case, there is only one path, therefore, the complexity is 1

            // Basic test case, there is still one path, therefore, the complexity is 1
            _treeDictionary.Add("0-CCTree1", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs){
                        _legs = legs;                    
                    }
                }
            "));

            _treeDictionary.Add("6-CCTree0", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;

                     private IEnumerable<int> bla = Enumerable.Range(1, 10)
                        .Select(i => i * 10) //map
                        .Where(i => i % 2 == 0) //filter
                        .OrderBy(i =>
                            {
                                return -i;
                            }); //sort
                    
                }
            "));
        }

        [DataTestMethod]
        [DataRow("0-CCTree1")]
        [DataRow("6-CCTree0")]
        public void slolTester(string treeKey)
        {
            SyntaxTree tree = _treeDictionary[treeKey];

            int result = int.Parse(treeKey.Split('-')[0]);

            var classDeclaration = (ClassDeclarationSyntax)tree.GetRoot().ChildNodes().Single();

            int sloc = SourceLinesOfCode.GetCount(classDeclaration);
            int slol = SourceLinesOfLambda.GetCount(classDeclaration);

            Assert.IsTrue(slol == result);
        }
    }
}