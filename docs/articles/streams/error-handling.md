---
layout: docs.hbs
title: Error Handling
---

Strategies for how to handle exceptions from processing stream elements can be defined when
materializing the stream. The error handling strategies are inspired by actor supervision
strategies, but the semantics have been adapted to the domain of stream processing.

> [!WARNING]
> *ZipWith*, *GraphStage* junction, *ActorPublisher* source and *ActorSubscriber* sink 
components do not honour the supervision strategy attribute yet.

#Supervision Strategies

There are three ways to handle exceptions from application code:

* ``Stop`` - The stream is completed with failure.
* ``Resume`` - The element is dropped and the stream continues.
* ``Restart`` - The element is dropped and the stream continues after restarting the stage.
  Restarting a stage means that any accumulated state is cleared. This is typically
  performed by creating a new instance of the stage.

By default the stopping strategy is used for all exceptions, i.e. the stream will be completed with
failure when an exception is thrown.

```csharp
var source = Source.From(Enumerable.Range(0, 6)).Select(x => 100/x);
var result = source.RunWith(Sink.Aggregate<int, int>(0, (sum, i) => sum + i), materializer);
// division by zero will fail the stream and the
// result here will be a Task completed with Failure(DivideByZeroException)
```

The default supervision strategy for a stream can be defined on the settings of the materializer.

```csharp
Decider decider = cause => cause is DivideByZeroException
    ? Directive.Resume
    : Directive.Stop;
var settings = ActorMaterializerSettings.Create(system).WithSupervisionStrategy(decider);
var materializer = system.Materializer(settings);

var source = Source.From(Enumerable.Range(0, 6)).Select(x => 100/x);
var result = source.RunWith(Sink.Aggregate<int, int>(0, (sum, i) => sum + i), materializer);
// the element causing division by zero will be dropped
// result here will be a Task completed with Success(228)
```

Here you can see that all ``DivideByZeroException`` will resume the processing, i.e. the
elements that cause the division by zero are effectively dropped.

> [!NOTE]
> Be aware that dropping elements may result in deadlocks in graphs with cycles, as explained in [Graph cycles, liveness and deadlocks](workingwithgraphs#graph-cycles-liveness-and-deadlocks).

The supervision strategy can also be defined for all operators of a flow.

```csharp
Decider decider = cause => cause is DivideByZeroException
    ? Directive.Resume
    : Directive.Stop;

var flow = Flow.Create<int>()
    .Where(x => 100/x < 50)
    .Select(x => 100/(5 - x))
    .WithAttributes(ActorAttributes.CreateSupervisionStrategy(decider));
var source = Source.From(Enumerable.Range(0, 6)).Via(flow);
var result = source.RunWith(Sink.Aggregate<int, int>(0, (sum, i) => sum + i), materializer);
// the elements causing division by zero will be dropped
// result here will be a Future completed with Success(150)
```

``Restart`` works in a similar way as ``Resume`` with the addition that accumulated state,
if any, of the failing processing stage will be reset.

```csharp
Decider decider = cause => cause is ArgumentException
    ? Directive.Restart
    : Directive.Stop;

var flow = Flow.Create<int>()
    .Scan(0, (acc, x) =>
    {
      if(x < 0)
            throw new ArgumentException("negative not allowed");
        return acc + x;
    })
    .WithAttributes(ActorAttributes.CreateSupervisionStrategy(decider));
var source = Source.From(new [] {1,3,-1,5,7}).Via(flow);
var result = source.Limit(1000).RunWith(Sink.Seq<int>(), materializer);
// the negative element cause the scan stage to be restarted,
// i.e. start from 0 again
// result here will be a Task completed with Success(List(0, 1, 4, 0, 5, 12))
```

#Errors from SelectAsync
Stream supervision can also be applied to the tasks of ``SelectAsync``.

Let's say that we use an external service to lookup email addresses and we would like to
discard those that cannot be found.

We start with the tweet stream of authors:

```csharp
var authors = tweets
    .Where(t => t.HashTags.Contains("Akka.Net"))
    .Select(t => t.Author);
```

Assume that we can lookup their email address using:

```csharp
Task<string> LookupEmail(string handle)
```

The ``Task`` is completed with ``Failure`` if the email is not found.

Transforming the stream of authors to a stream of email addresses by using the ``LookupEmail``
service can be done with ``SelectAsync`` and we use ``Deciders.ResumingDecider`` to drop
unknown email addresses:

```csharp
var emailAddresses = authors.Via(
    Flow.Create<Author>()
        .SelectAsync(4, author => AddressSystem.LookupEmail(author.Handle))
        .WithAttributes(ActorAttributes.CreateSupervisionStrategy(Deciders.ResumingDecider)));
```

If we would not use ``Resume`` the default stopping strategy would complete the stream
with failure on the first ``Task`` that was completed with ``Failure``.
