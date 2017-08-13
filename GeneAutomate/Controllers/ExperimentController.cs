using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using GeneAutomate.Models;
using GeneAutomate.Parser;

namespace GeneAutomate.Controllers
{
    public class ExperimentController : ApiController
    {

        public IHttpActionResult GetConditionAndExperiments(string exampleName)
        {
            var parser = new FileParser();

            var res = parser.ParseFiles(PathHelper.GetExamplePath(exampleName), PathHelper.GetSpecPath(exampleName));

            return Ok(res);
        }
    }
}
