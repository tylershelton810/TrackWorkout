using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebService.Models;
using WebService.Repositories;
using System.Data;

namespace WebService.Processors
{
    public class GetCoachDataProcessor
    {
        public static DataSet ProcessGetCoachList(UserID userID)
        {
            //Processing. Validating. Formatting.

            return GetUserDataRepository.GetCoachList(userID);
        }
    }
}