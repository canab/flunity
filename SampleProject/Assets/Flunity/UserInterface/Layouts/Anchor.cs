using System;
using Flunity.Utils;

namespace Flunity.UserInterface.Layouts
{
	public class Anchor
	{
		public static readonly AnchorProperty X = new AnchorProperty(o => o.x, (o, f) => o.x = f);
		public static readonly AnchorProperty Y = new AnchorProperty(o => o.y, (o, f) => o.y = f);
		public static readonly AnchorProperty WIDTH = new AnchorProperty(o => o.width, (o, f) => o.width = f);
		public static readonly AnchorProperty HEIGHT = new AnchorProperty(o => o.height, (o, f) => o.height = f);
		public static readonly AnchorProperty LEFT = new AnchorProperty(o => o.left, (o, f) => o.left = f);
		public static readonly AnchorProperty TOP = new AnchorProperty(o => o.top, (o, f) => o.top = f);
		public static readonly AnchorProperty RIGHT = new AnchorProperty(o => o.right, (o, f) => o.right = f);
		public static readonly AnchorProperty BOTTOM = new AnchorProperty(o => o.bottom, (o, f) => o.bottom = f);

		private readonly DisplayObject _source;
		private readonly DisplayObject _target;

		private readonly AnchorProperty _sourceProp;
		private readonly AnchorProperty _targetProp;

		private readonly float _multiplier;
		private readonly bool _roundToInt;
		private readonly float _distance;

		public Anchor(DisplayObject source, AnchorProperty sourceProp,
			DisplayObject target, AnchorProperty targetProp,
			float multiplier = 1, bool roundToInt = true)
		{
			_source = source;
			_sourceProp = sourceProp;
			_target = target;
			_targetProp = targetProp;
			_multiplier = multiplier;
			_roundToInt = roundToInt;
			_distance = _sourceProp.getter(_source) * _multiplier - _targetProp.getter(_target);
		}

		public void apply()
		{
			var value = _sourceProp.getter(_source) * _multiplier - _distance;

			if (_roundToInt)
				value = value.RoundToInt();

			_targetProp.setter(_target, value);
		}
	}
}
