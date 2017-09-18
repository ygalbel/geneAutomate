using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneAutomate.Models;

namespace GeneAutomate.BusinessLogic
{
   
    public static class AutomateViewHelper
    {
        public static AutomateObject ToViewAutomata(this List<GeneLink> automata)
        {
            return CreateViewAutomata(automata);
        }

        public static AutomateObject ToViewAutomata(this GeneNode automata)
        {
            return CreateViewAutomata(automata);
        }

        public static AutomateObject ToFullViewAutomata(this GeneNode automata, Dictionary<string,GeneNode> automateBank)
        {
            var res = new AutomateObject();
            var node = automata;

            var nodes = new List<GeneNode>();
            node.Visit((g) => nodes.Add(g));

            var trans = new List<Edge>();

            var tempViewAutomates = new AutomateObject() { edges = new List<Edge>(), nodes = new List<Node>()};

            node.Visit((g) =>
            {
                var toAdd = g.GetAllMergedExperiment()?.Where(a => a != g.GetExperimentName()) ?? Enumerable.Empty<string>();

                foreach (var automteToAdd in toAdd)
                {
                    var geneNodeToAdd = automateBank[automteToAdd];

                    if (geneNodeToAdd == null)
                    {
                        continue;
                    }

                    var lastNodeToMerge = g.GetAllMergedNodes().FirstOrDefault(a => a.StartsWith(automteToAdd));

                    var partAutomate = GetCutteddVersionOfAutomate(geneNodeToAdd, lastNodeToMerge, g.NodeName);

                    var toAddViewAutomate = partAutomate.ToViewAutomata();

                    toAddViewAutomate.edges.ForEach(e => e.color = "#FF5733");
                    tempViewAutomates.nodes.AddRange(toAddViewAutomate.nodes);
                    tempViewAutomates.edges.AddRange(toAddViewAutomate.edges);
                }


                if (g.Transitions != null && g.Transitions.Any())
                {
                    g.Transitions.ForEach(f =>
                    {
                        trans.Add(new Edge
                        {
                            source = g.NodeName,
                            target = f.Node.NodeName,
                        });

                    });
                }
            });

            res.nodes = nodes
                .Select(a => new Node()
                {
                    id = a.NodeName,
                    label = FormatNodeLabel(a),
                    size = 3
                }).ToList();

            res.edges =
                trans.Select(d => new Edge()
                {
                    id = d.source + "_" + d.target,
                    source = d.source,
                    target = d.target,
                    color = "#3300ff",
                    type = "arrow",
                    label = d.label //+ " " + CreateLabel(d)
                })
                    .ToList();



            res.nodes.AddRange(tempViewAutomates.nodes.Where(r => res.nodes.All(b => b.id != r.id)));
            res.edges.AddRange(tempViewAutomates.edges);
            return res;
        }

        private static GeneNode GetCutteddVersionOfAutomate(GeneNode geneNodeToAdd, string lastNodeToMerge, string newLastNode)
        {
            var newVersion = CloneHelper.Clone(geneNodeToAdd);
            var temp = newVersion;

            while (temp.NodeName != lastNodeToMerge)
            {
                temp = temp.Transitions.First().Node;
            }

            temp.NodeName = newLastNode;
            temp.Transitions = new List<GeneTransition>();

            return newVersion;


        }

        public static AutomateObject CreateViewAutomata(GeneNode node)
        {
            var res = new AutomateObject();

            var nodes = new List<GeneNode>();
            node.Visit((g) => nodes.Add(g));

            var trans = new List<Edge>();

            
            node.Visit((g) =>
            {
                if (g.Transitions != null && g.Transitions.Any())
                {
                    g.Transitions.ForEach(f =>
                    {
                        trans.Add(new Edge
                        {
                            source = g.NodeName,
                            target = f.Node.NodeName,
                        });

                    });
                }
            });

            res.nodes = nodes
                .Select(a => new Node() { id = a.NodeName,
                    label = FormatNodeLabel(a), size = 3 }).ToList();

            res.edges =
                trans.Select(d => new Edge()
                {
                    id = d.source + "_" + d.target,
                    source = d.source,
                    target = d.target,
                    color =  "#3300ff",
                    type = "arrow",
                    label = d.label //+ " " + CreateLabel(d)
                })
                    .ToList();

            return res;
        }

        private static string FormatNodeLabel(GeneNode a)
        {
            return a.NodeName + " " +  CreateLabel(a.CurrentCondition);
        }

        private static string CreateLabel(GeneTransition f)
        {
            var label = "";

            var fCondition = f.Condition;
            return CreateLabel(fCondition);
        }

        private static string CreateLabel(Condition fCondition)
        {
            string label;
            if (fCondition.Any())
            {
                label = string.Join(",", fCondition.Select(a => $"{a.Key}:{a.Value}"));
            }
            else
            {
                label = string.Empty;
            }

            return label;
        }

        public static AutomateObject ToViewAutomata(this BackTrackingNode automata)
        {
            return CreateViewAutomata(automata);
        }

        public static AutomateObject CreateViewAutomata(BackTrackingNode node)
        {
            var res = new AutomateObject();

            var nodes = new List<BackTrackingNode>();

            node.Visit((g) => nodes.Add(g));

            var trans = new List<Edge>();


            node.Visit((g) =>
            {
                if (g.Nodes != null && g.Nodes.Any())
                {
                    g.Nodes.ForEach(f =>
                    {
                        trans.Add(new Edge
                        {
                            source = g.Label,
                            target = f.Label
                        });

                    });
                }
            });

            res.nodes = nodes
                .Select(a => new Node()
                {
                    id = a.Label,
                    label = a.Label,
                    size = 3,
                    level = a.Level
                }).ToList();
            res.edges =
                trans.Select(d => new Edge()
                {
                    id = d.source + "_" + d.target,
                    source = d.source,
                    target = d.target,
                    color = "#3300ff",
                    type = "arrow",
                    label = d.label //+ " " + CreateLabel(d)
                })
                    .ToList();

            return res;
        }

        public static AutomateObject CreateViewAutomata(List<GeneLink> automata)
        {
            var res = new AutomateObject();
            res.nodes =
                automata.Select(a => a.From)
                    .Union(automata.Select(b => b.To)).Distinct()
                    .Select(a => new Node() {id = a, label = a, size = 3}).ToList();
            res.edges =
                automata.Select(d => new Edge()
                    {
                        id = d.To + "_" + d.From,
                        source = d.From,
                        target = d.To,
                        color = d.IsOptional ? "#3300ff" : "#b60d0d",
                        type = d.IsOptional ? "dashed" : "arrow",
                    })
                    .ToList();

            return res;
        }
    }
}