﻿using System;
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

            var res = parser.ParseFiles($"{HttpRuntime.AppDomainAppPath}/Examples/{exampleName}.net", $"{HttpRuntime.AppDomainAppPath}/Examples/{exampleName}.spec");

            return Ok(res);
        }

    }
}
