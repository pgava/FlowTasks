using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.Data.DAL
{
    internal class PlaceHolder
    {
        /// <summary>
        /// Regular expression for place holder
        /// </summary>
        private const string PLACEHOLDER_REGEX = @"^\{([a-z]+)\.([^\s]+)\}$";

        /// <summary>
        /// Replace Place Holder Parameter
        /// </summary>
        /// <param name="format">Format</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        public static string ReplacePlaceHolderParameter(string format, IEnumerable<PropertyInfo> parameters)
        {
            var result = format;
            var parameterList = parameters.ToList();

            foreach (var s in GetPlaceHolder(format))
            {
                var kv = PlaceHolderInfo(s);

                if (kv.Key != "p" && kv.Key != "param") continue;
                result = result.Replace(s, GetPropertyValueForPlaceHolder(kv.Value, parameterList, s));
            }

            return result;
        }

        /// <summary>
        /// Get User Role From PlaceHolder
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="user">User</param>
        /// <param name="role">Role</param>
        public static void GetUserRoleFromPlaceHolder(string input, out string user, out string role)
        {
            user = string.Empty;
            role = string.Empty;

            if (string.IsNullOrWhiteSpace(input)) return;

            var info = PlaceHolderInfo(input);

            if (string.IsNullOrWhiteSpace(info.Key))
            {
                user = input;
                return;
            }

            switch (info.Key)
            {
                case "u":
                case "user":
                    var u = info.Value;
                    if (!string.IsNullOrWhiteSpace(u)) user = u;
                    break;
                case "r":
                case "role":
                    var r = info.Value;
                    if (!string.IsNullOrWhiteSpace(r)) role = r;
                    break;
            }
        }

        /// <summary>
        /// Place Holder Info
        /// </summary>
        /// <param name="pl">Place Holder</param>
        /// <returns>Matching keyValue pair</returns>
        private static KeyValuePair<string, string> PlaceHolderInfo(string pl)
        {
            var match = Regex.Match(pl, PLACEHOLDER_REGEX, RegexOptions.IgnoreCase);

            if (!match.Success) return new KeyValuePair<string, string>();

            return new KeyValuePair<string, string>(
                match.Groups[1].ToString(), match.Groups[2].ToString());
        }

        /// <summary>
        /// Get Property Value For Place Holder
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="defaultResult">DefaultResult</param>
        /// <returns>string</returns>
        private static string GetPropertyValueForPlaceHolder(string name, IEnumerable<PropertyInfo> parameters, string defaultResult)
        {
            var res = defaultResult;
            foreach (var p in parameters)
            {
                if (p.Name == name)
                {
                    res = p.Value;
                    break;
                }
            }

            return res;
        }

        /// <summary>
        /// Get Place Holder
        /// </summary>
        /// <param name="format">Format</param>
        /// <returns>List of string</returns>
        private static IEnumerable<string> GetPlaceHolder(string format)
        {
            if (string.IsNullOrWhiteSpace(format)) yield return string.Empty;
            else
            {
                var matches = Regex.Matches(format,
                    @"\{[a-z]+\.[^\s]+\}", RegexOptions.IgnoreCase);

                foreach (Match match in matches)
                {
                    foreach (Capture capture in match.Captures)
                    {
                        yield return capture.Value;
                    }
                }
            }
        }
    }
}
