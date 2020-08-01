using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Entitys
{
    public class ColorWheel
    {
        public Color Hex { get; set; }
        public string Color { get; set; } 
        public double PrimaryOpacity { get; set; }
        public double SecondaryOpacity { get; set; }
        public int ColorCode { get; set; }
    }

    public class ColorWheelRootObject

    {
        public List<ColorWheel> ColorWheel { get; set; }
    }
}
