﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using Chicken.Common;
using Chicken.Model;
using Chicken.Service.Interface;
using Newtonsoft.Json;


namespace Chicken.Service.Implementation
{
    public class TweetService : ITweetService
    {
        #region properties
        #region properties
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
        private static JsonSerializer jsonSerializer = JsonSerializer.Create(jsonSettings);
        #endregion
        #endregion

        #region private method
        private void HandleWebRequest<T>(string url, Action<T> callBack, string method = Const.HTTPGET)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = method.ToString();
            request.BeginGetResponse(
                (result) =>
                {
                    HttpWebRequest requestResult = (HttpWebRequest)result.AsyncState;
                    WebResponse response = null;
                    response = requestResult.EndGetResponse(result);
                    T output = default(T);
                    var streamReader = new StreamReader(response.GetResponseStream());
#if RELEASE
                    try
                    {
#endif
                    using (var reader = new JsonTextReader(streamReader))
                    {
                        output = jsonSerializer.Deserialize<T>(reader);
                    }
#if RELEASE
                    }
                    catch (Exception e)
                    {
                    }
                    finally
                    {
#endif
                    if (callBack != null)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(
                            () =>
                            {
                                callBack(output);
                            });
                    }
                    if (streamReader != null)
                    {
                        streamReader.Close();
                        streamReader.Dispose();
                        streamReader = null;
                    }
                    request = null;
                    requestResult = null;
                    if (response != null)
                    {
                        response.Close();
                        response.Dispose();
                        response = null;
                    }
#if RELEASE
                    }
#endif
                },
              request);
        }

        public static IDictionary<string, object> CheckSinceIdAndMaxId(IDictionary<string, object> parameters)
        {
            parameters = TwitterHelper.GetDictionary(parameters);
            if (parameters.ContainsKey(Const.SINCE_ID) ||
                parameters.ContainsKey(Const.MAX_ID))
            {
                parameters.Add(Const.COUNT, Const.DEFAULT_COUNT_VALUE_PLUS_ONE);
            }
            else
            {
                parameters.Add(Const.COUNT, Const.DEFAULT_COUNT_VALUE);
            }
            return parameters;
        }

        #endregion

        #region Home Page
        public void GetTweets<T>(Action<T> callBack, IDictionary<string, object> parameters = null)
        {
            parameters = CheckSinceIdAndMaxId(parameters);
            string url = TwitterHelper.GenerateUrlParams(Const.STATUSES_HOMETIMELINE, parameters);
            HandleWebRequest<T>(url, callBack);
        }

        public void GetMentions<T>(Action<T> callBack, IDictionary<string, object> parameters = null)
        {
            parameters = CheckSinceIdAndMaxId(parameters);
            string url = TwitterHelper.GenerateUrlParams(Const.STATUSES_MENTIONS_TIMELINE, parameters);
            HandleWebRequest<T>(url, callBack);
        }

        public void GetDirectMessages<T>(Action<T> callBack, IDictionary<string, object> parameters = null)
        {
            parameters = CheckSinceIdAndMaxId(parameters);
            parameters.Add(Const.DIRECT_MESSAGE_SKIP_STATUS, Const.DEFAULT_VALUE_TRUE);
            string url = TwitterHelper.GenerateUrlParams(Const.DIRECT_MESSAGES, parameters);
            HandleWebRequest<T>(url, callBack);
        }
        #endregion

        #region MyRegion


        public List<Tweet> GetOldTweets()
        {
            var reader = System.Windows.Application.GetResourceStream(new Uri("SampleData/hometimeline1.json", UriKind.Relative));
            StreamReader streamReader = new StreamReader(reader.Stream);
            string output = streamReader.ReadToEnd();
            var tweets = JsonConvert.DeserializeObject<List<Tweet>>(output);
            for (int i = 0; i < tweets.Count; i++)
            {
                tweets[i].Text = i + "";
            }
            return tweets;
        }

        public List<Tweet> GetNewMentions()
        {
            var reader = System.Windows.Application.GetResourceStream(new Uri("SampleData/mentions.json", UriKind.Relative));
            StreamReader streamReader = new StreamReader(reader.Stream);
            string output = streamReader.ReadToEnd();
            var tweets = JsonConvert.DeserializeObject<List<Tweet>>(output);
            for (int i = 0; i < tweets.Count; i++)
            {
                tweets[i].Text = i + " mentions";
            }
            return tweets;
        }

        public List<Tweet> GetOldMentions()
        {
            return GetNewMentions();
        }

        public List<Tweet> GetUserTweets(string userId)
        {
            var reader = System.Windows.Application.GetResourceStream(new Uri("SampleData/user_timeline.json", UriKind.Relative));
            StreamReader streamReader = new StreamReader(reader.Stream);
            string output = streamReader.ReadToEnd();
            var tweets = JsonConvert.DeserializeObject<List<Tweet>>(output);
            foreach (var tweet in tweets)
            {
                tweet.Text = tweets.Count + "User Tweet";
            }
            return tweets;
        }

        public List<Tweet> GetUserOldTweets(string userId)
        {
            var reader = System.Windows.Application.GetResourceStream(new Uri("SampleData/user_timeline.json", UriKind.Relative));
            StreamReader streamReader = new StreamReader(reader.Stream);
            string output = streamReader.ReadToEnd();
            var tweets = JsonConvert.DeserializeObject<List<Tweet>>(output);
            foreach (var tweet in tweets)
            {
                tweet.Text = tweets.Count + "User Old Tweet";
            }
            return tweets;
        }

        public List<DirectMessage> GetDirectMessages()
        {
            var reader = System.Windows.Application.GetResourceStream(new Uri("SampleData/direct_messages.json", UriKind.Relative));
            StreamReader streamReader = new StreamReader(reader.Stream);
            string output = streamReader.ReadToEnd();
            var messages = JsonConvert.DeserializeObject<List<DirectMessage>>(output);
            foreach (var message in messages)
            {
                message.Text = messages.Count + " message";
            }
            return messages;
        }

        public List<DirectMessage> GetOldDirectMessages()
        {
            return GetDirectMessages();
        }
        #endregion

        #region profile page
        public void GetUserProfileDetail<T>(string userId, Action<T> callBack, IDictionary<string, object> parameters = null)
        {
            parameters = TwitterHelper.GetDictionary(parameters);
            parameters.Add(Const.USER_ID, userId);
            string url = TwitterHelper.GenerateUrlParams(Const.USERS_SHOW, parameters);
            HandleWebRequest<T>(url, callBack);
        }

        public void GetUserTweets<T>(string userId, Action<T> callBack, IDictionary<string, object> parameters = null)
        {
            parameters = CheckSinceIdAndMaxId(parameters);
            parameters.Add(Const.USER_ID, userId);
            string url = TwitterHelper.GenerateUrlParams(Const.USER_TIMELINE, parameters);
            HandleWebRequest<T>(url, callBack);
        }

        public void GetFollowingIds<T>(string userId, Action<T> callBack, IDictionary<string, object> parameters = null)
        {
            parameters = TwitterHelper.GetDictionary(parameters);
            parameters.Add(Const.USER_ID, userId);
            parameters.Add(Const.COUNT, Const.DEFAULT_COUNT_VALUE);
            string url = TwitterHelper.GenerateUrlParams(Const.USER_FOLLOWING_IDS, parameters);
            HandleWebRequest<T>(url, callBack);
        }

        public void GetFollowerIds<T>(string userId, Action<T> callBack, IDictionary<string, object> parameters = null)
        {
            parameters = TwitterHelper.GetDictionary(parameters);
            parameters.Add(Const.USER_ID, userId);
            parameters.Add(Const.COUNT, Const.DEFAULT_COUNT_VALUE);
            string url = TwitterHelper.GenerateUrlParams(Const.USER_FOLLOWER_IDS, parameters);
            HandleWebRequest<T>(url, callBack);
        }

        public void GetUserProfiles<T>(string userIds, Action<T> callBack, IDictionary<string, object> parameters = null)
        {
            parameters = TwitterHelper.GetDictionary(parameters);
            parameters.Add(Const.USER_ID, userIds);
            parameters.Add(Const.COUNT, Const.DEFAULT_COUNT_VALUE);
            parameters.Add(Const.INCLUDE_ENTITIES, Const.DEFAULT_VALUE_FALSE);
            string url = TwitterHelper.GenerateUrlParams(Const.USERS_LOOKUP, parameters);
            HandleWebRequest<T>(url, callBack);
        }

        public void GetUserFavorites<T>(string userId, Action<T> callBack, IDictionary<string, object> parameters = null)
        {
            parameters = CheckSinceIdAndMaxId(parameters);
            parameters.Add(Const.USER_ID, userId);
            string url = TwitterHelper.GenerateUrlParams(Const.USER_FAVORITE, parameters);
            HandleWebRequest<T>(url, callBack);
        }
        #endregion

        #region Status Page
        public void GetStatusDetail<T>(string statusId, Action<T> callBack, IDictionary<string, object> parameters = null)
        {
            parameters = TwitterHelper.GetDictionary(parameters);
            parameters.Add(Const.ID, statusId);
            string url = TwitterHelper.GenerateUrlParams(Const.STATUSES_SHOW, parameters);
            HandleWebRequest<T>(url, callBack);
        }

        public void GetStatusRetweetIds<T>(string statusId, Action<T> callBack, IDictionary<string, object> parameters = null)
        {
            parameters = TwitterHelper.GetDictionary(parameters);
            parameters.Add(Const.ID, statusId);
            parameters.Add(Const.COUNT, Const.DEFAULT_COUNT_VALUE);
            string url = TwitterHelper.GenerateUrlParams(Const.STATUSES_RETWEET_IDS, parameters);
            HandleWebRequest<T>(url, callBack);
        }
        #endregion

        #region new tweet
        public void PostNewTweet<T>(string text, Action<T> callBack, IDictionary<string, object> parameters = null)
        {
            parameters = TwitterHelper.GetDictionary(parameters);
            parameters.Add(Const.STATUS, text);
            string url = TwitterHelper.GenerateUrlParams(Const.STATUS_POST_NEW_TWEET, parameters);
            HandleWebRequest<T>(url, callBack, Const.HTTPPOST);
        }
        #endregion
    }
}
