﻿using System.Collections.Generic;
using System.Windows.Input;
using Chicken.Common;
using Chicken.Model;
using Chicken.Service;
using Chicken.Service.Interface;

namespace Chicken.ViewModel.Settings
{
    public class APISettingsViewModel : PivotItemViewModelBase
    {
        #region properties
        private APIProxy setting;
        public APIProxy APISettings
        {
            get
            {
                return setting;
            }
            set
            {
                setting = value;
                RaisePropertyChanged("APISettings");
            }
        }
        public APIProxy[] DefaultAPIs
        {
            get
            {
                return APIProxy.DefaultAPIs;
            }
        }
        private bool isInitAPI;
        #endregion

        #region binding
        public ICommand SaveCommand
        {
            get
            {
                return new DelegateCommand(SaveAction);
            }
        }

        public ICommand SettingsCommand
        {
            get
            {
                return new DelegateCommand(SettingsAction);
            }
        }
        #endregion

        #region services
        public ITweetService TweetService = TweetServiceManager.TweetService;
        #endregion

        public APISettingsViewModel()
        {
            RefreshHandler = this.RefreshAction;
        }

        #region actions
        private void RefreshAction()
        {
            if (App.Settings != null && App.Settings.APISettings != null)
                APISettings = App.Settings.APISettings;
            else
            {
                isInitAPI = true;
                APISettings = new APIProxy
                {
                    Name = APIProxy.DefaultAPIs[0].Name,
                    DisplayName = APIProxy.DefaultAPIs[0].DisplayName,
                    Url = Const.testAPI
                };
            }
            base.Refreshed();
        }

        private void SaveAction()
        {
            if (IsLoading
                || string.IsNullOrEmpty(APISettings.Url))
                return;
            IsLoading = true;
            APISettings.Url = APISettings.Url.TrimEnd('/', '\r', '\n', ' ') + "/";
            TweetService.TestAPIUrl(APISettings.Url,
                userProfileDetail =>
                {
                    IsLoading = false;
                    List<ErrorMessage> errors = userProfileDetail.Errors;
                    if (userProfileDetail.HasError)
                        return;
                    App.InitAPISettings(APISettings);
                    IsolatedStorageService.CreateAuthenticatedUser(userProfileDetail);
                    App.InitAuthenticatedUser();
                    App.HandleMessage(new ToastMessage
                    {
                        Message = isInitAPI ? "hello, " + App.AuthenticatedUser.DisplayName : "update successfully",
                        Complete = () => NavigationServiceManager.NavigateTo(PageNameEnum.HomePage)
                    });
                });
        }

        private void SettingsAction()
        {
            IsLoading = false;
            NavigationServiceManager.NavigateTo(PageNameEnum.SettingsPage);
        }
        #endregion
    }
}
