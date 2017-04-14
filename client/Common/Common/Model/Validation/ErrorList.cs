using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;

namespace StudioMobile
{
	public interface IErrorList : INotifyDataErrorInfo
	{
		bool IsEmpty { get; }

		void Add (AppError error);

		void Clear ();

		void Clear (string key);

		IEnumerable<AppError> Errors { get; }

		IEnumerable<AppError> ErrorsForKey (string key);
	}

	public static class ErrorListExtensions
	{
		public static IEnumerable<AppError> ErrorsForKey<T> (this IErrorList list, Expression<Func<T>> expr)
		{
			var key = PropertySupport.ExtractPropertyName (expr);
			return list.ErrorsForKey (key);
		}

		public static bool HasErrorsForKey (this IErrorList list, [CallerMemberName] string key = null)
		{
			key = key ?? String.Empty;
			return list.Errors.Any (e => e.Key == key);
		}

		public static bool HasErrorsForKey<T> (this IErrorList list, Expression<Func<T>> expr)
		{
			var key = PropertySupport.ExtractPropertyName (expr);
			return list.HasErrorsForKey (key);
		}

		public static Check<T> CheckValue<T> (this IErrorList result, T value, [CallerMemberName] string key = null)
		{
			if (result == null)
				throw new ArgumentNullException ("result");
			result.Clear (key);
			return new Check<T> (value, key, result);
		}
			
		public static readonly RuntimeEvent ErrorsChangedEvent = new RuntimeEvent (typeof(IErrorList), "ErrorsChanged");
		public static readonly IPropertyBindingStrategy ErrorsChangedBindingStrategy = new EventHandlerBindingStrategy (ErrorsChangedEvent);

		public static Property<IEnumerable<AppError>> Errors<T> (this T list)
			where T : IErrorList
		{
            return list.GetProperty(_ => _.Errors, ErrorsChangedBindingStrategy, () => list.Errors);
		}
	}

	public class ErrorList : IErrorList
	{
		List<AppError> errors = new List<AppError> ();

		public bool IsEmpty { get { return errors.Count == 0; } }

		public bool HasErrors { get { return errors.Count > 0; } }

		public void Add (AppError error)
		{			
			if (error == null)
				throw new ArgumentNullException ("error");
			if (ThrowImmediately) {
				throw error;
			}
			errors.Add (error);
			if (ErrorsChanged != null) {				
				ErrorsChanged (this, new DataErrorsChangedEventArgs (error.Key));
			}
		}

		public void Clear ()
		{
			var keys = errors.Select (e => e.Key).Distinct ();
			errors.Clear ();
			if (ErrorsChanged != null) {
				foreach (var key in keys) {
					ErrorsChanged (this, new DataErrorsChangedEventArgs (key));
				}
			}
		}

		public void Clear (string key)
		{
			key = key ?? String.Empty;
			errors = errors.FindAll (e => e.Key != key);
			if (ErrorsChanged != null) {
				ErrorsChanged (this, new DataErrorsChangedEventArgs (key));
			}
		}

		public IEnumerable<AppError> Errors { get { return errors.AsReadOnly (); } }

		public IEnumerable<AppError> ErrorsForKey (string key)
		{
			key = key ?? String.Empty;
			return errors.FindAll (e => e.Key == key);
		}

		public bool ThrowImmediately { get; set; }

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public IEnumerable GetErrors (string key)
		{
			if (string.IsNullOrEmpty (key)) {
				return Errors;
			} else {
				return ErrorsForKey (key);
			}
		}
	}

	public static class DataErrorsChangedEventArgsExtensions
	{
		public static bool IsForKey<T> (this DataErrorsChangedEventArgs args, Expression<Func<T>> prop)
		{
			var key = PropertySupport.ExtractPropertyName (prop);
			return args.PropertyName == key;
		}

		public static bool IsForKey (this DataErrorsChangedEventArgs args, string key)
		{
			return args.PropertyName == key;
		}
	}
}