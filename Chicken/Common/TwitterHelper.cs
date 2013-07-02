﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Chicken.Model.Entity;

namespace Chicken.Common
{
    public static class TwitterHelper
    {
        #region const
        public static string DATETIMEFORMAT = "yyyy-MM-dd  HH:mm:ss";

        private static Regex SourceRegex = new Regex(@".*>(?<url>[\s\S]+?)</a>");
        private static Regex SourceUrlRegex = new Regex(@"<a href=\""(?<link>[^\s>]+)\""");
        private static Regex UserNameRegex = new Regex(@"@(?<name>(_*[A-Za-z0-9]{1,15}_*)+)(?!(\.[A-Za-z]+))([^@A-Za-z]|$)");
        private static Regex HashTagRegex = new Regex(@"#(?<hashtag>\w+)([^\w]|$)");
        private const string URLPATTERN = @"(?<text>{0})([^/\w]|$)";
        #endregion

        #region parse tweet string
        public static string ParseToDateTime(this string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return string.Empty;
            }
            var year = date.Substring(25, 5);
            var month = date.Substring(4, 3);
            var day = date.Substring(8, 2);
            var time = date.Substring(11, 9).Trim();

            var dateTime = string.Format("{0}/{1}/{2} {3}", month, day, year, time);
            var result = DateTime.Parse(dateTime).ToLocalTime().ToString(DATETIMEFORMAT);
            return result;
        }

        public static string ParseToSource(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return Const.DEFAULTSOURCE;
            }
            string result = SourceRegex.Match(source).Groups["url"].Value;
            if (string.IsNullOrEmpty(result))
            {
                return source;
            }
            return result;
        }

        public static string ParseToSourceUrl(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return Const.DEFAULTSOURCEURL;
            }
            string result = SourceUrlRegex.Match(source).Groups["link"].Value;
            if (string.IsNullOrEmpty(result))
            {
                return Const.DEFAULTSOURCEURL;
            }
            return result;
        }

        public static IEnumerable<EntityBase> ParseUserMentions(string text)
        {
            var matches = UserNameRegex.Matches(text);
            foreach (Match match in matches)
            {
                var entity = new UserMention();
                entity.Index = match.Index;
                entity.DisplayName = match.Groups["name"].Value;
                yield return entity;
            }
        }

        public static IEnumerable<EntityBase> ParseHashTags(string text)
        {
            var matches = HashTagRegex.Matches(text);
            foreach (Match match in matches)
            {
                var entity = new HashTag();
                entity.Index = match.Index;
                entity.DisplayText = match.Groups["hashtag"].Value;
                yield return entity;
            }
        }

        public static IEnumerable<EntityBase> ParseUrls(string text, List<UrlEntity> urls)
        {
            foreach (var url in urls)
            {
                var matches = Regex.Matches(text, string.Format(URLPATTERN, Regex.Escape(url.Text)), RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    var m = new UrlEntity
                    {
                        Index = match.Index,
                        Text = url.Text,
                    };
                    yield return m;
                }
            }
        }

        public static IEnumerable<EntityBase> ParseMedias(string text, List<MediaEntity> medias)
        {
            foreach (var media in medias)
            {
                var matches = Regex.Matches(text, string.Format(URLPATTERN, Regex.Escape(media.Text)));
                foreach (Match match in matches)
                {
                    var m = new UrlEntity
                    {
                        Index = match.Index,
                        Text = media.Text,
                    };
                    yield return m;
                }
            }
        }
        #endregion

        #region generate url
        public static string GenerateUrlParams(string action, IDictionary<string, object> parameters = null)
        {
            StringBuilder sb = new StringBuilder(App.Settings.APISettings.Url).Append(action);
            if (parameters == null || parameters.Count == 0)
            {
                return sb.ToString();
            }
            sb.Append("?");
            foreach (var item in parameters)
            {
                sb.Append(item.Key).Append("=").Append(HttpUtility.UrlEncode(item.Value.ToString())).Append("&");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        /// <summary>
        /// for test api url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GenerateAPIParams(string url, IDictionary<string, object> parameters = null)
        {
            StringBuilder sb = new StringBuilder(url);
            if (parameters == null || parameters.Count == 0)
            {
                return sb.ToString();
            }
            sb.Append("?");
            foreach (var item in parameters)
            {
                sb.Append(item.Key).Append("=").Append(HttpUtility.UrlEncode(item.Value.ToString())).Append("&");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        #endregion

        public static IDictionary<string, object> GetDictionary(IDictionary<string, object> parameters = null)
        {
            if (parameters == null || parameters.Count == 0)
            {
                parameters = new Dictionary<string, object>();
            }
            return parameters;
        }

        #region Extension
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            Dictionary<TKey, object> keys = new Dictionary<TKey, object>();
            foreach (TSource element in source)
            {
                var elementValue = keySelector(element);
                if (!keys.ContainsKey(elementValue))
                {
                    keys.Add(elementValue, null);
                    yield return element;
                }
            }
        }
        #endregion
    }
}
