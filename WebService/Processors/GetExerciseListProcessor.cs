using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebService.Models;
using WebService.Repositories;
using System.Data;

namespace WebService.Processors
{
    public class GetExerciseListProcessor
    {
        public static DataSet ProcessGetExerciseList()
        {
            //Processing. Validating. Formatting.

            return GetUserDataRepository.GetExerciseList();
        }
    }
}