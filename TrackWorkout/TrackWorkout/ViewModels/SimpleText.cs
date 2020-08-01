using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace TrackWorkout.ViewModels
{
    public class SimpleTextViewModel : INotifyPropertyChanged
    {
        string DynamicText;

        public event PropertyChangedEventHandler PropertyChanged;

        public string TextValue
        {
            get
            {
                return DynamicText;
            }
            set
            {
                if (DynamicText != value)
                {
                    DynamicText = value;
                    //OnPropertyChanged("TextValue");
                    //SetNewColor();
                }
            }
            
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
