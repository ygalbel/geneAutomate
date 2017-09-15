using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Models;

namespace GeneAutomate.Writer
{
    public class RuleFileWriter
    {
        public void WriteFile(string prefix, List<GeneLink> links, List<GeneNode> nodes)
        {
            StringBuilder builder = new StringBuilder();

            StringBuilder predicatesBuilder = new StringBuilder();
            StringBuilder observationBuilder = new StringBuilder();

            nodes.ForEach(n => WriteNode(n, predicatesBuilder, observationBuilder));
        }

        private void WriteNode(GeneNode geneNode, StringBuilder predicatesBuilder, StringBuilder observationBuilder)
        {
            //return CreateSpecString(geneNode, predicatesBuilder, observationBuilder);
        }

        public string CreateSpecString(GeneNode geneNode)
        {
            StringBuilder predicatesBuilder = new StringBuilder();
            StringBuilder observationBuilder = new StringBuilder();
            var currentCondition = 0;
            HashSet<GeneNode> nodesVisited = new HashSet<GeneNode>();
            geneNode.Visit(v =>
            {
                if (!nodesVisited.Contains(v) && v != null && v.CurrentCondition != null && v.CurrentCondition.Any())
                {
                    //$Conditions1 := { S1 = 0 and S2 = 1};

                    var conditionName = $"$Condition{currentCondition}";
                    observationBuilder.AppendLine(
                        $"{conditionName} := {JoinCondition(v.CurrentCondition)}");


                    predicatesBuilder.AppendLine($"{CreateName(v.NodeName)} |= {conditionName}");
                    currentCondition++;

                    nodesVisited.Add(v);
                }
            });

            var builder = new StringBuilder();
            builder.AppendLine(observationBuilder.ToString());
            builder.AppendLine();
            builder.AppendLine("// Observation");
            builder.AppendLine(predicatesBuilder.ToString());
            return builder.ToString();
        }

        private string CreateName(string objNodeName)
        {
            var name = objNodeName.Split('_')[0];
            var time = objNodeName.Split('_')[1];

            return $"#{name}[{time}]";
        }

        private string JoinCondition(Condition condition)
        {
            StringBuilder builder = new StringBuilder();
            condition.ToList().ForEach(f =>
            {
                if (f.Value.HasValue)
                {
                    builder.Append($"{f.Key} = {f.Value.Value}");

                    if (f.Key != condition.Last().Key)
                    {
                        builder.Append(" and");
                    }
                }
            });

            return builder.ToString();
        }
    }
}
