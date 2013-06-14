﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Chicken.Common;
using Chicken.Model;
using Chicken.Service;
using Chicken.Service.Implementation;
using Chicken.Service.Interface;
using Chicken.ViewModel.Home.Base;

namespace Chicken.ViewModel.NewMessage
{
    public class NewMessageViewModel : PivotItemViewModelBase
    {
        #region event handler
        public delegate void AddEmotionEventHandler();
        public AddEmotionEventHandler AddEmotionHandler;
        public AddEmotionEventHandler KeyboardHandler;
        #endregion

        #region properties
        private LatestMessagesModel latestMessages;
        private List<DirectMessage> list;
        private bool hasMoreMsgs = true;
        private bool hasMoreMsgsByMe = true;
        private Dictionary<string, Conversation> dict;
        private ObservableCollection<DirectMessageViewModel> messages;
        public ObservableCollection<DirectMessageViewModel> Messages
        {
            get
            {
                return messages;
            }
            set
            {
                messages = value;
                RaisePropertyChanged("Messages");
            }
        }
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
        private NewMessageModel newMessage;
        public NewMessageModel NewMessage
        {
            get
            {
                return newMessage;
            }
            set
            {
                newMessage = value;
                RaisePropertyChanged("NewMessage");
            }
        }
        #endregion

        #region binding
        public ICommand SendCommand
        {
            get
            {
                return new DelegateCommand(SendAction);
            }
        }

        //public ICommand MentionCommand
        //{
        //    get
        //    {
        //        return new DelegateCommand(MentionAction);
        //    }
        //}

        public ICommand AddEmotionCommand
        {
            get
            {
                return new DelegateCommand(AddEmotionAction);
            }
        }

        public ICommand KeyboardCommand
        {
            get
            {
                return new DelegateCommand(KeyboardAction);
            }
        }
        #endregion

        #region services
        public ITweetService TweetService = TweetServiceManager.TweetService;
        #endregion

        public NewMessageViewModel()
        {
            Header = "Chat";
            list = new List<DirectMessage>();
            dict = new Dictionary<string, Conversation>();
            latestMessages = new LatestMessagesModel();
            Messages = new ObservableCollection<DirectMessageViewModel>();
            newMessage = new NewMessageModel();
            RefreshHandler = this.RefreshAction;
            LoadHandler = this.LoadAction;
            ClickHandler = this.ClickAction;
        }

        private void RefreshAction()
        {
            if (newMessage.Type != NewMessageActionType.Reply)
            {
                base.Refreshed();
                return;
            }
            Header += " " + newMessage.User.ScreenName;

            #region init from file
            var file = IsolatedStorageService.GetLatestMessages();
            if (file != null)
            {
                latestMessages = file;
            }
            #endregion

            GetReceivedMessages();
        }

        private void GetReceivedMessages(bool isLoadAction = false)
        {
            #region parameters
            var parameters = TwitterHelper.GetDictionary();
            if (!isLoadAction)
            {
                if (!string.IsNullOrEmpty(latestMessages.SinceId))
                {
                    parameters.Add(Const.SINCE_ID, latestMessages.SinceId);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(latestMessages.MaxId))
                {
                    parameters.Add(Const.MAX_ID, latestMessages.MaxId);
                }
            }
            #endregion

            TweetService.GetDirectMessages<DirectMessageList<DirectMessage>>(
                messages =>
                {
                    if (messages != null && messages.Count != 0)
                    {
                        foreach (var message in messages)
                        {
                            if (message.Id != latestMessages.MaxId)
                            {
                                list.Add(message);
                            }
                        }
                        if (!isLoadAction)
                        {
                            latestMessages.SinceId = messages.First().Id;
                        }
                        latestMessages.MaxId = messages.Last().Id;
                    }
                    else
                    {
                        if (isLoadAction)
                            hasMoreMsgs = false;
                    }

                    GetDirectMessagesSentByMe(isLoadAction);
                }, parameters);
        }

        private void GetDirectMessagesSentByMe(bool isLoadAction = false)
        {
            #region parameters
            var parameters = TwitterHelper.GetDictionary();
            if (!isLoadAction)
            {
                if (!string.IsNullOrEmpty(latestMessages.SinceIdByMe))
                {
                    parameters.Add(Const.SINCE_ID, latestMessages.SinceIdByMe);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(latestMessages.MaxIdByMe))
                {
                    parameters.Add(Const.MAX_ID, latestMessages.MaxIdByMe);
                }
            }
            #endregion

            TweetService.GetDirectMessagesSentByMe<DirectMessageList<DirectMessage>>(
                messages =>
                {
                    #region get messsages
                    if (messages != null && messages.Count != 0)
                    {
                        foreach (var message in messages)
                        {
                            if (message.Id != latestMessages.MaxIdByMe)
                            {
                                message.IsSentByMe = true;
                                message.User = message.Receiver;
                                list.Add(message);
                            }
                        }
                        if (!isLoadAction)
                        {
                            latestMessages.SinceIdByMe = messages.First().Id;
                        }
                        latestMessages.MaxIdByMe = messages.Last().Id;
                    }
                    else
                    {
                        if (isLoadAction)
                            hasMoreMsgsByMe = false;
                    }
                    #endregion

                    #region group

                    var group = list.OrderByDescending(m => m.Id).GroupBy(u => u.User.Id);

                    foreach (var msgs in group)
                    {
                        foreach (var msg in msgs)
                        {
                            if (!dict.ContainsKey(msgs.Key))
                            {
                                dict.Add(msgs.Key, new Conversation());
                            }
                            dict[msgs.Key].Messages.Add(msg);
                        }

                        IsolatedStorageService.AddMessages(dict[msgs.Key]);

                        #region store latest msgs
                        var first = msgs.First();
                        if (latestMessages.Messages.ContainsKey(first.User.Id))
                        {
                            latestMessages.Messages[first.User.Id] = first;
                        }
                        else
                        {
                            latestMessages.Messages.Add(first.User.Id, first);
                        }
                        #endregion
                    }
                    #endregion

                    #region finish
                    FinishRefreshing();
                    #endregion
                }, parameters);
        }

        private void FinishRefreshing()
        {
            var conversation = IsolatedStorageService.GetMessages(newMessage.User.Id);
            if (conversation != null)
            {
                Messages.Clear();
                var msgs = conversation.Messages.OrderBy(m => m.Id);
                foreach (var msg in msgs)
                {
                    Messages.Add(new DirectMessageViewModel(msg));
                }
            }
            base.Refreshed();
            base.Loaded();
            IsolatedStorageService.AddLatestMessages(latestMessages);
        }

        private void LoadAction()
        {
            if (hasMoreMsgs && hasMoreMsgsByMe)
            {
                list.Clear();
                dict.Clear();
                GetReceivedMessages(true);
            }
            else
            {
                LoadHandler = null;
                base.Loaded();
            }
        }

        private void ClickAction(object parameter)
        {
            NavigationServiceManager.NavigateTo(Const.PageNameEnum.ProfilePage, parameter);
        }

        #region actions
        private void SendAction()
        {
            if (string.IsNullOrEmpty(newMessage.Text))
            {
                return;
            }
            //TweetService.postNewMessage();
        }

        private void AddEmotionAction()
        {
            if (AddEmotionHandler != null)
            {
                State = AppBarState.AddEmotion;
                AddEmotionHandler();
            }
        }

        private void KeyboardAction()
        {
            if (KeyboardHandler != null)
            {
                State = AppBarState.Default;
                KeyboardHandler();
            }
        }
        #endregion
    }
}
