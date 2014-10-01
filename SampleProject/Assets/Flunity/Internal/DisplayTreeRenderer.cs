namespace Flunity.Internal
{
	/// <summary>
	/// Walk through all DisplayTree and draw each item;
	/// </summary>
	internal class DisplayTreeRenderer
	{
		private int _drawOrderCounter;

		private readonly FlashStage _stage;
		private DisplayObject _currentItem;
		private DrawOptions _drawOptions;

		public DisplayTreeRenderer(FlashStage stage)
		{
			_stage = stage;
			_drawOptions = new DrawOptions();
		}

		internal void DrawStage()
		{
			_currentItem = _stage.root;
			_drawOrderCounter = 0;
			_drawOptions.Reset();

			var drawBatch = _stage.sceneBatch;

			while (_currentItem != null)
			{
				if (!_currentItem.visible)
				{
					GotoNextItem();
					continue;
				}
				
				_currentItem.drawOrder = _drawOrderCounter++;

				if (_currentItem.parent != null)
				{
					_currentItem.colorDirty = _currentItem.colorDirty || _currentItem.parent.colorDirty;
					_currentItem.transformDirty = _currentItem.transformDirty || _currentItem.parent.transformDirty;
				}

				if (_currentItem.colorDirty)
					_currentItem.UpdateColor();

				if (_currentItem.transformDirty)
					_currentItem.UpdateTransform();

				if (_currentItem.drawOptions != null)
					ApplyDrawOptions();

				if (_currentItem.drawRect != null)
					drawBatch.drawRect = _currentItem.drawRect;

				var container = _currentItem as DisplayContainer;
				if (container != null)
				{
					if (container.numChildren > 0)
						_currentItem = container.GetChildAt(0);
					else
						GotoNextItem();
				}
				else
				{
					_currentItem.Draw();
					GotoNextItem();
				}
			}

			drawBatch.Flush();
		}

		private void ApplyDrawOptions()
		{
			if (_currentItem.drawOptions == null)
				_drawOptions.Reset();
			else
				_drawOptions.CopyFrom(_currentItem.drawOptions);
			
			for (var item = _currentItem.parent; item != null; item = item.parent)
			{
				if (item.drawOptions != null)
					_drawOptions.MergeWith(item.drawOptions);
			}

			_stage.sceneBatch.ApplyOptions(_drawOptions);
		}

		private void GotoNextItem()
		{
			var drawOptionsWasSet = false;

			while (_currentItem != null)
			{
				if (_currentItem.drawRect != null)
					_stage.sceneBatch.drawRect = null;
				
				_currentItem.colorDirty = false;
				_currentItem.transformDirty = false;

				drawOptionsWasSet = drawOptionsWasSet || _currentItem.drawOptions != null;
				
				var nextNode = _currentItem.node.Next;
				if (nextNode != null)
				{
					_currentItem = nextNode.Value;
					break;
				}
				else
				{
					_currentItem = _currentItem.parent;
				}
			}

			if (_currentItem != null && _currentItem.drawOptions == null && drawOptionsWasSet)
				ApplyDrawOptions();
		}
	}
}