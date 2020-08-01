using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Entitys
{
    public class User
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime Birthday { get; set; }

        public int WeightType { get; set; }

        public float Weight { get; set; }

        public string Source { get; set; }

        public string Photo { get; set; }
    }
}
