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
            var merged = new GeneNode() {MergeName = null}.GetAllMergedExperiment();
            Assert.AreEqual(null,merged);
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
    }
}