package actionlib.common.ui
{
	import actionlib.common.errors.EnumInstantiationError;

	public class Key
	{
		private static var _constructed:Boolean = false;
		private static var _map:Object = {};

		static public const NUMBER_0:Key = createEntry("0", 48);
		static public const NUMBER_1:Key = createEntry("1", 49);
		static public const NUMBER_2:Key = createEntry("2", 50);
		static public const NUMBER_3:Key = createEntry("3", 51);
		static public const NUMBER_4:Key = createEntry("4", 52);
		static public const NUMBER_5:Key = createEntry("5", 53);
		static public const NUMBER_6:Key = createEntry("6", 54);
		static public const NUMBER_7:Key = createEntry("7", 55);
		static public const NUMBER_8:Key = createEntry("8", 56);
		static public const NUMBER_9:Key = createEntry("9", 57);

		static public const A:Key = createEntry("A", 65);
		static public const B:Key = createEntry("B", 66);
		static public const C:Key = createEntry("C", 67);
		static public const D:Key = createEntry("D", 68);
		static public const E:Key = createEntry("E", 69);
		static public const F:Key = createEntry("F", 70);
		static public const G:Key = createEntry("G", 71);
		static public const H:Key = createEntry("H", 72);
		static public const I:Key = createEntry("I", 73);
		static public const J:Key = createEntry("J", 74);
		static public const K:Key = createEntry("K", 75);
		static public const L:Key = createEntry("L", 76);
		static public const M:Key = createEntry("M", 77);
		static public const N:Key = createEntry("N", 78);
		static public const O:Key = createEntry("O", 79);
		static public const P:Key = createEntry("P", 80);
		static public const Q:Key = createEntry("Q", 81);
		static public const R:Key = createEntry("R", 82);
		static public const S:Key = createEntry("S", 83);
		static public const T:Key = createEntry("T", 84);
		static public const U:Key = createEntry("U", 85);
		static public const V:Key = createEntry("V", 86);
		static public const W:Key = createEntry("W", 87);
		static public const X:Key = createEntry("X", 88);
		static public const Y:Key = createEntry("Y", 89);
		static public const Z:Key = createEntry("Z", 90);

		static public const SEMICOLON:Key = createEntry(";", 186);
		static public const EQUAL:Key = createEntry("=", 187);
		static public const COMMA:Key = createEntry(",", 188);
		static public const MINUS:Key = createEntry("-", 189);
		static public const PERIOD:Key = createEntry(".", 190);

		static public const SLASH:Key = createEntry("/", 191);
		static public const BACKQUOTE:Key = createEntry("`", 192);
		static public const LEFTBRACKET:Key = createEntry("[", 219);
		static public const BACKSLASH:Key = createEntry("\\", 220);
		static public const RIGHTBRACKET:Key = createEntry("]", 221);

		static public const QUOTE:Key = createEntry("'", 222);
		static public const ALTERNATE:Key = createEntry("Alt", 18);
		static public const BACKSPACE:Key = createEntry("Backspace", 8);
		static public const CAPS_LOCK:Key = createEntry("Caps", 20);
		static public const COMMAND:Key = createEntry("Cmd", 15);

		static public const CONTROL:Key = createEntry("Ctrl", 17);
		static public const DELETE:Key = createEntry("Del", 46);
		static public const DOWN:Key = createEntry("Down", 40);
		static public const END:Key = createEntry("End", 35);
		static public const ENTER:Key = createEntry("Enter", 13);

		static public const ESCAPE:Key = createEntry("Esc", 27);
		static public const F1:Key = createEntry("F1", 112);
		static public const F2:Key = createEntry("F2", 113);
		static public const F3:Key = createEntry("F3", 114);
		static public const F4:Key = createEntry("F4", 115);
		static public const F5:Key = createEntry("F5", 116);
		static public const F6:Key = createEntry("F6", 117);
		static public const F7:Key = createEntry("F7", 118);
		static public const F8:Key = createEntry("F8", 119);
		static public const F9:Key = createEntry("F9", 120);
		static public const F10:Key = createEntry("F10", 121);
		static public const F11:Key = createEntry("F11", 122);
		static public const F12:Key = createEntry("F12", 123);
		static public const F13:Key = createEntry("F13", 124);
		static public const F14:Key = createEntry("F14", 125);
		static public const F15:Key = createEntry("F15", 126);

		static public const HOME:Key = createEntry("Home", 36);
		static public const INSERT:Key = createEntry("Ins", 45);
		static public const LEFT:Key = createEntry("Left", 37);
		static public const NUMPAD:Key = createEntry("Num", 21);

		static public const NUMPAD_0:Key = createEntry("Num(0)", 96);
		static public const NUMPAD_1:Key = createEntry("Num(1)", 97);
		static public const NUMPAD_2:Key = createEntry("Num(2)", 98);
		static public const NUMPAD_3:Key = createEntry("Num(3)", 99);
		static public const NUMPAD_4:Key = createEntry("Num(4)", 100);
		static public const NUMPAD_5:Key = createEntry("Num(5)", 101);
		static public const NUMPAD_6:Key = createEntry("Num(6)", 102);
		static public const NUMPAD_7:Key = createEntry("Num(7)", 103);
		static public const NUMPAD_8:Key = createEntry("Num(8)", 104);
		static public const NUMPAD_9:Key = createEntry("Num(9)", 105);

		static public const NUMPAD_ADD:Key = createEntry("Num(+)", 107);
		static public const NUMPAD_DECIMAL:Key = createEntry("Num(.)", 110);
		static public const NUMPAD_DIVIDE:Key = createEntry("Num(/)", 111);
		static public const NUMPAD_ENTER:Key = createEntry("Num(Enter)", 108);
		static public const NUMPAD_MULTIPLY:Key = createEntry("Num(*)", 106);

		static public const NUMPAD_SUBTRACT:Key = createEntry("Num(-)", 109);
		static public const PAGE_DOWN:Key = createEntry("PgDown", 34);
		static public const PAGE_UP:Key = createEntry("PgUp", 33);
		static public const RIGHT:Key = createEntry("Right", 39);
		static public const SHIFT:Key = createEntry("Shift", 16);

		static public const SPACE:Key = createEntry("Space", 32);
		static public const TAB:Key = createEntry("Tab", 9);
		static public const UP:Key = createEntry("Up", 38);

		_constructed = true;

		public static function getByName(name:String):Key
		{
			return _map[name];
		}

		private static function createEntry(name:String, code:int):Key
		{
			var keyCode:Key = new Key(name, code);
			_map[name] = keyCode;
			return keyCode;
		}


		//-- instance --//


		private var _code:int;
		private var _name:String;

		public function Key(name:String, code:int)
		{
			if (_constructed)
				throw new EnumInstantiationError();

			_name = name;
			_code = code;
		}

		public function get isPressed():Boolean
		{
			return KeyboardManager.isKeyPressed(_code);
		}

		public function get code():int
		{
			return _code;
		}

		public function get name():String
		{
			return _name;
		}
	}
}
