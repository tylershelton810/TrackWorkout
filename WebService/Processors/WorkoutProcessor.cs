using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TrackWorkout.Entitys;
using WebService.Repositories;

namespace WebService.Processors
{
    public class WorkoutProcessor
    {
        public static DataSet ProcessCompleteWorkout(RoutineList routineList)
        {
            //Processing. Validating. Formatting.

            return WorkoutRepository.CompleteWorkout(routineList);
        }
    }
}