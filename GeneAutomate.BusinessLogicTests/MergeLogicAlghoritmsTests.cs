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
    public class MergeLogicAlghoritmsTests
    {
        [TestMethod()]
        public void CreateWithNegativeLogicTest()
        {
            var c1 = new Condition() { {"A", true},  };
            var c2 = new Condition() { { "A", true }, {"B", true} };
            var res = new MergeLogicAlghoritms().CreateWithNegativeLogic(c1, c2);

            Assert.AreEqual(res.Count,2);
            Assert.IsTrue(res["A"].Value);
            Assert.IsFalse(res["B"].Value);
        }
    }
}