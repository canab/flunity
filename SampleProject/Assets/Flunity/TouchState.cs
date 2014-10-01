using UnityEngine;

namespace Flunity
{
	/// <summary>
	/// Struct holds data about touch.
	/// Unity's Touch struct doesn't allow to change its fields.
	/// </summary>
	public struct TouchState
    {
		public int id;
		public TouchPhase phase;
		public Vector2 position;

		public TouchState(int id, TouchPhase phase, Vector2 position)
		{
			this.id = id;
			this.phase = phase;
			this.position = position;
		}

	    public override string ToString()
	    {
	        return string.Format("TouchState[id: {0}, phase: {1}, position: {2}]", id, phase, position);
	    }
    }
}