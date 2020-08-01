using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackWorkout.Pages.Routine
{
    public class RoutinesDetailBottomNavMenuItem
    {
        public RoutinesDetailBottomNavMenuItem()
        {
            TargetType = typeof(RoutinesDetailBottomNavDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}
