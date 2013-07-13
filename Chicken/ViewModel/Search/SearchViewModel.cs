﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Chicken.Common;
using Chicken.Service;
using Chicken.ViewModel.Search.VM;

namespace Chicken.ViewModel.Search
{
    public class SearchViewModel : PivotViewModelBase
    {
        #region properties
        public string SearchQuery { get; set; }
        #endregion

        #region binding
        public ICommand NewTweetCommand
        {
            get
            {
                return new DelegateCommand(NewTweetAction);
            }
        }

        #endregion

        public SearchViewModel()
        {
            var baseViewModelList = new List<PivotItemViewModelBase>
            {
                new SearchTweetViewModel(),
                new SearchUserViewModel()
            };
            PivotItems = new ObservableCollection<PivotItemViewModelBase>(baseViewModelList);
        }

        #region public method
        public void Search()
        {
            if (string.IsNullOrEmpty(SearchQuery))
                return;
            IsolatedStorageService.CreateObject(Const.SearchPage, SearchQuery);
            (PivotItems[SelectedIndex] as SearchViewModelBase).Search();
        }
        #endregion

        #region actions
        private void NewTweetAction()
        {
            (PivotItems[SelectedIndex] as SearchViewModelBase).NewTweet();
        }


        #endregion
    }
}