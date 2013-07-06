﻿using System.Collections.Generic;
using System.Windows.Input;
using Chicken.Common;
using Chicken.Model;
using Chicken.Service;
using Chicken.Service.Interface;
using Chicken.ViewModel.Base;

namespace Chicken.ViewModel.Profile
{
    public class MyProfileViewModel : PivotItemViewModelBase
    {
        #region properties
        private UserProfileDetailViewModel myProfile;
        public UserProfileDetailViewModel MyProfile
        {
            get
            {
                return myProfile;
            }
            set
            {
                myProfile = value;
                RaisePropertyChanged("MyProfile");
            }
        }
        #endregion

        #region binding
        public ICommand SaveCommand
        {
            get
            {
                return new DelegateCommand(SaveAction);
            }
        }
        #endregion

        #region services
        public ITweetService TweetService = TweetServiceManager.TweetService;
        #endregion

        public MyProfileViewModel()
        {
            Header = "Edit";
            RefreshHandler = this.RefreshAction;
        }

        #region actions
        private void RefreshAction()
        {
            TweetService.GetMyProfileDetail(
                profile =>
                {
                    MyProfile = new UserProfileDetailViewModel(profile);
                    base.Refreshed();
                });
        }

        private void SaveAction()
        {
            IsLoading = true;
            var parameters = TwitterHelper.GetDictionary();
            if (MyProfile.Name != App.AuthenticatedUser.Name)
                parameters.Add(Const.USER_NAME, MyProfile.Name);
            if (MyProfile.Location != App.AuthenticatedUser.Location)
                parameters.Add(Const.LOCATION, MyProfile.Location);
            if (MyProfile.Url != App.AuthenticatedUser.Url)
                parameters.Add(Const.URL, MyProfile.Url);
            parameters.Add(Const.DESCRIPTION, MyProfile.ExpandedDescription);
            TweetService.UpdateMyProfile(
                user =>
                {
                    IsLoading = false;
                    List<ErrorMessage> errors = user.Errors;
                    if (errors != null && errors.Count != 0)
                    {
                        HandleMessage(new ToastMessage
                        {
                            Message = errors[0].Message
                        });
                        return;
                    }
                    HandleMessage(new ToastMessage
                    {
                        Message = "update successfully",
                        Complete =
                        () =>
                        {
                            NavigationServiceManager.NavigateTo(PageNameEnum.ProfilePage, user);
                        }
                    });
                }, parameters);
        }
        #endregion
    }
}
