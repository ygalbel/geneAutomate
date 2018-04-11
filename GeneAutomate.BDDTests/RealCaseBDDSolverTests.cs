using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Threading.Tasks;
using GeneAutomate.BusinessLogic;
using GeneAutomate.Models;
using GeneAutomate.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace GeneAutomate.BDD.Tests
{
    [TestClass]
    public class RealCaseBDDSolverTests : AbstractBddTest
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [TestInitialize]
        public void Init()
        {
            logger.Info("start " + this.TestContext.TestName);
        }

        [TestMethod]
        public void TestToyCaseBddSolver()
        {
            var experimentName = "toy";
            var res = IsExistPath(experimentName, 40);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void TestToyYeastBddSolver()
        {
            var experimentName = "yeast";
            var res = IsExistPath(experimentName, 40);
            Assert.IsTrue(res);
        }

        [TestMethod]
        //   [Timeout(1000 * 120)]
        public void TestKrumsiekBddSolver()
        {
             var experimentName = "Krumsiek";
            var res = IsExistPath(experimentName, 8);
            Assert.IsTrue(res);

            // Thread.Sleep(240 * 1000);
            //Assert.IsTrue(true);

        }

        private static bool IsExistPath(string experimentName, int length)
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

            var automate = automates.First().Value;
            int i = 0;
            var temp = automate;
            while (i < length && temp != null && temp.Transitions != null && temp.Transitions.Any())
            {
                i++;
                temp = temp.Transitions.First().Node;
            }

            if (temp != null)
            {
                // cut the childrens;
                temp.Transitions = null;
            }


            var sos = solver.IsValidPath(automates.First().Value, res, automates.First().Value.NodeLength + 1);
            return sos;
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
                var solver = NinjectHelper.Get<IBDDSolver>();

                logger.Info("Start " + (i++));
                sos &= solver.IsValidPath(a.Value, res);

            });

            Assert.IsFalse(sos);
        }
    }
}