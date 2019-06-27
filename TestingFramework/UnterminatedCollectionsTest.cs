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
    public class UnterminatedCollectionsTest
    {
        private static Dictionary<string, SyntaxTree> _treeDictionary;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _treeDictionary = new Dictionary<string, SyntaxTree>();
            // Basic test case, there is only one path, therefore, the complexity is 1

            // Basic test case, there is still one path, therefore, the complexity is 1
            _treeDictionary.Add("1-UTCTree0", CSharpSyntaxTree.ParseText(@"
                namespace Something
                {
                    using System;
                    using System.Collections.Generic;
                    using System.Linq;
                    class Animal
                    {
                        private int _legs;
                        private bool _canSwim;
                        private bool _canFly;
                        
                        public Animal(){
                            IEnumerable<int> bla = Enumerable.Range(1, 10)
                                .Where(i => i % 2 == 0) //filter
                            ;
                        }
                    }
                }    
            "));

            _treeDictionary.Add("0-UTCTree2", CSharpSyntaxTree.ParseText(@"
                class Animal
                {
                    private int _legs;
                    private bool _canSwim;
                    private bool _canFly;
                    
                    public Animal(){
                        var x = 5;
                    }
                }
            "));

            _treeDictionary.Add("1-UTCTree1", CSharpSyntaxTree.ParseText(@"
                namespace Something
                {
                    using System;
                    using System.Collections.Generic;
                    using System.Linq;
                    class Animal
                    {
                        private int _legs;
                        private bool _canSwim;
                        private bool _canFly;
                        
                        public Animal(){
                            var bla = Enumerable.Range(1, 10)
                                .Where(i => i % 2 == 0) //filter
                            ;
                        }
                    }
                }
            "));
        }

        [DataTestMethod]
        [DataRow("1-UTCTree0")]
        [DataRow("1-UTCTree0")]
        [DataRow("0-UTCTree2")]
        public void UtcTester(string treeKey)
        {
            SyntaxTree tree = _treeDictionary[treeKey];

            int result = int.Parse(treeKey.Split('-')[0]);

            CSharpCompilation compilation = CSharpCompilation.Create("Trivial")
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location))
                .AddSyntaxTrees(tree);

            SemanticModel semanticModel = compilation.GetSemanticModel(tree);
            var classDeclaration = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
            var problems = compilation.GetDiagnostics();
            int utc = UnterminatedCollections.GetCount(classDeclaration, semanticModel);

            Assert.IsTrue(utc == result);
        }
    }
}