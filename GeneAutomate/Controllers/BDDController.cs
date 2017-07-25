using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GeneAutomate.BDD;

namespace GeneAutomate.Controllers
{
    public class BDDController : ApiController
    {
        public IHttpActionResult GetData()
        {
            //var msg=  BDDSolver.Test1();

            return Ok(new {message = ""});
        }

    }
}
