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
    public class DeleteRoutineController : ApiController
    {

        [HttpPost]
        [Route("DeleteRoutine")]
        // POST api/values
        public void DeleteRoutine(UserID routineID)
        {
            AddNewRoutineProcessor.ProcessDeleteRoutine(routineID);
        }


    }
}