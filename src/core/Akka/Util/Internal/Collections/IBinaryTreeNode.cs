﻿//-----------------------------------------------------------------------
// <copyright file="IBinaryTreeNode.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

namespace Akka.Util.Internal.Collections
{
	public interface IBinaryTreeNode<out TKey, out TValue>:IKeyValuePair<TKey,TValue>
	{
		IBinaryTreeNode<TKey, TValue> Left { get; }
		IBinaryTreeNode<TKey, TValue> Right { get; }
	}
}

