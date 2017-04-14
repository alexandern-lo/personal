using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace StudioMobile
{
	public static partial class Inspection
	{		
		public static object GetValue (object container, MemberInfo member)
		{
			if (container == null)
				throw new ArgumentNullException ("container");
			if (member == null)
				throw new ArgumentNullException ("member");
			var memberType = member.MemberType;
			if ((memberType & MemberTypes.Field) != 0) {
				return ((FieldInfo)member).GetValue (container);
			} else if ((memberType & MemberTypes.Property) != 0) {
				return ((PropertyInfo)member).GetValue (container);
			}
			throw new ArgumentException ("member");
		}

		public static void SetValue (object container, MemberInfo member, object value)
		{
			if (container == null)
				throw new ArgumentNullException ("container");
			if (member == null)
				throw new ArgumentNullException ("member");
			var memberType = member.MemberType;
			if ((memberType & MemberTypes.Field) != 0) {
				((FieldInfo)member).SetValue (container, value);
			} else if ((memberType & MemberTypes.Property) != 0) {
				((PropertyInfo)member).SetValue (container, value);
			} else {
				throw new ArgumentException ("member");
			}
		}

		public static Type GetMemberType(MemberInfo member)
		{
			if (member == null)
				throw new ArgumentNullException ("member");
			var memberType = member.MemberType;
			if ((memberType & MemberTypes.Field) != 0) {
				var fieldInfo = member as FieldInfo;
				return fieldInfo.FieldType;
			} else if ((memberType & MemberTypes.Property) != 0) {
				var propertyInfo = member as PropertyInfo;
				return  propertyInfo.PropertyType;
			}
			throw new ArgumentException ("member");
		}

		public static bool HasAttribute<T>(this MemberInfo member) where T : Attribute
		{
			return member.HasAttribute (typeof(T));
		}

		public static bool HasAttribute(this MemberInfo member, Type attributeType)
		{
			return Inspection.Attributes (member, attributeType).DefaultIfEmpty (null).First () != null;
		}

		public static IEnumerable<T> Attributes<T>(this MemberInfo member) where T : Attribute
		{
			return Attributes(member, typeof(T)).Cast<T>();
		}

		public static IEnumerable<Attribute> Attributes(this MemberInfo member, Type attributeType)
		{
			if (member == null)
				throw new ArgumentNullException ("member");
			return member.GetCustomAttributes().Where (a => a.GetType().IsSubclassOf(attributeType));
		}

		public static PropertyInfo[] InstanceProperties(Type type)
		{
			if (type == null)
				throw new ArgumentNullException ("type");
			return type.GetProperties (BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
		}

		public static IEnumerable<Type> ParentHierarchy(this Type root)
		{
			if (root == null)
				yield break; 
			yield return root;
			foreach (var intf in root.GetInterfaces()) {
				foreach (var t in intf.ParentHierarchy())
					yield return t;
			}
			foreach (var t in root.BaseType.ParentHierarchy())
				yield return t;
		}
	}
}
