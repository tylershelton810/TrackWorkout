using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebService.Repositories;
using System.Data;
using TrackWorkout.Entitys;

namespace WebService.Processors
{
    public class GetUserDataProcessor
    {
        public static DataSet ProcessGetUserData(UserEmail email)
        {
            //Processing. Validating. Formatting.

            return GetUserDataRepository.GetUserData(email);
        }

        public static DataSet GetUserHistoryData(UserEmail email)
        {
            //Processing. Validating. Formatting.

            return GetUserDataRepository.GetUserHistoryData(email);
        }

        public static DataSet UpdateUserSettings(UserInformation UserInfo)
        {
            //Processing. Validating. Formatting.

            return GetUserDataRepository.UpdateUserSettings(UserInfo);
        }

    }
}