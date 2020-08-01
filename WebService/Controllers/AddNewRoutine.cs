using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TrackWorkout.Entitys;
using WebService.Processors;

namespace WebService.Controllers
{
    public class AddNewRoutineController : ApiController
    {

        [HttpPost]
        [Route("AddNewRoutine")]
        // POST api/values
        public DataSet AddNewRoutine(List<RoutineList> routineList)
        {
            return AddNewRoutineProcessor.ProcessAddNewRoutine(routineList);
        }


    }
}