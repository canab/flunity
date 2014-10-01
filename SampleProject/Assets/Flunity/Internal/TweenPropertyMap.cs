using System.Collections.Generic;
using Flunity.Common;

namespace Flunity.Internal
{
	internal class TweenPropertyMap : Dictionary<ITweenProperty, TweenDataHolder>
	{
		public TweenPropertyMap(int capacity) : base(capacity)
		{}
	};
}