using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebService.Models;
using WebService.Repositories;
using System.Data;

namespace WebService.Processors
{
    public class GetUserRoutineProcessor
    {
        public static DataSet ProcessGetUserRoutine(UserID userID)
        {
            //Processing. Validating. Formatting.

            return WorkoutRepository.GetUserRoutineList(userID);
        }

        public static DataSet ProcessGetRoutineForCompare(UserID userID)
        {
            //Processing. Validating. Formatting.

            return WorkoutRepository.GetRoutineForCompare(userID);
        }
    }
}