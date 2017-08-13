using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GeneAutomate.BDD;
using GeneAutomate.BusinessLogic;
using GeneAutomate.Parser;

namespace GeneAutomate.Controllers
{
    public class BDDController : ApiController
    {
        public IHttpActionResult GetData()
        {
            var solver = new BDDSolver();

            var parser = new FileParser();

            var data = new ParseRuleResponse();
            var res = parser.GetConditionAndExperiments(PathHelper.GetExamplePath("toy"), PathHelper.GetSpecPath("toy"), out data);

            var automates =
                data.Experiments.ToDictionary(s => s.Key,
                    s => new AutomataFromExperimentCreator().CreateAutomata(s.Value));

            var sos = solver.IsValidPath(automates.First().Value, res);

            return Ok(new {message = sos});
        }

    }
}
