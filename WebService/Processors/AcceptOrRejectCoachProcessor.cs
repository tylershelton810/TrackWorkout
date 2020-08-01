using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebService.Models;
using WebService.Repositories;

namespace WebService.Processors
{
    public class AcceptOrRejectCoachProcessor
    {
        public static DataSet ProcessAcceptOrRejectCoach(AcceptOrRejectCoach acceptOrRejectCoach)
        {
            //Processing. Validating. Formatting.

            return ClientRequestRepository.AcceptOrRejectCoach(acceptOrRejectCoach);
        }
    }
}