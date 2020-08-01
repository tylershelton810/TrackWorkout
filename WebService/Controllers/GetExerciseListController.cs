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
    public class GetExerciseListController : ApiController
    {

        [HttpPost]
        [Route("GetExerciseList")]
        // POST api/values
        public DataSet GetExerciseList()
        {
            return GetExerciseListProcessor.ProcessGetExerciseList();
        }


    }
}