using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebService.Models;
using WebService.Repositories;
using System.Data;

namespace WebService.Processors
{
    public class GetClientDataProcessor
    {
        public static DataSet ProcessGetClientData(UserID userID)
        {
            //Processing. Validating. Formatting.

            return GetUserDataRepository.GetClientData(userID);
        }
    }
}