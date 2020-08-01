using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebService.Models;
using WebService.Processors;

namespace WebService.Controllers
{
    public class AcceptOrRejectCoachController : ApiController
    {

        [HttpPost]
        [Route("AcceptOrRejectCoach")]
        // POST api/values
        public DataSet AcceptOrRejectCoach(AcceptOrRejectCoach acceptOrRejectCoach)
        {

            if (acceptOrRejectCoach == null)
            {
                DataSet fakeDataSet = new DataSet();
                //throw
                return fakeDataSet;
            }
            return AcceptOrRejectCoachProcessor.ProcessAcceptOrRejectCoach(acceptOrRejectCoach);
        }


    }
}