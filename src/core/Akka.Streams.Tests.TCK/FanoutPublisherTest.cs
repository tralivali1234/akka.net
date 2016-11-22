﻿//-----------------------------------------------------------------------
// <copyright file="FanoutPublisherTest.cs" company="Akka.NET Project">
//     Copyright (C) 2015-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Streams.Dsl;
using Reactive.Streams;

namespace Akka.Streams.Tests.TCK
{
    class FanoutPublisherTest : AkkaPublisherVerification<int>
    {
        public override IPublisher<int> CreatePublisher(long elements)
            => Source.From(Enumerate(elements == 0, elements)).RunWith(Sink.AsPublisher<int>(true), Materializer);
    }
}
