using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebService.Models;
using WebService.Processors;
using System.Data;

namespace WebService.Controllers
{
    public class GetRoutineForCompareController : ApiController
    {

        [HttpPost]
        [Route("GetRoutineForCompare")]
        // POST api/values
        public DataSet GetRoutineForCompare(UserID userID)
        {
            return GetUserRoutineProcessor.ProcessGetRoutineForCompare(userID);
        }


    }
}