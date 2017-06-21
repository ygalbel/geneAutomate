using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Models;

namespace GeneAutomate.Parser
{
    public class FileParser
    {
        public FileParsingResult ParseFiles(string netPath, string specPath)
        {
            var links = GeneLinks(netPath);

            var conditionAndExperiments = new RuleFileParser().ParseRules(specPath, links.GetAllNodes());

            return new FileParsingResult() {GeneLinks = links, Conditions = conditionAndExperiments.Conditions, Experiments = conditionAndExperiments.Experiments};
        }

        private List<GeneLink> GeneLinks(string netPath)
        {
            var lines = File.ReadAllLines(netPath).ToList();

            lines = lines.SkipWhile(a => a.StartsWith("directive")).ToList();

            lines = lines.Skip(1).ToList(); // skip rules line

            var links = lines.Select(l => CreateLinkFromLine(l)).ToList();
            return links;
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
        public static List<string> GetAllNodes(this List<GeneLink>  t)
        {
            return t.Select(a => a.From).Union(t.Select(a => a.To)).Distinct().ToList();
        }
    }
}
