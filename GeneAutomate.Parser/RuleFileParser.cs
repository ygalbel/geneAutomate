using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Models;

namespace GeneAutomate.Parser
{
    public class RuleFileParser
    {
        public ParseRuleResponse ParseRules(string path, List<string> variables)
        {
            var lines = File.ReadLines(path);


            var conditions = new Dictionary<string, Condition>();
            var experiment = new Dictionary<string, Experiment>();

            foreach (var line in lines)
            {
                var condition = HandleConditions(variables, line);

                if (condition != null)
                {
                    conditions[condition.Name] = condition;
                }
            }

            foreach (var line in lines)
            {
                HandleExperiments(line, experiment, conditions);
            }

            return new ParseRuleResponse() { Conditions = conditions, Experiments = experiment };
        }

        private void HandleExperiments(string line, Dictionary<string, Experiment> experiment, Dictionary<string, Condition> conditions)
        {
            var trim = line.Trim();

            if (trim.StartsWith("fixpoint"))
            {
                ParseFixPoint(trim, experiment);
            }
            else if (trim.StartsWith("#")) // experiment
            {
                PasreExperimentLine(trim, experiment, conditions);
            }

            

        }

        private void ParseFixPoint(string trim, Dictionary<string, Experiment> experiments)
        {
            //fixpoint(#Experiment1[18])
            var name = trim.Split('#')[1].Split('[')[0];
            var experimentTime = Int32.Parse(trim.Split('[')[1].Split(']')[0]);

            if (!experiments.ContainsKey(name))
            {
                experiments[name] = new Experiment() { Name = name };
            }

            var experiment = experiments[name];

            if (experiment.Conditions.ContainsKey(experimentTime))
            {
                experiment.Conditions[experimentTime].IsFixedPoint = true;
            }
            else
            {
                experiment.Conditions[experimentTime] = new Condition() { IsFixedPoint = true };
            }

        }

        private static string tempLine;
        private static bool isOnMultipleLineCondition = false;
        private Condition HandleConditions(List<string> variables, string line)
        {

            var conditions = new List<Condition>();

            var trim = line.Trim();

            if (trim.StartsWith("$")) // condition
            {
                if (line.Contains("}")) // single line condition
                {
                    return CreateConditionFromSingleLine(line, variables);
                }
                else
                {
                    isOnMultipleLineCondition = true;
                    tempLine = line;
                }
            }
            else if (trim.StartsWith("}") && isOnMultipleLineCondition)
            {
                tempLine += " " + line;
                isOnMultipleLineCondition = false;
                return CreateConditionFromSingleLine(tempLine, variables);
            }
            else if (isOnMultipleLineCondition)
            {
                tempLine += " " + line;
            }

            return null;
        }

        private void PasreExperimentLine(string line, Dictionary<string, Experiment> experiments, Dictionary<string, Condition> conditions)
        {
            var experimentName = line.Split('[')[0].Substring(1); //#ExperimentOne[0]
            var experimentTime = Int32.Parse(line.Split('[')[1].Split(']')[0]);
            var value = line.Split(' ').ToList().First(a => a.StartsWith("$")).Substring(1);


            var currentCondition = conditions[value];


            if (!experiments.ContainsKey(experimentName)) // first time
            {
                experiments[experimentName] = new Experiment() { Name = experimentName };
            }

            var currentExperiment = experiments[experimentName];

            if (!currentExperiment.Conditions.ContainsKey(experimentTime))
            {
                currentExperiment.Conditions[experimentTime] = currentCondition;
            }
            else // existing only OR
            {
                var conditionToAdd = currentCondition;//.Where(a => a.Value == true);

                conditionToAdd.ToList().ForEach(c =>
                {
                    if (!currentExperiment.Conditions[experimentTime].ContainsKey(c.Key))
                    {
                        currentExperiment.Conditions[experimentTime][c.Key] = c.Value;
                    }
                    else
                    {

                        var currentValue = currentExperiment.Conditions[experimentTime][c.Key];


                        bool? newValue;

                        if (!currentValue.HasValue)
                        {
                            newValue = c.Value;
                        }
                        else if (!c.Value.HasValue)
                        {
                            newValue = currentValue.Value;
                        }
                        else // both have values make OR
                        {
                            newValue = currentValue.Value | c.Value.Value;
                        }

                        currentExperiment.Conditions[experimentTime][c.Key] = newValue;
                    }
                });
            }
        }

        private Condition CreateConditionFromSingleLine(string line, List<string> variables)
        {
            var splitted = line.Split(':');
            var conditionName = splitted[0].Trim().Substring(1); // line is like $Conditions1 so i skip $
            splitted = splitted[1].Split('{')[1].Split('}'); // todo: change to regex;

            var rules = splitted[0].Split(new[] { "and" }, StringSplitOptions.None).ToList();

            var row = new Condition() { Name = conditionName };
            variables.ForEach(v => row.Add(v, null));

            rules.ForEach(r =>
            {
                var s = r.Split('=');
                var name = s[0].Trim();
                var value = s[1].Trim() == "1";
                row[name] = value;
            });

            return row;
        }
    }
}
