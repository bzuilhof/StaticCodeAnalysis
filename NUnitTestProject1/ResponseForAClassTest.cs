using System.Collections.Generic;
using System.Linq;
using CodeQualityAnalyzer.CodeMetrics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace TestFramework
{
    public class ResponseForAClassTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;

        [SetUp]
        public void Setup()
        {
            _treeDictionary = new Dictionary<string, SyntaxTree>();

            _treeDictionary.Add("2-RFCTree1", CSharpSyntaxTree.ParseText(@"
                namespace Something
                {
                    class Animal
                    {
                        private int _legs;
                        private bool _canSwim;
                        private bool _canFly;
                        private Guy owner;
                        public Animal(){
                            owner = new Guy();
                        }
                    }
                    
                    class Guy
                    {
                        public int teeth;
                    }
                }
            "));

            _treeDictionary.Add("4-RFCTree1", CSharpSyntaxTree.ParseText(@"
                namespace Something
                {
                    class Animal
                    {
                        private int _legs;
                        private bool _canSwim;
                        private bool _canFly;
                        private Guy owner;
                        public Animal(){
                            Helper.WriteSomething();

                        }

                        public static void WriteSomething()
                        {
                            Console.WriteLine(""Something"");
                        }
                    }
                }
            "));

        }

        [TestCase("2-RFCTree1")]
        [TestCase("4-RFCTree1")]
        public void RfcTester(string treeKey)
        {
            SyntaxTree tree = _treeDictionary[treeKey];

            int result = int.Parse(treeKey.Split("-")[0]);
            
            IEnumerable<ClassDeclarationSyntax> classDeclarations =  MetricRunner.GetClassesFromRoot(tree.GetRoot());
             
            int rfc = ResponseForAClass.GetCount(classDeclarations.First());

            Assert.IsTrue(rfc == result);
        }
    }
}