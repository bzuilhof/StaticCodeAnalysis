using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeQualityAnalyzer.CodeMetrics;
using CsvHelper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Threading;


namespace CodeQualityAnalyzer
{
    class Program
    {
        private const string RepositoriesFolder = @"C:\Users\BartZ\source\repos";
        private static ShellService _shellService;
        private static string _repository;
        static void Main(string[] args)
        {
            Console.WriteLine("Repository Analyzer");
            Console.WriteLine("Select target project");

            TargetProject[] targetProjects = TargetProject.GetTargetProjects();
            for(int i = 0; i < targetProjects.Length; i++)
            {
                Console.WriteLine($@"[{(char)('A' + i) }]: {targetProjects[i].OrganizationName}/{targetProjects[i].RepositoryName}");
            }

            Console.Write("Please enter project #: ");
            var input = Console.ReadKey().KeyChar;
            int choice = char.ToUpper(input) - 65;
            Console.WriteLine();

            TargetProject targetProject = targetProjects[choice];

            IIssueTrackerService issueTrackerService;
            if (targetProject.RepositoryName == "knowNow")
            {
                issueTrackerService = new TfsService($@"{RepositoriesFolder}/{targetProject.RepositoryName}"); ;
            }
            else
            {
                issueTrackerService = new GithubService(targetProject);

            }

            string solutionFile = $@"{RepositoriesFolder}/{targetProject.RepositoryName}/{targetProject.SolutionFileLocation}";

            _repository = targetProject.RepositoryName;
            _shellService = new ShellService($@"{RepositoriesFolder}/{_repository}");
            _shellService.CheckoutCommit();
            var hash = _shellService.GetHeadHash();
            RepositoryWithMetrics repositoryWithMetrics = new RepositoryWithMetrics();
            MetricRunner runner = new MetricRunner(solutionFile);
            SolutionVersionWithMetrics solutionVersionWithMetrics = runner.GetSolutionVersionWithMetrics().Result;
   
            repositoryWithMetrics.AddVersion("HEAD", solutionVersionWithMetrics);

            List<FaultyVersion> faultyVersions = issueTrackerService.GetFaultyVersions();
            Dictionary<string, int> bugAmountInClasses = GetBugAmountInClasses(faultyVersions);

            List<OutputRow> output = GetOutput(solutionVersionWithMetrics.ClassesWithMetrics, bugAmountInClasses);

            OutputCsv(output, $@"C:\Users\BartZ\code-analysis-results\{targetProject.OrganizationName}_{_repository}_{hash}.csv");
           
            Console.ReadKey();
        }

        private static List<OutputRow> GetOutput(Dictionary<string, ClassWithMetrics> classesWithMetrics, Dictionary<string, int> faultyClasses)
        {
            Dictionary<string, OutputRow> dictionaryResultsOutput = new Dictionary<string, OutputRow>();
            foreach (KeyValuePair<string, ClassWithMetrics> classWithMetrics in classesWithMetrics)
            {
                dictionaryResultsOutput.Add(classWithMetrics.Key, new OutputRow(classWithMetrics.Value.ClassName, classWithMetrics.Value.MetricResult));
            }

            foreach (var bugClass in faultyClasses.Keys)
            {
                if (dictionaryResultsOutput.ContainsKey(bugClass))
                {
                    dictionaryResultsOutput[bugClass].SetFaulty();
                }
                else
                {
                    Console.WriteLine($"Bugclass {bugClass} doesn't exist in HEAD");
                }
            }

            return dictionaryResultsOutput.Values.ToList();
        }

        private static void OutputCsv(List<OutputRow> rows, string path)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteHeader<OutputRow>();
                csv.NextRecord();
                csv.WriteRecords(rows);
            }
        }

        private static Dictionary<string, int> GetBugAmountInClasses(List<FaultyVersion> faultyVersions)
        {
            Dictionary<string, int> bugAmountInClasses = new Dictionary<string, int>();
            foreach (var faultyVersion in faultyVersions)
            {
                foreach (FaultyFile faultyFile in faultyVersion.FaultyFiles)
                {
                    // This means the first line of the file has been inserted. And its the only change. Which means its a new file. Therefore, skip file.
                    if (faultyFile.AffectedLines[0] == -1 && faultyFile.AffectedLines.Count == 1) continue;
                    string absoluteFileLocation = $"{RepositoriesFolder}/{_repository}/{faultyFile.Filename}";
                    var response = _shellService.CheckoutFileForCommit(faultyFile.Filename, faultyVersion.Hash);
                    Thread.Sleep(100);
                    response = _shellService.CheckoutFileForCommit(faultyFile.Filename, faultyVersion.Hash);
                    string code;
                    using (StreamReader r = new StreamReader(absoluteFileLocation))
                    {
                        code = r.ReadToEnd();
                    }


                    SyntaxNode treeRoot = CSharpSyntaxTree.ParseText(code).GetRoot();
                    HashSet<string> affectedClasses = GithubService.GetAffectedClasses(treeRoot, faultyFile.AffectedLines, absoluteFileLocation);

                    foreach (string affectedClass in affectedClasses)
                    {
                        if (bugAmountInClasses.ContainsKey(affectedClass))
                        {
                            bugAmountInClasses[affectedClass]++;
                        }
                        else
                        {
                            bugAmountInClasses.Add(affectedClass, 1);
                        }
                    }
                }
            }

            return bugAmountInClasses;
        }

        

    }

    class RepositoryWithMetrics
    {
        private Dictionary<string, SolutionVersionWithMetrics> _projectVersionsWithMetrics;

        public RepositoryWithMetrics()
        {
            _projectVersionsWithMetrics = new Dictionary<string, SolutionVersionWithMetrics>();
        }

        public void AddVersion(string version, SolutionVersionWithMetrics metrics)
        {
            _projectVersionsWithMetrics.Add(version, metrics);
        }
    }
}
