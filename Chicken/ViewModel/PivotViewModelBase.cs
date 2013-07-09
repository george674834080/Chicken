﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using Chicken.Common;
using Chicken.Service;

namespace Chicken.ViewModel
{
    public class PivotViewModelBase : ViewModelBase
    {
        #region properties
        public int SelectedIndex { get; set; }
        private AppBarState state;
        public AppBarState State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                RaisePropertyChanged("State");
            }
        }
        public ObservableCollection<PivotItemViewModelBase> PivotItems { get; set; }
        protected bool IsInit { get; set; }
        #endregion

        #region binding
        public ICommand SettingsCommand
        {
            get
            {
                return new DelegateCommand(SettingsAction);
            }
        }
        #endregion

        public PivotViewModelBase()
        {
            RefreshHandler = this.RefreshAction;
            ScrollToTopHandler = this.ScrollToTopAction;
            ScrollToBottomHandler = this.ScrollToBottomAction;
        }

        #region public method
        public virtual void MainPivot_LoadedPivotItem()
        {
            if (!PivotItems[SelectedIndex].IsInited)
            {
                Refresh();
            }
        }
        #endregion

        #region private method
        private void RefreshAction()
        {
            PivotItems[SelectedIndex].Refresh();
        }

        private void ScrollToTopAction()
        {
            PivotItems[SelectedIndex].ScrollToTop();
        }

        private void ScrollToBottomAction()
        {
            PivotItems[SelectedIndex].ScrollToBottom();
        }

        private void SettingsAction()
        {
            NavigationServiceManager.NavigateTo(PageNameEnum.SettingsPage);
        }
        #endregion
    }
}
