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
            var res = new AutomateObject();
            res.nodes =
                data.GeneLinks.Select(a => a.From)
                            .Union(data.GeneLinks.Select(b => b.To)).Distinct()
                            .Select(a => new Node() { id = a, label = a, size = 3 }).ToList();
            res.edges =
                data.GeneLinks.Select(d => new Edge()
                {
                    id = d.To + "_" + d.From,
                    source = d.From,
                    target = d.To,
                    color = d.IsOptional ? "#3300ff" : "#b60d0d",
                    type = d.IsOptional ? "dashed" : "arrow",
                })
                    .ToList();

            return res;
        }
    }
}
