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
    public class CompleteWorkoutController : ApiController
    {

        [HttpPost]
        [Route("CompleteWorkout")]
        // POST api/values
        public DataSet CompleteWorkout(RoutineList routineList)
        {
            return WorkoutProcessor.ProcessCompleteWorkout(routineList);
        }


    }
}