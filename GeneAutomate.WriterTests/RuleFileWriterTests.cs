using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneAutomate.Writer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Models;
using NLog;

namespace GeneAutomate.Writer.Tests
{
    [TestClass()]
    public class RuleFileWriterTests
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        [TestMethod()]
        public void WriteFileTestSingleTakeRightTime()
        {
            var builder = new RuleFileWriter();

            // Act
            var txt = builder.CreateSpecString(
                new GeneNode()
                {
                    NodeName = "Experiment1_3",
                    CurrentCondition = new Condition()
                    {
                        {"A", true},
                        {"B", false}
                    }
            });


            Assert.IsTrue(txt.Contains("#Experiment1[3] |= $Condition0"));
            logger.Info(txt);
        }

        [TestMethod()]
        public void WriteFileTestMultipleTakeRightTime()
        {
            var builder = new RuleFileWriter();

            // Act
            var txt = builder.CreateSpecString(
                new GeneNode()
                {
                    NodeName = "Experiment1_3",
                    CurrentCondition = new Condition()
                    {
                        {"A", true},
                        {"B", false}
                    }, Transitions = new List<GeneTransition>()
                    {
                        new GeneTransition()
                        {
                            Node = new GeneNode()
                            {
                                NodeName = "Experiment1_8",
                                CurrentCondition = new Condition()
                                {
                                    {"A", false }

                                }
                            }
                        }
                    }

                });


            Assert.IsTrue(txt.Contains("#Experiment1[3] |= $Condition0"));
            Assert.IsTrue(txt.Contains("#Experiment1[8] |= $Condition1"));
            logger.Info(txt);
        }
    }
}