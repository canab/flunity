package actionlib.common.utils
{
	import flash.system.ApplicationDomain;
	import flash.utils.describeType;
	import flash.utils.getDefinitionByName;
	import flash.utils.getQualifiedClassName;

	public class ReflectUtil
	{
		public static function createInstance(className:String, args:Array = null):Object
		{
			return createInstanceFromDomain(ApplicationDomain.currentDomain, className,  args);
		}

		public static function createInstanceFromDomain(domain:ApplicationDomain, className:String, args:Array = null):Object
		{
			var classRef:Class = Class(domain.getDefinition(className));

			if (!args || args.length == 0)
				return new classRef();
			else if (args.length == 1)
				return new classRef(args[0]);
			else if (args.length == 2)
				return new classRef(args[0], args[1]);
			else if (args.length == 3)
				return new classRef(args[0], args[1], args[2]);
			else if (args.length == 4)
				return new classRef(args[0], args[1], args[2], args[3]);
			else if (args.length == 5)
				return new classRef(args[0], args[1], args[2], args[3], args[4]);
			else
				throw new ArgumentError("Incorrect number of arguments");
		}

		public static function getInstanceClass(instance:Object, domain:ApplicationDomain = null):Class
		{
			if (domain)
				return domain.getDefinition(getQualifiedClassName(instance)) as Class;
			else
				return getDefinitionByName(getQualifiedClassName(instance)) as Class;
		}
		
		public static function getFullName(object:Object):String
		{
			return getQualifiedClassName(object).replace("::", ".");
		}

		public static function getClassName(object:Object):String
		{
			var fullName:String = getQualifiedClassName(object);
			var index:int = fullName.indexOf("::");
			
			return (index >= 0)
				? fullName.substr(index + 2)
				: fullName;
		}

		public static function cloneObject(source:Object):Object
		{
			var constructor:Class = source.constructor;
			var result:Object = new constructor();
			copyFieldsAndProperties(source, result);
			return result;
		}

		public static function getFields(target:Object):Vector.<String>
		{
			var targetType:XML = describeType(target);
			var result:Vector.<String> = new <String>[];
			for each (var variable:XML in targetType.variable)
			{
				result.push(variable.@name);
			}
			return result;
		}

		public static function copyFieldsAndProperties(source:Object, target:Object):void
		{
			var targetType:XML = describeType(target);
			var value:*;

			if (targetType.@isDynamic == "true")
			{
				for (var propName:String in source)
				{
					target[propName] = source[propName];
				}
			}

			for each (var variable:XML in targetType.variable)
			{
				var memberName:String = variable.@name;

				if (source.hasOwnProperty(memberName))
				{
					value = source[memberName];

					if (!(value is Function))
						target[memberName] = value;
				}
			}

			for each (var accessor:XML in targetType.accessor)
			{
				var accessorName:String = accessor.@name;
				var accessType:String = accessor.@access;

				if (accessType == "readwrite" && target.hasOwnProperty(memberName))
				{
					value = source[accessorName];

					if (!(value is Function))
						target[accessorName] = value;
				}
			}
		}
	}

}