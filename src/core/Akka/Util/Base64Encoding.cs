﻿//-----------------------------------------------------------------------
// <copyright file="Base64Encoding.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System.Text;

namespace Akka.Util
{
    /// <summary>
    /// TBD
    /// </summary>
    public static class Base64Encoding
    {
        /// <summary>
        /// TBD
        /// </summary>
        public const string Base64Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789+~";

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="value">TBD</param>
        /// <returns>TBD</returns>
        public static string Base64Encode(this long value)
        {
            var sb = new StringBuilder();
            var next = value;
            do
            {
                var index = (int)(next & 63);
                sb.Append(Base64Chars[index]);
                next = next >> 6;
            } while(next != 0);
            return sb.ToString();
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="s">TBD</param>
        /// <returns>TBD</returns>
        public static string Base64Encode(this string s)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(s);
            return System.Convert.ToBase64String(bytes);
        }
    }
}

