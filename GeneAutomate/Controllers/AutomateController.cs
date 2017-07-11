using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using GeneAutomate.BusinessLogic;
using GeneAutomate.Models;
using GeneAutomate.Parser;

namespace GeneAutomate.Controllers
{
    public class AutomateController : ApiController
    {

        public IHttpActionResult GetAutomate(string exampleName)
        {
            var parser = new FileParser();

            var res = parser.ParseFiles($"{HttpRuntime.AppDomainAppPath}/Examples/{exampleName}.net", $"{HttpRuntime.AppDomainAppPath}/Examples/{exampleName}.spec");

            var convertedResult = CreateAutomateViewObject(res);

            return Ok(convertedResult);
        }
        

        private AutomateObject CreateAutomateViewObject(FileParsingResult data)
        {
            var automata = data.GeneLinks;
            return AutomateViewHelper.CreateViewAutomata(automata);
        }
    }
}
