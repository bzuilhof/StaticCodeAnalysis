﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace CodeQualityAnalyzer.CodeMetrics
{
    public class MetricRunner
    {
        private readonly string _solutionLocation;

        public MetricRunner(string solutionLocation)
        {
            this._solutionLocation = solutionLocation;
            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            var instance = visualStudioInstances[1];
            Console.WriteLine($"Using MSBuild at '{instance.MSBuildPath}' to load projects.");

            // NOTE: Be sure to register an instance with the MSBuildLocator 
            //       before calling MSBuildWorkspace.Create()
            //       otherwise, MSBuildWorkspace won't MEF compose.
            MSBuildLocator.RegisterInstance(instance);
        }

        public async Task<SolutionVersionWithMetrics> GetSolutionVersionWithMetrics()
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                workspace.WorkspaceFailed += (o, e) => Console.WriteLine(e.Diagnostic.Message);
                var solution = await workspace.OpenSolutionAsync(_solutionLocation, new ConsoleProgressReporter());

                var projects = solution.Projects.Where(x => x.FilePath.EndsWith(".csproj") && !x.Name.Contains("Tests"));

//                Project project = projects.First();
                SolutionVersionWithMetrics solutionVersionWithMetrics = new SolutionVersionWithMetrics();
                foreach (Project project in projects)
                {
                    Compilation comp = project.GetCompilationAsync().Result;
                    List<SyntaxTree> syntaxTrees = comp.SyntaxTrees.ToList();

                    Dictionary<INamedTypeSymbol, int> classExtensions = NumberOfChildren.GetClassExtensions(syntaxTrees, comp);
                    Dictionary<INamedTypeSymbol, int> classCouplings = CouplingBetweenObjects.CalculateCouplings(syntaxTrees, comp);

                    foreach (var syntaxTree in syntaxTrees)
                    {
                        string location = syntaxTree.FilePath;
                        if(location.ToLower().Contains("test")) continue;
                        SemanticModel semanticModel = comp.GetSemanticModel(syntaxTree);
                        List<ClassDeclarationSyntax> classes = GetClassesFromRoot(syntaxTree.GetRoot());
                        string relativePath = location.Replace(@"\", "/");
                        var pathArr = relativePath.Split('/');
                        string fileAndParent = $@"{pathArr[pathArr.Length-2]}/{pathArr[pathArr.Length-1]}";

                        foreach (ClassDeclarationSyntax classDecl in classes)
                        {
                            string className = classDecl.Identifier.ValueText;
                            string classNameWithHash = $"{className}-{fileAndParent.GetHashCode().ToString()}";

                            if (solutionVersionWithMetrics.ClassProcessed(classNameWithHash)) continue;

                            ClassWithMetrics classMetricResults = GetMetricResults(classDecl, semanticModel, className, location, classExtensions, classCouplings);

                            if(className == "LocalizableStrings")
                                Console.WriteLine();

                            solutionVersionWithMetrics.AddClassWithMetrics(
                                classNameWithHash
                                , classMetricResults);
                        }
                    }

                }
                return solutionVersionWithMetrics;
            }
        }


        private static ClassWithMetrics GetMetricResults(ClassDeclarationSyntax classDecl, SemanticModel semanticModel, string className, string location, Dictionary<INamedTypeSymbol, int> classExtensions, Dictionary<INamedTypeSymbol, int> classCouplings)
        {
            ClassWithMetrics classMetricResults = new ClassWithMetrics(className, location);

            var lambdaMetrics = LambdaMetrics.GetValueList(classDecl, semanticModel);
            classMetricResults.AddMetric(Measure.LambdaCount, lambdaMetrics.LambdaCount);
            classMetricResults.AddMetric(Measure.LambdaFieldVariableUsageCount, lambdaMetrics.FieldVariableUsageCount);
            classMetricResults.AddMetric(Measure.LambdaLocalVariableUsageCount, lambdaMetrics.LocalVariableUsageCount);
            classMetricResults.AddMetric(Measure.LambdaSideEffectCount, lambdaMetrics.SideEffects);
            

            int sourceLinesOfCode = SourceLinesOfCode.GetCount(classDecl);
            classMetricResults.AddMetric(Measure.SourceLinesOfCode, sourceLinesOfCode);

            int commentDensity = CommentDensity.GetCount(classDecl, sourceLinesOfCode);
            
            classMetricResults.AddMetric(Measure.CommentDensity, commentDensity);

            int cyclomaticComplexity = CyclomaticComplexity.GetCount(classDecl);
            classMetricResults.AddMetric(Measure.CyclomaticComplexity, cyclomaticComplexity);

            int weightedMethodsPerClass = WeightedMethodsPerClass.GetCount(classDecl);
            classMetricResults.AddMetric(Measure.WeightedMethodsPerClass, weightedMethodsPerClass);

            int depthOfInheritanceTree = DepthOfInheritanceTree.GetCount(classDecl, semanticModel);
            classMetricResults.AddMetric(Measure.DepthOfInheritanceTree, depthOfInheritanceTree);

            int numberOfChildren = NumberOfChildren.GetCount(classDecl, semanticModel, classExtensions);
            classMetricResults.AddMetric(Measure.NumberOfChildren, numberOfChildren);

            int couplingBetweenObjects = CouplingBetweenObjects.GetCount(classDecl, semanticModel, classCouplings);
            classMetricResults.AddMetric(Measure.CouplingBetweenObjects, couplingBetweenObjects);

            int responseForAClass = ResponseForAClass.GetCount(classDecl);
            classMetricResults.AddMetric(Measure.ResponseForAClass, responseForAClass);

            int lackOfCohesionOfMethods = LackOfCohesionOfMethods.GetCount(classDecl, semanticModel);
            classMetricResults.AddMetric(Measure.LackOfCohesionOfMethods, lackOfCohesionOfMethods);

            int sourceLinesOfLambda = SourceLinesOfLambda.GetCount(classDecl);
            classMetricResults.AddMetric(Measure.SourceLinesOfLambda, sourceLinesOfLambda);

            int lambdaScore = (int)((double)sourceLinesOfLambda / sourceLinesOfCode * 100);
            classMetricResults.AddMetric(Measure.LambdaScore, lambdaScore);

            int unterminatedCollections = UnterminatedCollections.GetCount(classDecl, semanticModel);
            classMetricResults.AddMetric(Measure.UnterminatedCollections, unterminatedCollections);

            return classMetricResults;
        }

        public static List<ClassDeclarationSyntax> GetClassesFromRoot(SyntaxNode rootNode)
        {
            return rootNode
                    .ChildNodes()
                    .Where(x => x is NamespaceDeclarationSyntax)
                    .SelectMany(x => x.ChildNodes())
                    .OfType<ClassDeclarationSyntax>()
                    .ToList()
                ;
        }

        private class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
        {
            public void Report(ProjectLoadProgress loadProgress)
            {
                var projectDisplay = Path.GetFileName(loadProgress.FilePath);
                if (loadProgress.TargetFramework != null)
                {
                    projectDisplay += $" ({loadProgress.TargetFramework})";
                }

                Console.WriteLine($"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\\:ss\\.fffffff} {projectDisplay}");
            }
        }
    }

    public class SolutionVersionWithMetrics
    {
        public readonly Dictionary<string, ClassWithMetrics> ClassesWithMetrics;

        public SolutionVersionWithMetrics()
        {
            ClassesWithMetrics = new Dictionary<string, ClassWithMetrics>();
        }

        public void AddClassWithMetrics(string classNameWithHash, ClassWithMetrics classWithMetrics)
        {
            ClassesWithMetrics.Add(classNameWithHash, classWithMetrics);
        }

        public bool ClassProcessed(string classNameWithHash)
        {
            return ClassesWithMetrics.ContainsKey(classNameWithHash);
        }
    }



    public class ClassWithMetrics
    {
        public string ClassName { get; set; }
        public string ClassPath { get; set; }

        public Dictionary<Measure, int> MetricResult { get; }

        public ClassWithMetrics(string className, string classPath)
        {
            ClassName = className;
            ClassPath = classPath;
            MetricResult = new Dictionary<Measure, int>();
        }

        public void AddMetric(Measure measure, int value)
        {
            MetricResult.Add(measure, value);
        }
    }

    public enum Measure
    {
        CyclomaticComplexity,
        SourceLinesOfCode,
        CommentDensity,

        WeightedMethodsPerClass,
        DepthOfInheritanceTree,
        NumberOfChildren,
        CouplingBetweenObjects,
        ResponseForAClass,
        LackOfCohesionOfMethods,

        SourceLinesOfLambda,
        LambdaCount,
        LambdaScore,
        LambdaFieldVariableUsageCount,
        LambdaLocalVariableUsageCount,
        LambdaSideEffectCount,

        UnterminatedCollections,
    }
}
