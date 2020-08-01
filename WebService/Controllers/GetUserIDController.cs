using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TrackWorkout.Entitys;
using WebService.Processors;

namespace WebService.Controllers
{
    public class GetUserIDController : ApiController
    {

        [HttpPost]
        [Route("GetUserID")]
        // POST api/values
        public int GetUserID(UserEmail userEmail)
        {

            if (userEmail == null)
            {
                return -1;
            }
            return GetUserIdProcessor.ProcessGetUserId(userEmail);
        }


    }
}