using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneAutomate.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneAutomate.Parser.Tests
{
    [TestClass()]
    public class RuleFileParserTests
    {
        [TestMethod()]
        public void TestCanReadToysSpec()
        {
            var parser = new RuleFileParser();

            var res = parser.ParseRules(@"C:\Users\ygalbe\Dropbox\Automate\toy.spec", new List<string>() {"S1"});

            Assert.AreEqual(res.Conditions.Count,4);
            Assert.IsTrue(res.Conditions.ContainsKey("Conditions1"));
            Assert.IsTrue(res.Conditions.ContainsKey("Conditions2"));
            Assert.IsTrue(res.Conditions.ContainsKey("Expression1"));
            Assert.IsTrue(res.Conditions.ContainsKey("Expression2"));

            Assert.IsTrue(res.Conditions["Conditions1"]["S1"] == false);
            Assert.IsTrue(res.Conditions["Conditions1"]["S2"] == true);

            Assert.IsTrue(res.Conditions["Conditions2"]["S1"] == true);
            Assert.IsTrue(res.Conditions["Conditions2"]["S2"] == true);

            Assert.IsTrue(res.Conditions["Expression1"]["A"] == true);
            Assert.IsTrue(res.Conditions["Expression1"]["B"] == true);
            Assert.IsTrue(res.Conditions["Expression1"]["C"] == true);

            Assert.IsTrue(res.Conditions["Expression2"]["A"] == false);
            Assert.IsTrue(res.Conditions["Expression2"]["B"] == true);
            Assert.IsTrue(res.Conditions["Expression2"]["C"] == true);

            Assert.IsTrue(res.Experiments.Count == 2);
            Assert.IsTrue(res.Experiments.ContainsKey("Experiment1"));
            Assert.IsTrue(res.Experiments.ContainsKey("Experiment2"));

            Assert.IsTrue(res.Experiments["Experiment1"].Conditions[0]["S1"] == false);
            Assert.IsTrue(res.Experiments["Experiment1"].Conditions[0]["S2"] == true);
            Assert.IsTrue(res.Experiments["Experiment1"].Conditions[0]["A"] == true);
            Assert.IsTrue(res.Experiments["Experiment1"].Conditions[0]["B"] == true);
            Assert.IsTrue(res.Experiments["Experiment1"].Conditions[0]["C"] == true);

            
            Assert.IsTrue(res.Experiments["Experiment1"].Conditions[18]["A"] == false);
            Assert.IsTrue(res.Experiments["Experiment1"].Conditions[18]["B"] == true);
            Assert.IsTrue(res.Experiments["Experiment1"].Conditions[18]["C"] == true);
            Assert.IsTrue(res.Experiments["Experiment1"].Conditions[18].IsFixedPoint);

            Assert.IsTrue(res.Experiments["Experiment2"].Conditions[0]["S1"] == true);
            Assert.IsTrue(res.Experiments["Experiment2"].Conditions[0]["S2"] == true);
            Assert.IsTrue(res.Experiments["Experiment2"].Conditions[0]["A"] == false);
            Assert.IsTrue(res.Experiments["Experiment2"].Conditions[0]["B"] == true);
            Assert.IsTrue(res.Experiments["Experiment2"].Conditions[0]["C"] == true);


            Assert.IsTrue(res.Experiments["Experiment2"].Conditions[18]["A"] == true);
            Assert.IsTrue(res.Experiments["Experiment2"].Conditions[18]["B"] == true);
            Assert.IsTrue(res.Experiments["Experiment2"].Conditions[18]["C"] == true);
            Assert.IsTrue(res.Experiments["Experiment2"].Conditions[18].IsFixedPoint);



        }

        [TestMethod]
        public void TestCanReadPluriPotencySpec()
        {
            var parser = new RuleFileParser();

            var res = parser.ParseRules(@"C:\dev\Automate\GeneAutomate\GeneAutomate\Examples\pluripotency.spec", new List<string>() { "LIF" });

            var condition = res.Conditions;
            var exp = res.Experiments;

            Assert.AreEqual(23, exp.Count);
            Assert.IsTrue(exp.ContainsKey("ExperimentOne"));
            Assert.IsTrue(exp.ContainsKey("ExperimentTwo"));
            Assert.IsTrue(exp.ContainsKey("ExperimentThree"));
            Assert.IsTrue(exp.ContainsKey("ExperimentFour"));
            Assert.IsTrue(exp.ContainsKey("ExperimentFive"));
            Assert.IsTrue(exp.ContainsKey("ExperimentSix"));
            Assert.IsTrue(exp.ContainsKey("ExperimentSeven"));
            Assert.IsTrue(exp.ContainsKey("ExperimentEight"));
            Assert.IsTrue(exp.ContainsKey("ExperimentNine"));
            Assert.IsTrue(exp.ContainsKey("ExperimentTen"));
            Assert.IsTrue(exp.ContainsKey("ExperimentEleven"));
            Assert.IsTrue(exp.ContainsKey("ExperimentTwelve"));
            Assert.IsTrue(exp.ContainsKey("ExperimentThirteen"));
            Assert.IsTrue(exp.ContainsKey("ExperimentFourteen"));
            Assert.IsTrue(exp.ContainsKey("ExperimentFifteen"));
            Assert.IsTrue(exp.ContainsKey("ExperimentSixteen"));
            Assert.IsTrue(exp.ContainsKey("ExperimentSeventeen"));
            Assert.IsTrue(exp.ContainsKey("ExperimentEighteen"));
            Assert.IsTrue(exp.ContainsKey("ExperimentNineteen"));
            Assert.IsTrue(exp.ContainsKey("ExperimentTwenty"));
            Assert.IsTrue(exp.ContainsKey("ExperimentTwentyOne"));
            Assert.IsTrue(exp.ContainsKey("ExperimentTwentyTwo"));
            Assert.IsTrue(exp.ContainsKey("ExperimentTwentyThree"));

            Assert.AreEqual(33,condition.Count);

            Assert.IsTrue(condition["2iPlusLifTfcp2l1Overexpression"]["MEKERK"] == false);
            Assert.IsTrue(condition["2iPlusLifTfcp2l1Overexpression"]["Oct4"] == true);
            Assert.IsTrue(condition["2iPlusLifTfcp2l1Overexpression"]["Sox2"] == true);
            Assert.IsTrue(condition["2iPlusLifTfcp2l1Overexpression"]["Nanog"] == true);
            Assert.IsTrue(condition["2iPlusLifTfcp2l1Overexpression"]["Esrrb"] == true);
            Assert.IsTrue(condition["2iPlusLifTfcp2l1Overexpression"]["Klf2"] == true);
            Assert.IsTrue(condition["2iPlusLifTfcp2l1Overexpression"]["Tfcp2l1"] == true);
            Assert.IsTrue(condition["2iPlusLifTfcp2l1Overexpression"]["Klf4"] == true);
            Assert.IsTrue(condition["2iPlusLifTfcp2l1Overexpression"]["Gbx2"] == true);
            Assert.IsTrue(condition["2iPlusLifTfcp2l1Overexpression"]["Tbx3"] == true);
            Assert.IsTrue(condition["2iPlusLifTfcp2l1Overexpression"]["Tcf3"] == false);
            Assert.IsTrue(condition["2iPlusLifTfcp2l1Overexpression"]["Sall4"] == true);
            Assert.IsTrue(condition["2iPlusLifTfcp2l1Overexpression"]["Stat3"] == true);
        }
    }
}