using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackWorkout.Entitys;
using WebService.Repositories;

namespace WebService.Processors
{
    public class GetUserIdProcessor
    {
        public static int ProcessGetUserId(UserEmail userEmail)
        {
            //Processing. Validating. Formatting.

            return GetUserIDRepository.GetUserIDFromDB(userEmail);
        }
    }
}