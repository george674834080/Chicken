﻿using System.Collections.ObjectModel;
using Chicken.Model;

namespace Chicken.ViewModel.Profile.Base
{
    public class FriendListViewModel : NotificationObject
    {
        private ObservableCollection<FriendProfileViewModel> friendProfileList = new ObservableCollection<FriendProfileViewModel>();
        public ObservableCollection<FriendProfileViewModel> FriendProfileList
        {
            get
            {
                return friendProfileList;
            }
            set
            {
                friendProfileList = value;
                RaisePropertyChanged("FriendProfileList");
            }
        }

        private string nextCursor;
        public string NextCursor
        {
            get
            {
                return nextCursor;
            }
            set
            {
                nextCursor = value;
            }
        }

        private string previousCursor;
        public string PreviousCursor
        {
            get
            {
                return previousCursor;
            }
            set
            {
                previousCursor = value;
            }
        }

        public FriendListViewModel() { }

        public void Refresh(FriendList list)
        {
            if (list.Users != null && list.Users.Count != 0)
            {
                foreach (var userProfile in list.Users)
                {
                    this.FriendProfileList.Add(new FriendProfileViewModel(userProfile));
                }
            }
            this.nextCursor = list.NextCursor;
            this.previousCursor = list.PreviousCursor;
        }
    }
}