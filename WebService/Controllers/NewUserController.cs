using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TrackWorkout.Entitys;
using WebService.Models;
using WebService.Processors;

namespace WebService.Controllers
{
    public class NewUserController : ApiController
    {

        [HttpPost]
        [Route("SaveNewUser")]
        // POST api/values
        public int SaveNewUser(User newUser)
        {

            if (newUser == null)
            {
                return 0;
            }
            return NewUserProcessor.ProcessNewUser(newUser);
        }


    }
}