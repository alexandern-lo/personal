using System;
using System.Reflection;
using System.Collections.Generic;

namespace StudioMobile
{
	public static partial class Inspection
	{
		public static object[] AllOutlets (this Foundation.NSObject obj)
		{
			List<object> outlets = new List<object> (); 
			var type = obj.GetType ();
			var members = type.GetMembers (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (var member in members) {
				var outletAttribute = member.GetCustomAttribute<Foundation.OutletAttribute> (true);
				if (outletAttribute != null) {
					var memberType = member.MemberType;
					if ((memberType & MemberTypes.Field) != 0) {
						outlets.Add (((FieldInfo)member).GetValue (obj));
					} else if ((memberType & MemberTypes.Property) != 0) {
						outlets.Add (((PropertyInfo)member).GetValue (obj));
					}
					break;
				}
			}
			return outlets.ToArray ();
		}


		public static bool IsOutlet (MemberInfo member)
		{
			var outletAttribute = member.GetCustomAttribute<Foundation.OutletAttribute> (true);
			return outletAttribute != null;
		}
	}
}

