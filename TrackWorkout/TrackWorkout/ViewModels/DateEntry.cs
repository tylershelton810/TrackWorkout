using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace TrackWorkout.ViewModels
{
    public class DateEntry : INotifyPropertyChanged
    {
        string label = string.Empty;
        string validDate = string.Empty;
        bool invalidCharacter = false;

        public string Label
        {
            get
            {
                return label;
            }
            set
            {
                //This variable is used to check that only numbers are being entered. 
                string formatCheck = value;
                //These are valid characters but for our compare we will remove them.
                formatCheck = formatCheck.Replace("/", "");

                //If nothing was entered do not change anything
                if (label == value)
                {
                    return;
                }

                //Check if string does contains something other than numbers or /
                if (Pages.SharedClasses.Valid.IsOnlyNumbers(formatCheck) == true)
                {
                    //Check to make sure the key entered was not a backspace before moving on.
                    if (value.Length > label.Length)
                    {
                        //if the value is 2 or 5 add the / for the user
                        if (value.Length == 2 || value.Length == 5)
                        {
                            label = value;
                            label += "/";
                            OnPropertyChanged(nameof(Label));
                            validDate = ValidDate;
                            OnPropertyChanged(nameof(ValidDate));
                        }
                        else
                        {
                            //if the value is 10 or less it is a valid length
                            if (value.Length < 11)
                            {
                                label = value;
                                OnPropertyChanged(nameof(Label));
                                validDate = ValidDate;
                                OnPropertyChanged(nameof(ValidDate));
                            }
                            //Not a valid length do no add the users entry
                            else
                            {
                                OnPropertyChanged(nameof(Label));
                                return;
                            }
                        }
                    }
                    //This handles the backspace by simply applying the change
                    else
                    {
                        label = value;
                        OnPropertyChanged(nameof(Label));
                        validDate = ValidDate;
                        OnPropertyChanged(nameof(ValidDate));
                    }
                }
                //Handles the invalid characters by ignoring them
                else
                {
                    invalidCharacter = true;
                    //validDate = ValidDate;
                    OnPropertyChanged(nameof(ValidDate));
                    OnPropertyChanged(nameof(Label));
                    return;
                }
            }
        }

        public string ValidDate
        {
            get
            {               
                //Check the format of the string to make sure it is in the requested format
                if (Pages.SharedClasses.Valid.IsValidDate(label))
                {
                    if (int.Parse(label.Substring(label.Length - 4)) <= 1900)
                    {
                        validDate = "Year must be at least 1900";
                    }
                    else if (int.Parse(label.Substring(label.Length - 4)) > DateTime.Today.Year)
                    {
                        validDate = $"Year can't be greater than {DateTime.Today.Year}";
                    }
                    else
                    {
                        //The user has a complete date
                        validDate = "";
                    }                    
                }
                else if (invalidCharacter)
                {
                    validDate = "Invalid character entered";
                    invalidCharacter = false;
                }
                else
                {
                    if (label.Length == 10 &&
                        Pages.SharedClasses.Valid.IsOnlyNumbers(label.Substring(label.Length - 4)) &&
                        Pages.SharedClasses.Valid.IsOnlyNumbers(label.Substring(0,2)) &&
                        Pages.SharedClasses.Valid.IsOnlyNumbers(label.Substring(3, 2)))
                    {
                        validDate = "Invalid Date Entered";
                    }
                    else
                    {
                        //The user has it in the incorrect format
                        validDate = "Date Format: MM/DD/YYYY";
                    }
                }
                return validDate;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
