using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SL4N;

#if __IOS__
using View = UIKit.UIView;
using ContainerView = UIKit.UIView;
#endif

#if __ANDROID__
using View = Android.Views.View;
using ContainerView = Android.Views.ViewGroup;
#endif


namespace StudioMobile
{
	//TODO clients of this class has to maintain List<object> of all objects built by ViewBuilder
	//to be able to dispose them later on. Maybe it is better to host ViewBuilder objects which control
	//created views instead of having List<object> in each client view?
	public partial class ViewBuilder
	{
		static readonly ILogger LOG = LoggerFactory.GetLogger<ViewBuilder>();

		public static List<object> Build(ContainerView view)
		{
			var builtObjects = new List<object>();
			var viewType = view.GetType();
			var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
			var properties = from property in viewType.GetProperties(bindingFlags)
							 let viewAttr = property.GetCustomAttribute<ViewAttribute>()
							 orderby viewAttr != null ? viewAttr.Order : int.MaxValue
							 select property;

			foreach (var property in properties)
			{
				var obj = Build(view, property);
				if (obj != null)
				{
					builtObjects.Add(obj);
				}
			}
			return builtObjects;
		}

		static object Build(ContainerView view, MemberInfo member)
		{
			View subview = null;
			var viewAttribute = member.GetCustomAttribute<ViewAttribute>();
			if (viewAttribute != null)
			{
				subview = InstantiateView(view, member);
			}
			var decorator = member.GetCustomAttribute<DecoratorAttribute>();
			if (decorator != null)
			{
				subview = subview ?? Inspection.GetValue(view, member) as View;
				if (subview != null)
				{
					decorator.Decorate(subview);
				}
				else {
					throw new InvalidOperationException(String.Format("Cannot decorate {0}.{1}", member.DeclaringType.Name, member.Name));
				}
			}
			if (subview != null)
			{
				AddSubview(view, subview);
			}
			return subview;
		}


		static View InstantiateView(View view, MemberInfo member)
		{
			View subview = Inspection.GetValue(view, member) as View;
			if (subview == null)
			{
				var outletType = Inspection.GetMemberType(member);
				subview = CreateView(view, outletType);

				Inspection.SetValue(view, member, subview);
			}
			return subview;
		}
	}

	public class ViewAttribute : Attribute
	{
		public ViewAttribute(int order = int.MaxValue)
		{
			Order = order;
		}
		public int Order { get; private set; }
	}

	public class DecoratorAttribute : Attribute
	{
		static readonly ILogger LOG = LoggerFactory.GetLogger<DecoratorAttribute>();
		MethodInfo[] methods;
		readonly string[] methodNames;
		readonly Type type;

		public DecoratorAttribute(Type type, params string[] methods)
		{
			methodNames = methods;
			this.type = type;
		}

		public void Decorate(View view)
		{
			if (methods == null)
			{
				var argTypes = new[] { view.GetType() };
				methods = methodNames.Select(methodName =>
				{
					var method = type.GetMethod(
									 methodName,
									 BindingFlags.Static | BindingFlags.Public,
									 Type.DefaultBinder,
									 argTypes,
									 null);
					if (method == null)
					{
						LOG.Error("Decorator method not found {0} {1}", type, methodName);
					}
					return method;
				}).ToArray();
			}

			var args = new[] { view };
			foreach (var method in methods)
			{
				if (method != null)
				{
					method.Invoke(null, args);
				}
			}
		}
	}
}

