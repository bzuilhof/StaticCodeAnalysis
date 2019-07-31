using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeQualityAnalyzer.CodeMetrics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace TestFramework
{
    class LambdaMetricsTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;

        [SetUp]
        public void Setup()
        {
            _treeDictionary = new Dictionary<string, SyntaxTree>();
           
            // Basic test case, no lambda functions
            _treeDictionary.Add("0-0-0-LMTree1", CSharpSyntaxTree.ParseText(@"
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

            _treeDictionary.Add("1-0-0-LMTree0", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs){
                        _legs = legs;
                        Func<int, int> doubler = (x => x * 2);
                    }
                }
            "));

            _treeDictionary.Add("1-0-0-LMTree1", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs){
                        int[] numbers = { 1, 2, 3, 4, 5 };
                        IEnumerable<int> numbi = from numb in numbers select *;
                        var doubleNumbers = numbers.Select(x => x * 2);            
                    }
                }
            "));

            _treeDictionary.Add("2-0-0-LMTree1", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs){
                        int[] numbers = { 1, 2, 3, 4, 5 };
                        IEnumerable<int> numbi = from numb in numbers select *;
                        var doubleNumbers = numbers.Select(x => x * 2).Select(x => x * 2);            
                    }
                }
            "));

            _treeDictionary.Add("1-1-0-LMTree1", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs){
                        int[] numbers = { 1, 2, 3, 4, 5 };
                        IEnumerable<int> numbi = from numb in numbers select *;
                        int y = 2;
                        var doubleNumbers = numbers.Select(x => x * y);            
                    }
                }
            "));

            _treeDictionary.Add("1-2-0-LMTree1", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs){
                        int[] numbers = { 1, 2, 3, 4, 5 };
                        IEnumerable<int> numbi = from numb in numbers select *;
                        int y = 2;
                        int z = 3;
                        var doubleNumbers = numbers.Select(x => x * y * z);            
                    }
                }
            "));

            _treeDictionary.Add("1-1-0-LMTree2", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs){
                        int[] numbers = { 1, 2, 3, 4, 5 };
                        IEnumerable<int> numbi = from numb in numbers select *;
                        const int y = 2;
                        int z = 3;
                        var doubleNumbers = numbers.Select(x => x * y * z);            
                    }
                }
            "));

            _treeDictionary.Add("1-1-1-LMTree2", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    private int y = 2;

                    public Animal(int legs){
                        int[] numbers = { 1, 2, 3, 4, 5 };
                        IEnumerable<int> numbi = from numb in numbers select *;
                        int z = 3;
                        var doubleNumbers = numbers.Select(x => x * y * z);            
                    }
                }
            "));

        _treeDictionary.Add("1-1-0-LMTree3", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    private const int y = 2;

                    public Animal(int legs){
                        int[] numbers = { 1, 2, 3, 4, 5 };
                        IEnumerable<int> numbi = from numb in numbers select *;
                        int z = 3;
                        var doubleNumbers = numbers.Select(x => x * y * z);            
                    }
                }
            "));
        }

        [TestCase("0-0-0-LMTree1")]
        [TestCase("1-0-0-LMTree0")]
        [TestCase("1-0-0-LMTree1")]
        [TestCase("2-0-0-LMTree1")]
        [TestCase("1-1-0-LMTree1")]
        [TestCase("1-1-0-LMTree2")]
        [TestCase("1-1-0-LMTree3")]
        [TestCase("1-2-0-LMTree1")]
        [TestCase("1-1-1-LMTree2")]
        public void LambdaMetricsTester(string treeKey)
        {
            SyntaxTree tree = _treeDictionary[treeKey];

            var expectedVars = treeKey.Split("-");

            int lc = int.Parse(expectedVars[0]);
            int llu = int.Parse(expectedVars[1]);
            int flu = int.Parse(expectedVars[2]);

            var classDeclaration = (ClassDeclarationSyntax)tree.GetRoot().ChildNodes().Single();

            var compilation = CSharpCompilation.Create("Trivial")
                .AddReferences(
                    MetadataReference.CreateFromFile(
                        typeof(object).Assembly.Location))
                .AddSyntaxTrees(tree);

            SemanticModel model = compilation.GetSemanticModel(tree);

            var lm = LambdaMetrics.GetValueList(classDeclaration, model);

            Assert.IsTrue(lm.LambdaCount == lc);
            Assert.IsTrue(lm.LocalVariableUsageCount == llu);
            Assert.IsTrue(lm.FieldVariableUsageCount ==  flu);
        }
    }
}
