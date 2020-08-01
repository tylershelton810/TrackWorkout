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
    public class GetUserRoutineListController : ApiController
    {

        [HttpPost]
        [Route("GetUserRoutineList")]
        // POST api/values
        public DataSet GetUserRoutineList(UserID userID)
        {
            return GetUserRoutineProcessor.ProcessGetUserRoutine(userID);
        }


    }
}