using System.Collections.Generic;
using CodeQualityAnalyzer.CodeMetrics;

namespace CodeQualityAnalyzer
{
    class OutputRow
    {
        public string ClassName { get; set; }
        public int Faulty { get; set; } = 0;

        public int CyclomaticComplexity { get; set; }
        public int SourceLinesOfCode { get; set; }
        public int CommentDensity { get; set; }

        public int WeightedMethodsPerClass { get; set; }
        public int DepthOfInheritance { get; set; }
        public int NumberOfChildren { get; set; }
        public int CouplingBetweenObjects { get; set; }
        public int ResponseForAClass { get; set; }
        public int LackOfCohesionOfMethods { get; set; }

        public int LambdaScore { get; set; }
        public int LambdaCount { get; set; }
        public int SourceLinesOfLambda { get; set; }
        public int LambdaUsed { get; set; }
        public int LambdaFieldVariableUsageCount { get; set; }
        public int LambdaFieldVariableUsed { get; set; }
        public int LambdaLocalVariableUsageCount { get; set; }
        public int LambdaLocalVariableUsed { get; set; }
        public int UnterminatedCollections { get; set; }
        public int LambdaSideEffectCount { get; set; }

        public OutputRow(string className, IReadOnlyDictionary<Measure, int> metricResults)
        {
            ClassName = className;

            CyclomaticComplexity = metricResults[Measure.CyclomaticComplexity];
            SourceLinesOfCode = metricResults[Measure.SourceLinesOfCode];
            CommentDensity = metricResults[Measure.CommentDensity];

            WeightedMethodsPerClass = metricResults[Measure.WeightedMethodsPerClass];
            DepthOfInheritance= metricResults[Measure.DepthOfInheritanceTree];
            NumberOfChildren = metricResults[Measure.NumberOfChildren];
            CouplingBetweenObjects = metricResults[Measure.CouplingBetweenObjects];
            ResponseForAClass = metricResults[Measure.ResponseForAClass];
            LackOfCohesionOfMethods = metricResults[Measure.LackOfCohesionOfMethods];

            SourceLinesOfLambda = metricResults[Measure.SourceLinesOfLambda];
            LambdaScore = metricResults[Measure.LambdaScore];
//            LambdaUsed = metricResults[Measure.LambdaCount].LimitToOne();
            LambdaCount = metricResults[Measure.LambdaCount]; 
//            LambdaFieldVariableUsed = metricResults[Measure.LambdaFieldVariableUsageCount].LimitToOne(); 
            LambdaFieldVariableUsageCount = metricResults[Measure.LambdaFieldVariableUsageCount];
//            LambdaLocalVariableUsed = metricResults[Measure.LambdaLocalVariableUsageCount].LimitToOne(); 
            LambdaLocalVariableUsageCount = metricResults[Measure.LambdaLocalVariableUsageCount];
            LambdaSideEffectCount = metricResults[Measure.LambdaSideEffectCount];
            UnterminatedCollections = metricResults[Measure.UnterminatedCollections]; 
        }

        public void SetFaulty() => Faulty = 1;
    }
}