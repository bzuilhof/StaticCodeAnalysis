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
    public class SourceLinesOfCodeTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;
        

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _treeDictionary = new Dictionary<string, SyntaxTree>();
            // Basic test case
            _treeDictionary.Add("6-SlocTree0", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                }
            "));

            // Test case with blank lines
            _treeDictionary.Add("6-SlocTree1", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;



                }
            "));

            // Test case with commented attribute with single line comment
            _treeDictionary.Add("6-SlocTree2", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    // private int teeth;
                }
            "));

            // Test case with commented attribute with multi line comment
            _treeDictionary.Add("6-SlocTree3", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    /** private int teeth; */
                }
            "));

            // Test case with commented attributes with multi line comment
            _treeDictionary.Add("6-SlocTree4", CSharpSyntaxTree.ParseText(@"
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
            _treeDictionary.Add("7-SlocTree5", CSharpSyntaxTree.ParseText(@"
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

        [DataTestMethod]
        [DataRow("6-SlocTree0")]
        [DataRow("6-SlocTree1")]
        [DataRow("6-SlocTree2")]
        [DataRow("6-SlocTree3")]
        [DataRow("6-SlocTree4")]
        [DataRow("7-SlocTree5")]
        public void SlocTester(string treeKey)
        {
            SyntaxTree tree = _treeDictionary[treeKey];

            int result = int.Parse(treeKey.Split('-')[0]);

            var classDeclaration = (ClassDeclarationSyntax)tree.GetRoot().ChildNodes().Single();

            Assert.IsTrue(SourceLinesOfCode.GetCount(classDeclaration) == result);
        }

        
    }
}