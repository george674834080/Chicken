﻿using Chicken.Model;
using Chicken.Service;
using Chicken.ViewModel.Profile.Base;

namespace Chicken.ViewModel.Profile.VM
{
    public class ProfileDetailViewModel : ProfileViewModelBase
    {
        #region properties
        private UserProfileViewModel userProfileViewModel;
        public UserProfileViewModel UserProfileViewModel
        {
            get
            {
                return userProfileViewModel;
            }
            set
            {
                userProfileViewModel = value;
                RaisePropertyChanged("UserProfileViewModel");
            }
        }
        #endregion

        public ProfileDetailViewModel()
        {
            Header = "Profile";
        }

        public override void Refresh()
        {
            IsLoading = true;
            TweetService.GetUserProfileDetail<UserProfile>(UserId,
                obj =>
                {
                    this.UserProfileViewModel = new UserProfileViewModel(obj);
                    base.Refresh();
                });
        }
    }
}
