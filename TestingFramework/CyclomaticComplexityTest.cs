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
    public class CyclomaticComplexityTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _treeDictionary = new Dictionary<string, SyntaxTree>();
            // Basic test case, there is only one path, therefore, the complexity is 1
            _treeDictionary.Add("1-CCTree0", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                }
            "));

            // Basic test case, there is still one path, therefore, the complexity is 1
            _treeDictionary.Add("1-CCTree1", CSharpSyntaxTree.ParseText(@"
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

            // Test case, there is still one path, therefore, the complexity is 2
            _treeDictionary.Add("2-CCTree2", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs){
                        if(legs % 2 == 1) _legs = legs + 1;
                        else _legs = legs;                    
                    }
                }
            "));

            // Test case, there is still one path, therefore, the complexity is 2
            _treeDictionary.Add("3-CCTree0", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs){
                        if(legs % 2 == 1) _legs = legs + 1;
                        else if (legs > 1000) _legs = 1000;
                        else _legs = legs;                    
                    }
                }
            "));
            // Test case, there is still one path, therefore, the complexity is 2
            _treeDictionary.Add("4-CCTree0", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs){
                        if(legs % 2 == 1) _legs = legs + 1;
                        else if (legs > 1000) _legs = 1000;
                        else _legs = legs;                    
                    }

                    public int GetLegs() {
                        int result = 1;
                        for(int i = 0; i <= _legs; i++;) {
                            result++;
                        }

                        return result;
                    }
                }
            "));
            // Test case, there is still one path, therefore, the complexity is 2
            _treeDictionary.Add("5-CCTree0", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs){
                        if(legs % 2 == 1) _legs = legs + 1;
                        else if (legs > 1000 || legs < 0) _legs = 1000;
                        else _legs = legs;                    
                    }

                    public int GetLegs() {
                        int result = 1;
                        for(int i = 0; i <= _legs; i++;) {
                            result++;
                        }

                        return result;
                    }
                }
            "));

            // Basic test case, there is still one path, therefore, the complexity is 1
            _treeDictionary.Add("2-CCTree1", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(int legs, string name){
                        _legs = legs;
                        _canFly = name?.Equals(""Bird"");                        
                    }
                }
            "));

            _treeDictionary.Add("3-CCTree1", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;


                    public Animal(int legs, string name){
                        _legs = legs;
                         try
                        {
                                int.Parse(""123123O"");
                        }
                        catch (ArrayTypeMismatchException e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }                        
                   }
               }
            "));

            _treeDictionary.Add("2-CCTree3", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;

                    public Animal(int legs){
                        _legs = (legs % 2 == 1) ? legs + 1 : _legs = legs;                    
                    }
                }

            "));

            _treeDictionary.Add("3-CCTree3", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;

                    public Animal(int legs){
                        _legs = (legs % 2 == 1) ? legs + 1 : ( legs < 0 ?  _legs = 0 : _legs = legs);                    
                    }
                }

            "));
        }

        [DataTestMethod]
        [DataRow("1-CCTree0")]
        [DataRow("1-CCTree1")]
        [DataRow("2-CCTree3")]
        [DataRow("2-CCTree2")]
        [DataRow("2-CCTree1")]
        [DataRow("3-CCTree0")]
        [DataRow("3-CCTree1")]
        [DataRow("3-CCTree3")]
        [DataRow("4-CCTree0")]
        [DataRow("5-CCTree0")]
        public void CcTester(string treeKey)
        {
            SyntaxTree tree = _treeDictionary[treeKey];

            int result = int.Parse(treeKey.Split('-')[0]);

            var classDeclaration = (ClassDeclarationSyntax)tree.GetRoot().ChildNodes().Single();

            int cc = CyclomaticComplexity.GetCount(classDeclaration);

            Assert.IsTrue(cc == result);
        }
    }
}