package flashexporter.rendering
{
	internal class StatRecord
	{
		public var key:String;
		public var totalBytes:uint = 0;
		public var framesRendered:uint = 0;
		public var framesReused:uint = 0;

		public function StatRecord(key:String)
		{
			this.key = key;
		}

		public function toString():String
		{
			var result:String = "" + sizeMB + " MB, " + framesRendered + " frames";

			if (framesReused > 0)
				result += " (" + framesReused + " reused)";

			result += " - " + key;

			return result;
		}

		public function add(record:StatRecord):void
		{
			framesRendered += record.framesRendered;
			framesReused += record.framesReused;
			totalBytes += record.totalBytes;
		}

		public function get sizeMB():Number
		{
			return Math.round(totalBytes / 1000) / 1000.0;
		}

		public function addReusedFrame():void
		{
			framesReused++;
		}

		public function addRenderedFrame(frame:BitmapFrame):void
		{
			framesRendered++;
			totalBytes += frame.dataSize;
		}
	}
}

