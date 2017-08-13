using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneAutomate.BusinessLogic;
using GeneAutomate.Models;
using GeneAutomate.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneAutomate.BDD.Tests
{
    [TestClass]
    public class RealCaseBDDSolverTests
    {
        [TestMethod]
        public void TestToyCaseBddSolver()
        {
            var solver = new BDDSolver();

            var parser = new FileParser();

            var data = new ParseRuleResponse();
            var res = parser.GetConditionAndExperiments($"toy.net", $"toy.spec", out data);

            var automates =
                data.Experiments.ToDictionary(s => s.Key,
                    s => new AutomataFromExperimentCreator().CreateAutomata(s.Value));

            var sos = solver.IsValidPath(automates.First().Value, res);
            Assert.IsTrue(sos);
        }

        [TestMethod]
        public void TestHerrmannCaseBddSolverIsWrong()
        {

            var parser = new FileParser();

            var data = new ParseRuleResponse();
            var res = parser.GetConditionAndExperiments($"herrmann.net", $"herrmann.spec", out data);

            var automates =
                data.Experiments.ToDictionary(s => s.Key,
                    s => new AutomataFromExperimentCreator().CreateAutomata(s.Value));

            bool sos = true;
            int i = 0;
            automates.ToList().ForEach(a =>
            {
                var solver = new BDDSolver();

                Trace.WriteLine("Start " + (i++));
                sos &= solver.IsValidPath(a.Value, res);
                
            });

            Assert.IsFalse(sos);
        }
    }
}