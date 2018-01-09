using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.BusinessLogic;
using GeneAutomate.Models;
using GeneAutomate.Writer;
using Newtonsoft.Json;
using NLog;

namespace GeneAutomate.Parser
{
    public class FileParser
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public FileParsingResult ParseFiles(string netPath, string specPath)
        {
            ParseRuleResponse conditionAndExperiments;
            var links = GetConditionAndExperiments(netPath, specPath, out conditionAndExperiments);

            var automates =
               conditionAndExperiments.Experiments.ToDictionary(s => s.Key,
                   s => new AutomataFromExperimentCreator().CreateAutomata(s.Value));

            var res = new List<GeneNode>();
            var availableNodes = new Stack<GeneNode>(automates.Select(a => a.Value).ToList());
            var backTrackingNode = AutomataMergeLogic.CreateBackTrackingNodeFromStack(availableNodes, 0);

            // find merges
            var automataMergeLogic = new AutomataMergeLogic();

            automataMergeLogic
                .GetFinalMerges(availableNodes, links, res, backTrackingNode);

            // handle not merged experiments
            availableNodes.Where(d => NotExistInMerges(d, res)).ToList().ForEach(f =>
            {
                f = automataMergeLogic.ApplyAllPossibleLoops(f, links);
                res.Add(f);
            });

            var merges = res
             .Take(100)
             .Select(a => a.ToFullViewAutomata(automates))
             .ToList();

            logger.Info($"Finish merges found {merges.Count} valid merges");


            /*   var allMerges = new AutomataMergeLogic()
                   .GetMerges(automates.Select(a => a.Value).ToList())
                   .Take(100)
                   .Select(a => a.ToViewAutomata())
                   .ToList();
                   */

            var automatesView = automates
                    .ToDictionary(a => a.Key, a => a.Value.ToViewAutomata());

            var backTrackingAutomata = backTrackingNode?.ToViewAutomata();

            logger.Info("BackTracing");
            logger.Info(JsonConvert.SerializeObject(backTrackingAutomata, Formatting.Indented));
            return new FileParsingResult()
            {
                GeneLinks = links,
                Conditions = conditionAndExperiments.Conditions,
                Experiments = conditionAndExperiments.Experiments,
                Automates = automatesView,
                Merges = merges,
                BackTrackingNode = backTrackingAutomata,
                MergeObjects = res
                //AllMerges = allMerges
            };
        }

        // TODO: handle "cutted" merged
        private bool NotExistInMerges(GeneNode geneNode, List<GeneNode> res)
        {
            return !res.Any(m => m.GetAllMergedExperiment().Contains(geneNode.GetExperimentName()));
        }

        public List<GeneLink> GetConditionAndExperiments(string netPath, string specPath,
            out ParseRuleResponse conditionAndExperiments)
        {
            var links = GeneLinks(netPath);

            var function = GetFunctions(netPath);

            function = MergeFunctions(links, function);

            conditionAndExperiments = new RuleFileParser()
                .ParseRules(specPath, links.GetAllNodes());

            conditionAndExperiments.Functions = function;
            return links;
        }

        private Dictionary<string, List<int>> MergeFunctions(
            List<GeneLink> links, 
            Dictionary<string, List<int>> function)
        {
            Dictionary<string, List<int>> @new = new Dictionary<string, List<int>>();
            function.ToList().ForEach(f =>
            {
                var froms = links.Where(a => a.To == f.Key).ToList();
                var positives = froms.Positives().Any();
                var negative = froms.Negatives().Any();

                @new[f.Key] = f.Value.Select(s => GetRealValue(s, positives, negative))
                    .Distinct().ToList();
            });

            return @new;

        }

        private int GetRealValue(int i, bool positives, bool negative)
        {
            // A
            if (!positives && !negative)
            {
                if (i < 9)
                {
                    return 0;
                }
                else
                {
                    return 9;
                }
            }

            // B case
            if (!negative)
            {
                if (i == 0 || i == 2 || i == 4)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }

            // C Case
            if (!positives)
            {
                if (i == 12 || i == 15 || i == 17)
                {
                    return 12;
                }
                else
                {
                    return 0;
                }
            }

            // D Case
            return i;
        }

        private List<GeneLink> GeneLinks(string netPath)
        {
            var lines = File.ReadAllLines(netPath).ToList();
            lines = lines.SkipWhile(a => a.StartsWith("directive")).ToList();
            lines = lines.Skip(1).ToList(); // skip rules line
            var links = lines.Select(l => CreateLinkFromLine(l)).ToList();
            return links;
        }

        private Dictionary<string, List<int>> GetFunctions(string netPath)
        {
            var lines = File.ReadAllLines(netPath).ToList();

            var functionLine = lines[5];

            return FunctionParserLogic(functionLine);
        }

        public static Dictionary<string, List<int>> FunctionParserLogic(string functionLine)
        {
            Dictionary<string, List<int>> res = new Dictionary<string, List<int>>();
            //C[](1,3,5); B[](0..8); A[](0..8); S2[](0); S1[](0); 
            var singles = functionLine.Split(';');
            foreach (var s in singles.ToList())
            {
                var spliltted = s.Split('['); //'(')[0].Split('')

                if (spliltted.Length == 1)
                {
                    continue; // but should be last
                }

                var letter = spliltted[0].Trim();
                var numbers = spliltted[1].Split('(')[1].Split(')')[0];
                var currentList = new List<int>();
                foreach (var n in numbers.Split(',').ToList())
                {
                    if (n.Contains(".."))
                    {
                        var from = n.Split('.')[0];
                        var to = n.Split('.').Last();

                        for (var i = Int32.Parse(@from); i <= Int32.Parse(to); i++)
                        {
                            currentList.Add(i);
                        }
                    }
                    else
                    {
                        currentList.Add(Int32.Parse(n));
                    }
                }

                res.Add(letter, currentList);
            }

            return res;
        }

        private GeneLink CreateLinkFromLine(string s)
        {
            var data = s.Split('\t');

            var result = new GeneLink();
            result.From = data[0];
            result.To = data[1];
            result.IsPositive = data[2].StartsWith("positive");

            if (data.Length > 3)
            {
                result.IsOptional = data[3].StartsWith("optional");
            }

            return result;
        }
    }

    public static class GeneLinkHelper
    {
        public static List<string> GetAllNodes(this List<GeneLink> t)
        {
            return t.Select(a => a.From).Union(t.Select(a => a.To)).Distinct().ToList();
        }


    }

    public static class ListGeneNodeExtensions
    {
        public static List<GeneLink> Positives(this List<GeneLink> links)
        {
            return links.Where(a => a.IsPositive).ToList();
        }

        public static List<GeneLink> Negatives(this List<GeneLink> links)
        {
            return links.Where(a => !a.IsPositive).ToList();
        }
    }
}
