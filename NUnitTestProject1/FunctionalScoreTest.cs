using System.Collections.Generic;
using System.Linq;
using CodeQualityAnalyzer.CodeMetrics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace TestFramework
{
    public class FunctionalScoreTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;

        [SetUp]
        public void Setup()
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

            _treeDictionary.Add("31-CCTree0", CSharpSyntaxTree.ParseText(@"
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


        [TestCase("0-CCTree1")]
        [TestCase("31-CCTree0")]
        
        public void LocTester(string treeKey)
        {
            SyntaxTree tree = _treeDictionary[treeKey];

            int result = int.Parse(treeKey.Split("-")[0]);

            var classDeclaration = (ClassDeclarationSyntax)tree.GetRoot().ChildNodes().Single();

            int sloc = SourceLinesOfCode.GetCount(classDeclaration);
            int fs = LambdaScore.GetScore(classDeclaration, sloc);

            Assert.IsTrue(fs == result);
        }
    }
}