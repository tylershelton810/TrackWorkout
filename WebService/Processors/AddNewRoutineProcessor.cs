using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TrackWorkout.Entitys;
using WebService.Repositories;

namespace WebService.Processors
{
    public class AddNewRoutineProcessor
    {
        public static DataSet ProcessAddNewRoutine(List<RoutineList> routineList)
        {
            //Processing. Validating. Formatting.

            return WorkoutRepository.AddNewRoutine(routineList);
        }

        public static void ProcessDeleteRoutine(Models.UserID RoutineID)
        {
            //Processing. Validating. Formatting.

            WorkoutRepository.DeleteRoutine(RoutineID);
        }
    }
}