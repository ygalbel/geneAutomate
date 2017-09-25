using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneAutomate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneAutomate.Models.Tests
{
    [TestClass()]
    public class GeneNodeTests
    {
        [TestMethod()]
        public void TestReturnEmptyInCaseOfnull()
        {
            var merged = new GeneNode() { MergeName = null }.GetAllMergedExperiment();
            Assert.AreEqual(null, merged);
        }

        [TestMethod()]
        public void TestReturnExperimentNameSingleCase()
        {
            var merged = new GeneNode() { MergeName = "Experiment_0" }.GetAllMergedExperiment();
            Assert.AreEqual("Experiment", merged.First());
        }


        [TestMethod()]
        public void TestReturnExperimentNameDistinct()
        {
            var merged = new GeneNode() { MergeName = "Experiment_0 ~ Experiment_2" }.GetAllMergedExperiment();
            Assert.AreEqual(1, merged.Count);
            Assert.AreEqual("Experiment", merged.First());
        }

        [TestMethod()]
        public void IsLoopFixedPointSimplePositiveTest()
        {
            var node1 = new GeneNode()
            {
                CurrentCondition = new Condition() { { "A" , true}, {"B", false} },
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition() { Node = new GeneNode()
                    {
                        CurrentCondition = new Condition() { { "A", true}, { "B", false} }
                    }}
                }
            };

            Assert.IsTrue(node1.IsLoopFixedPoint());
        }


        [TestMethod()]
        public void IsLoopFixedPointSimpleNegativeTest()
        {
            var node1 = new GeneNode()
            {
                CurrentCondition = new Condition() { { "A", true }, { "B", false } },
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition() { Node = new GeneNode()
                    {
                        CurrentCondition = new Condition() { { "A", true}, { "B", true} }
                    }}
                }
            };

            Assert.IsFalse(node1.IsLoopFixedPoint());
        }

        [TestMethod()]
        public void IsLoopFixedPointMissingKeyNegativeTest()
        {
            var node1 = new GeneNode()
            {
                CurrentCondition = new Condition() { { "A", true }, { "B", false } },
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition() { Node = new GeneNode()
                    {
                        CurrentCondition = new Condition() { { "A", true} }
                    }}
                }
            };

            Assert.IsFalse(node1.IsLoopFixedPoint());
        }


        [TestMethod()]
        public void IsLoopFixedPointMissingKeyChildNegativeTest()
        {
            var node1 = new GeneNode()
            {
                CurrentCondition = new Condition() { { "A", true } },
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition() { Node = new GeneNode()
                    {
                        CurrentCondition = new Condition() { { "A", true}, { "B", false } }
                    }}
                }
            };

            Assert.IsFalse(node1.IsLoopFixedPoint());
        }
    }
}