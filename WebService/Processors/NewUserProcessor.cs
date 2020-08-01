using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackWorkout.Entitys;
using WebService.Models;
using WebService.Repositories;

namespace WebService.Processors
{
    public class NewUserProcessor
    {
        public static int ProcessNewUser(User newUser)
        {
            //Processing. Validating. Formatting.

            return AccountInformationRepository.AddNewUserToDB(newUser);
        }
    }
}