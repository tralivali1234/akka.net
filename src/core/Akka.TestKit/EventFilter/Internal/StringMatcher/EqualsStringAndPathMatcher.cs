﻿//-----------------------------------------------------------------------
// <copyright file="EqualsStringAndPathMatcher.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System;
using Akka.Actor;

namespace Akka.TestKit.Internal.StringMatcher
{
    /// <summary>
    /// <remarks>Note! Part of internal API. Breaking changes may occur without notice. Use at own risk.</remarks>
    /// </summary>
    public class EqualsStringAndPathMatcher : IStringMatcher
    {
        private readonly string _path;
        private readonly bool _canBeRelative;

        public EqualsStringAndPathMatcher(string path, bool canBeRelative=true)
        {
            _path = path;
            _canBeRelative = canBeRelative;
        }

        public bool IsMatch(string path)
        {
            if (String.Equals(_path, path, StringComparison.OrdinalIgnoreCase)) return true;
            if(!_canBeRelative)return false;

            ActorPath actorPath;
            if (!ActorPath.TryParse(path, out actorPath)) return false;
            var pathWithoutAddress = actorPath.ToStringWithoutAddress();
            return String.Equals(_path, pathWithoutAddress, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return "== \"" + _path + "\"";
        }
    }
}

