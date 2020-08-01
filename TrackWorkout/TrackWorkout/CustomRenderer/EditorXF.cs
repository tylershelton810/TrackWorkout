using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TrackWorkout.CustomRenderer
{
    public class EditorXF:Editor
    {
        public EditorXF()
        {
            this.InvalidateMeasure();
        }
    }
}
