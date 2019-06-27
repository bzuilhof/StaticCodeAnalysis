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
    public class LambdaMetricsTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _treeDictionary = new Dictionary<string, SyntaxTree>();
           
            // Basic test case, no lambda functions
            _treeDictionary.Add("0-0-0-0-LMTree1", CSharpSyntaxTree.ParseText(@"
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

            _treeDictionary.Add("1-0-0-0-LMTree0", CSharpSyntaxTree.ParseText(@"
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

            _treeDictionary.Add("1-0-0-0-LMTree1", CSharpSyntaxTree.ParseText(@"
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

            _treeDictionary.Add("2-0-0-0-LMTree1", CSharpSyntaxTree.ParseText(@"
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

            _treeDictionary.Add("1-1-0-0-LMTree1", CSharpSyntaxTree.ParseText(@"
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

            _treeDictionary.Add("1-2-0-0-LMTree1", CSharpSyntaxTree.ParseText(@"
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

            _treeDictionary.Add("1-1-0-0-LMTree2", CSharpSyntaxTree.ParseText(@"
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

            _treeDictionary.Add("1-1-1-0-LMTree2", CSharpSyntaxTree.ParseText(@"
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

        _treeDictionary.Add("1-1-0-0-LMTree3", CSharpSyntaxTree.ParseText(@"
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

        _treeDictionary.Add("1-3-0-1-LMTree0", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs){
                        IEnumerable<int> numbers = Enumerable.Range(0, 5);
                        int y = 2;
                        int z = 3;
                        var doubleNumbers = numbers.Select(x =>
                        {
                            z += 10;
                            return x * y * z;
                        });
                    }
                }
            "));
        _treeDictionary.Add("1-3-0-1-LMTree1", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs){
                        IEnumerable<int> numbers = Enumerable.Range(0, 5);
                        int y = 2;
                        int z = 3;
                        var doubleNumbers = numbers.Select(x =>
                        {
                            z = x;
                            return x * y * z;
                        });
                    }
                }
            "));
        _treeDictionary.Add("1-3-0-1-LMTree2", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs){
                        IEnumerable<int> numbers = Enumerable.Range(0, 5);
                        int y = 2;
                        int z = 3;
                        var doubleNumbers = numbers.Select(x =>
                        {
                            z++;
                            return x * y * z;
                        });
                    }
                }
            "));
        }

        [DataTestMethod]
        [DataRow("0-0-0-0-LMTree1")]
        [DataRow("1-0-0-0-LMTree0")]
        [DataRow("1-0-0-0-LMTree1")]
        [DataRow("2-0-0-0-LMTree1")]
        [DataRow("1-1-0-0-LMTree1")]
        [DataRow("1-1-0-0-LMTree2")]
        [DataRow("1-1-0-0-LMTree3")]
        [DataRow("1-2-0-0-LMTree1")]
        [DataRow("1-1-1-0-LMTree2")]
        [DataRow("1-3-0-1-LMTree0")]
        [DataRow("1-3-0-1-LMTree1")]
        [DataRow("1-3-0-1-LMTree2")]
        public void LambdaMetricsTester(string treeKey)
        {
            SyntaxTree tree = _treeDictionary[treeKey];

            var expectedVars = treeKey.Split('-');

            int lc = int.Parse(expectedVars[0]);
            int llu = int.Parse(expectedVars[1]);
            int flu = int.Parse(expectedVars[2]);
            int si = int.Parse(expectedVars[3]);

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
            Assert.IsTrue(lm.SideEffects ==  si);
        }
    }
}
