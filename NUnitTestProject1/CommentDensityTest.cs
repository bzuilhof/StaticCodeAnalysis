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
    public class CommentDensityTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;
        

        [SetUp]
        public void Setup()
        {
            _treeDictionary = new Dictionary<string, SyntaxTree>();
            // Basic test case
            _treeDictionary.Add("0-CDTree0", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                }
            "));

           

            // Test case with commented attribute with single line comment
            _treeDictionary.Add("14-CDTree2", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    // private int teeth;
                }
            "));

            // Test case with commented attribute with multi line comment
            _treeDictionary.Add("1-CDTree3", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    /** private int teeth; */
                }
            "));

            // Test case with commented attributes with multi line comment
            _treeDictionary.Add("2-CDTree4", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    /** 

                    private int teeth; */
                }
            "));
            // Test case with commented attributes with multi line comment
            _treeDictionary.Add("3-CDTree1", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    /** 
                    something
                    private int teeth; */
                }
            "));

            // Test case with commented attributes with multi line comment
            _treeDictionary.Add("2-CDTree5", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    /** 

                    private int teeth; */ private bool tooth;
                }
            "));
        }


        [TestCase("0-CDTree0")]
        [TestCase("14-CDTree2")]
//        [TestCase("0-CDTree1")]
//        [TestCase("3-CDTree1")]
//        [TestCase("1-CDTree2")]
//        [TestCase("1-CDTree3")]
//        [TestCase("2-CDTree4")]
//        [TestCase("2-CDTree5")]
        public void CdTester(string treeKey)
        {
            SyntaxTree tree = _treeDictionary[treeKey];

            int result = int.Parse(treeKey.Split("-")[0]);

            var classDeclaration = (ClassDeclarationSyntax)tree.GetRoot().ChildNodes().Single();
            int sloc = SourceLinesOfCode.GetCount(classDeclaration);
            int cd = CommentDensity.GetCount(classDeclaration, sloc);

            Assert.IsTrue(cd == result);
        }
    }
}