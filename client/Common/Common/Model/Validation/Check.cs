using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Linq;

namespace StudioMobile
{
	public struct Check<T>
	{
		public T Value { get; private set; }

		public string Key { get; private set; }

		public IErrorList Errors { get; private set; }

		public Check (T value, string key, IErrorList errors) : this ()
		{
			if (errors == null)
				throw new ArgumentNullException ("errors");
			Errors = errors;
			Value = value;
			Key = key ?? String.Empty;
		}

		public Check (T value, string key) : this (value, key, new ErrorList ())
		{			
		}

		public Check (T value) : this (value, null, new ErrorList ())
		{			
		}

		public void Fail (AppError error)
		{
			Check.Argument (error, "error").NotNull ();
			error.Key = Key;
			Errors.Add (error);
		}

		public static implicit operator bool(Check<T> check)
		{
			return check.IsValid;
		}

		public bool IsValid {
			get { return Errors.HasErrorsForKey(Key); }
		}
	}

	public abstract class AlwaysThrowErrorList : IErrorList
	{
		#pragma warning disable 0067
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged {
			add { throw new NotImplementedException (); }
			remove { throw new NotImplementedException (); }
		}
		#pragma warning restore 0067
		public abstract void Add (AppError error);

		public void Clear ()
		{				
		}

		public void Clear (string key)
		{				
		}

		public IEnumerable<AppError> ErrorsForKey (string key)
		{
			return Enumerable.Empty<AppError> ();
		}

		public System.Collections.IEnumerable GetErrors (string propertyName)
		{
			return Enumerable.Empty<AppError> ();
		}

		public bool IsEmpty {
			get { return true; }
		}

		public IEnumerable<AppError> Errors {
			get { return Enumerable.Empty<AppError> (); }
		}

		public bool HasErrors {
			get { return false; }
		}
	}

	public class ArgumentErrorList : AlwaysThrowErrorList
	{
		public static readonly ArgumentErrorList Instance = new ArgumentErrorList ();

		public override void Add (AppError error)
		{
			if (error is NullError) {
				throw new ArgumentNullException (error.Key);
			} else {
				throw new ArgumentException (error.Message, error.Key);
			}
		}
	}

	public class ValidationErrorList : AlwaysThrowErrorList
	{
		public static readonly ValidationErrorList Instance = new ValidationErrorList ();

		public override void Add (AppError error)
		{
			throw error;
		}
	}

	public class OperationErrorList : AlwaysThrowErrorList
	{
		public static readonly OperationErrorList Instance = new OperationErrorList ();

		public override void Add (AppError error)
		{
			throw new InvalidOperationException (error.Message);
		}
	}

	public static class Check
	{
		static Check ()
		{						
		}

		public static Check<T> Value<T> (T value, string key = null)
		{			
			return new Check<T> (value, key, ValidationErrorList.Instance);
		}

		public static Check<T> Argument<T> (T value, string key)
		{			
			return new Check<T> (value, key, ArgumentErrorList.Instance);
		}

		public static Check<T> State<T> (T value, [CallerMemberName] string key = null)
		{			
			return new Check<T> (value, key, OperationErrorList.Instance);
		}
	}
}