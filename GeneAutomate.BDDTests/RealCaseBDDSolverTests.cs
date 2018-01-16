using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneAutomate.BusinessLogic;
using GeneAutomate.Models;
using GeneAutomate.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace GeneAutomate.BDD.Tests
{
    [TestClass]
    public class RealCaseBDDSolverTests
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        [TestMethod]
        public void TestToyCaseBddSolver()
        {
            var experimentName = "toy";
            var res = IsExistPath(experimentName);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void TestToyYeastBddSolver()
        {
            var experimentName = "yeast";
            var res = IsExistPath(experimentName);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void TestKrumsiekBddSolver()
        {
            var experimentName = "Krumsiek";
            var res = IsExistPath(experimentName);
            Assert.IsTrue(res);
        }

        private static bool IsExistPath(string experimentName)
        {
            var solver = NinjectHelper.Get<IBDDSolver>();

            var parser = new FileParser();

            var data = new ParseRuleResponse();
            var res = parser
                .GetConditionAndExperiments($"{experimentName}.net", 
                    $"{experimentName}.spec", out data);

            var automates =
                data.Experiments.ToDictionary(s => s.Key,
                    s => new AutomataFromExperimentCreator()
                        .CreateAutomata(s.Value));

            var sos = solver.IsValidPath(automates.First().Value, res);
            return sos;
        }

        [TestMethod]
        public void TestHerrmannCaseBddSolverIsWrong()
        {

            var parser = new FileParser();

            var data = new ParseRuleResponse();
            var res = parser.ParseFiles($"herrmann.net", $"herrmann.spec");
            

            Assert.IsFalse(res.MergeObjects.Any());
        }
    }
}