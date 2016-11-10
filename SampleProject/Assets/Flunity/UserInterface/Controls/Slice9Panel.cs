using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flunity.UserInterface.Controls
{
	public class Slice9Panel : ControlBase
	{
		private static readonly int[] _rightIndexes = { 0, 3, 6 };
		private static readonly int[] _leftIndexes = { 2, 5, 8 };
		private static readonly int[] _topIndexes = { 0, 1, 2 };
		private static readonly int[] _bottomIndexes = { 6, 7, 8 };

		private static readonly List<int[]> _templates = new List<int[]>
		{
			new[] // #########
			{
				0, 1, 2,
				3, 4, 5,
				6, 7, 8,
			},

			new[] // ####_####
			{
				0, 1, 2,
				3, 9, 5,
				6, 7, 8,
			},

			new[] // ##_#_____
			{
				0, 1, 0,
				3, 9, 3,
				0, 1, 0,
			},

			new[] // #__##____
			{
				0, 3, 0,
				3, 4, 3,
				0, 3, 0,
			},

			new[] // ##_##____
			{
				0, 1, 0,
				3, 4, 3,
				0, 1, 0,
			},

			new[] // ##__#_##_
			{
				0, 1, 0,
				4, 4, 4,
				6, 7, 6,
			},

			new[] // ##_##_##_
			{
				0, 1, 0,
				3, 4, 3,
				6, 7, 6,
			},

			new[] // ###_#____
			{
				0, 1, 0,
				3, 4, 3,
				6, 7, 6,
			},
			
		};

		private readonly SpriteResource _resource;

		private readonly FlashSprite _tl;
		private readonly FlashSprite _tc;
		private readonly FlashSprite _tr;
		private readonly FlashSprite _ml;
		private readonly FlashSprite _mc;
		private readonly FlashSprite _mr;
		private readonly FlashSprite _bl;
		private readonly FlashSprite _bc;
		private readonly FlashSprite _br;

		public Slice9Panel(DisplayContainer parent, SpriteResource resource)
			:this(resource)
		{
			this.parent = parent;
		}

		public Slice9Panel(SpriteResource resource)
		{
			_resource = resource;

			var template = findCurrentTemplate();

			_tl = createPart(0, template[0]);
			_tc = createPart(1, template[1]);
			_tr = createPart(2, template[2]);
			_ml = createPart(3, template[3]);
			_mc = createPart(4, template[4]);
			_mr = createPart(5, template[5]);
			_bl = createPart(6, template[6]);
			_bc = createPart(7, template[7]);
			_br = createPart(8, template[8]);

			updateMinSize();
		}

		private int[] findCurrentTemplate()
		{
			int[] currentTemplate = null;
			int currentTemplateUniqueParts = 0;
			
			foreach (var template in _templates)
			{
				int compareCount = 0;
				int uniquePartCount = 0;
				for (int i = 0; i < template.Length; i++)
				{
					int partIndex = template[i];
					if (partIndex < template.Length && _resource.frames[partIndex].isEmpty)
						break;
					else
						compareCount++;

					if(partIndex == i)
						uniquePartCount++;
				}
				if (compareCount == template.Length && currentTemplateUniqueParts < uniquePartCount)
				{
					currentTemplate = template;
					currentTemplateUniqueParts = uniquePartCount;
				}
			}
			return currentTemplate;
		}

		private FlashSprite createPart(int partIndex, int frameIndex)
		{
			if (frameIndex < 0 || frameIndex >= _resource.frames.Length)
				return null;

			var part = new FlashSprite(_resource);
			part.currentFrame = frameIndex;
			part.parent = this;

			if (frameIndex != partIndex)
				applyFlip(frameIndex, partIndex, part);

			return part;
		}

		private void applyFlip(int frameIndex,int framePlaceIndex, FlashSprite part)
		{
			if (_leftIndexes.Contains(framePlaceIndex) && _rightIndexes.Contains(frameIndex))
				part.flipHorizontal = true;
			
			if (_bottomIndexes.Contains(framePlaceIndex) && _topIndexes.Contains(frameIndex))
				part.flipVertical = true;
		}

		protected override void ApplyLayout()
		{
			var w = width;
			var h = height;

			_tc.x = _tl.width;
			_tc.width = w - _tl.width - _tr.width;
			
			_tr.x = w - _tr.width;
			
			_ml.y = _tl.height;
			_ml.height = h - _tl.height - _bl.height;

			if (_mc != null)
			{
				_mc.position = new Vector2(_ml.width, _ml.y);
				_mc.width = w - _ml.width - _mr.width;
				_mc.height = h - _tc.height - _bc.height;
			}
			
			_mr.x = w - _mr.width;
			_mr.y = _tr.height;
			_mr.height = h - _tr.height - _br.height;
			
			_bl.y = h - _bl.height;

			_bc.x = _bl.width;
			_bc.y = h - _bc.height;
			_bc.width = w - _bl.width - _br.width;
			
			_br.x = w - _br.width;
			_br.y = h - _br.height;
		}

		private void updateMinSize()
		{
			minSize = new Vector2(
				_tl.width + _tc.width + _tr.width,
				_tl.height + _ml.height + _bl.height);
		}

		public float topPartHeight
		{
			get { return _bl.height; }
			set
			{
				_tl.height = value;
				_tc.height = value;
				_tr.height = value;

				updateMinSize();
				InvalidateLayout();
			}
		}
		
		public float bottomPartHeight
		{
			get { return _bl.height; }
			set
			{
				_bl.height = value;
				_bc.height = value;
				_br.height = value;

				updateMinSize();
				InvalidateLayout();
			}
		}
	}
}