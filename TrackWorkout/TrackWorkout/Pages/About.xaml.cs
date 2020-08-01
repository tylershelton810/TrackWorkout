using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class About : ContentPage
	{
		public About ()
		{
			InitializeComponent ();

            //For smartphone apps, please set a link to https://icons8.com in the About dialog or settings.

            //Also, please credit our work in your App Store or Google Play description(something like "Icons by Icons8" is fine).

        }
	}
}