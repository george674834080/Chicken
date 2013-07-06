﻿using System.Collections.ObjectModel;
using Chicken.Common;
using Chicken.Model;
using Chicken.ViewModel.Base;

namespace Chicken.ViewModel.Profile.VM
{
    public class UserFollowersViewModel : ProfileViewModelBase
    {
        public UserFollowersViewModel()
        {
            Header = "Followers";
            UserList = new ObservableCollection<UserProfileViewModel>();
            RefreshHandler = this.RefreshAction;
            LoadHandler = this.LoadAction;
        }

        private void RefreshAction()
        {
            if (!CheckIfFollowingPrivate())
            {
                base.Refreshed();
                return;
            }
            if (previousCursor == "0")
            {
                base.Refreshed();
                return;
            }
            #region parameter
            var parameters = TwitterHelper.GetDictionary();
            if (!string.IsNullOrEmpty(previousCursor))
            {
                parameters.Add(Const.CURSOR, previousCursor);
            }
            #endregion
            TweetService.GetFollowerIds(UserProfile.Id,
                userIdList =>
                {
                    if (userIdList.HasError)
                    {
                        base.Refreshed();
                        return;
                    }
                    nextCursor = userIdList.NextCursor;
                    previousCursor = userIdList.PreviousCursor;
                    string ids = string.Join(",", userIdList.UserIds);
                    RefreshUserProfiles(ids);
                }, parameters);
        }

        private void LoadAction()
        {
            if (nextCursor == "0")
            {
                base.Loaded();
                return;
            }
            #region parameter
            var parameters = TwitterHelper.GetDictionary();
            if (!string.IsNullOrEmpty(nextCursor))
            {
                parameters.Add(Const.CURSOR, nextCursor);
            }
            #endregion
            TweetService.GetFollowerIds(UserProfile.Id,
                userIdList =>
                {
                    if (userIdList.HasError)
                    {
                        base.Loaded();
                        return;
                    }
                    nextCursor = userIdList.NextCursor;
                    previousCursor = userIdList.PreviousCursor;
                    string ids = string.Join(",", userIdList.UserIds);
                    LoadUserProfiles(ids);
                }, parameters);
        }
    }
}
