﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TrackWorkout.Entitys;
using TrackWorkout.Services;

namespace TrackWorkout.Pages.ViewModel
{
    public class GoogleViewModel : INotifyPropertyChanged
    {
        private GoogleProfile _googleProfile;
        private readonly GoogleServices _googleServices;

        public GoogleProfile GoogleProfile
        {
            get { return _googleProfile; }
            set
            {
                _googleProfile = value;
                OnPropertyChanged();
            }
        }

        public GoogleViewModel()
        {
            _googleServices = new GoogleServices();
        }

        public async Task<string> GetAccessTokenAsync(string code)
        {

            var accessToken = await _googleServices.GetAccessTokenAsync(code);

            return accessToken;
        }

        public async Task SetGoogleUserProfileAsync(string accessToken)
        {

            GoogleProfile = await _googleServices.GetGoogleUserProfileAsync(accessToken);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
