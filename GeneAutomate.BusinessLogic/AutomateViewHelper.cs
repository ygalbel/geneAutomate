﻿using System.Collections.Generic;
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
                    g.Transitions.ForEach(f => trans.Add(new Edge {
                        source = g.NodeName,
                        target = f.Node.NodeName,
                        label = CreateLabel(f)
                    }));
                }
            });

            res.nodes = nodes.Select(a => new Node() { id = a.NodeName, label = a.NodeName, size = 3 }).ToList();
            res.edges =
                trans.Select(d => new Edge()
                {
                    id = d.source + "_" + d.target,
                    source = d.source,
                    target = d.target,
                    color =  "#3300ff",
                    type = "arrow",
                    label = d.label
                })
                    .ToList();

            return res;
        }

        private static string CreateLabel(GeneTransition f)
        {
            var label = "";

            if (f.Condition.Any())
            {
                label = string.Join(",", f.Condition.Select(a => $"{a.Key}:{a.Value}"));
            }
            else
            {
                label = "?";
            }

            return label;
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