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
        public string CreateSpecString(GeneNode geneNode)
        {
           var predicatesBuilder = new StringBuilder();
           var observationBuilder = new StringBuilder();

            int num = 0;
            var observationNames = new HashSet<string>();

            CreateSpecString(geneNode, predicatesBuilder, observationBuilder,  observationNames, ref num);

            var builder = new StringBuilder();
            builder.AppendLine(observationBuilder.ToString());
            builder.AppendLine();
            builder.AppendLine("// Observation");
            builder.AppendLine(predicatesBuilder.ToString());
            return builder.ToString();
        }

        public string CreateSpecString(List<GeneNode> geneNodes)
        {
            var predicatesBuilder = new StringBuilder();
            var observationBuilder = new StringBuilder();

            int num = 0;
            var alreadyTakenName = new HashSet<string>();
            for (int index = 0; index < geneNodes.Count; index++)
            {
                var geneNode = geneNodes[index];
                CreateSpecString(geneNode, predicatesBuilder, observationBuilder,alreadyTakenName, ref num);

                // last one
                if (index == geneNodes.Count - 1)
                {
                    predicatesBuilder.AppendLine(";");
                }
                else
                {
                    predicatesBuilder.AppendLine("and");
                }
            }

            var builder = new StringBuilder();
            builder.AppendLine(observationBuilder.ToString());
            builder.AppendLine();
            builder.AppendLine("// Observation");
            builder.AppendLine(predicatesBuilder.ToString());
            return builder.ToString();
        }

        public void CreateSpecString(GeneNode geneNode, StringBuilder predicatesBuilder, StringBuilder observationBuilder, HashSet<string> alreadyTakenNames, ref int currentCondition)
        {
             
            HashSet<GeneNode> nodesVisited = new HashSet<GeneNode>();
            var z = currentCondition;
            string OneName = "";
            geneNode.Visit(v =>
            {
                if (!nodesVisited.Contains(v) && v != null && v.CurrentCondition != null && v.CurrentCondition.Any())
                {
                    //$Conditions1 := { S1 = 0 and S2 = 1};

                    var conditionName = $"$Condition{z}";
                    observationBuilder.AppendLine(
                        $"{conditionName} := {JoinCondition(v.CurrentCondition)}");

                    var connector = (v.Transitions == null || !v.Transitions.Any()) ? "" : " and ";

                    var name = CreateName(v.NodeName, alreadyTakenNames, out OneName);



                    predicatesBuilder.AppendLine($"{name} |= {conditionName} {connector}");
                    z++;

                    nodesVisited.Add(v);
                }

            });

            currentCondition = z;
            alreadyTakenNames.Add(OneName);
        }

        private string CreateName(string objNodeName, HashSet<string> alreadyTakenNames, out string onlyName)
        {
            var name = objNodeName.Split('_')[0];

            while (alreadyTakenNames.Contains(name))
            {
                name = name + '0';
            }

            onlyName = name;

            var time = objNodeName.Split('_')[1].Split('^', '~')[0].Trim();

            return $"#{name}[{time}]";
        }

        private string JoinCondition(Condition condition)
        {
            StringBuilder builder = new StringBuilder();

            if (condition.Any(d => d.Value.HasValue))
            {
                builder.Append("{ ");
            }

            condition.ToList().ForEach(f =>
            {

                if (f.Value.HasValue)
                {
                    var val = f.Value.Value ? "1" : "0";
                    builder.Append($"{f.Key} = {val}");

                    if (f.Key != condition.Last().Key)
                    {
                        builder.Append(" and ");
                    }
                }
            });


            if (condition.Any(d => d.Value.HasValue))
            {
                builder.Append(" };");
            }
            return builder.ToString();
        }
    }
}
