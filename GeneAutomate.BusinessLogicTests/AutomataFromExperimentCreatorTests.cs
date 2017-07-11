using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneAutomate.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Models;

namespace GeneAutomate.BusinessLogic.Tests
{
    [TestClass()]
    public class AutomataFromExperimentCreatorTests
    {
        private AutomataFromExperimentCreator _creator = new AutomataFromExperimentCreator();

        [TestMethod()]
        public void TestCanCreateSimpleAutomataFromOneNodeExperiment()
        {
            var res = _creator.CreateAutomata(new Experiment()
            {
                Name = "Test",
                Conditions = new Dictionary<int, Condition>() {{1, new Condition() {{"A", true}, {"B", false}}}}
            });

            Assert.IsNotNull(res);
            Assert.AreEqual(1, res.NodeLength);
            Assert.AreEqual("Test_1",res.NodeName);
        }


        [TestMethod()]
        public void TestCreateExperimenyWithTwoNodesClosed()
        {
            var res = _creator.CreateAutomata(new Experiment()
            {
                Name = "Test",
                Conditions = new Dictionary<int, Condition>()
                {
                    { 1, new Condition() { { "A", true }, { "B", false } } },
                    { 2, new Condition() { { "A", true }, { "B", false } } }
                }
            });

            Assert.IsNotNull(res);
            Assert.AreEqual(2, res.NodeLength);
        }

        [TestMethod()]
        public void TestCreateExperimenyWithTwoNodesWithGaps()
        {
            var res = _creator.CreateAutomata(new Experiment()
            {
                Name = "Test",
                Conditions = new Dictionary<int, Condition>()
                {
                    { 1, new Condition() { { "A", true }, { "B", false } } },
                    { 3, new Condition() { { "A", true }, { "B", false } } }
                }
            });

            Assert.IsNotNull(res);
            Assert.AreEqual(3, res.NodeLength);
        }
    }
}