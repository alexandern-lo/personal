using System;

namespace StudioMobile
{
	public class AppError : ApplicationException
	{
		public AppError ()
		{
		}

		public AppError (string message, bool useFormat = true)
		{
			if (useFormat) {
				MessageFormat = message;
			}
		}

		public AppError (string message, Exception innerException, bool useFormat = true) : base (useFormat ? null : message, innerException)
		{
			if (useFormat) {
				MessageFormat = message;
			}
		}

		public AppError (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}

		string key = String.Empty;

		public string Key { 
			get { return key; }
			set { 
				if (key == null)
					throw new ArgumentNullException ("value");
				key = value;
			}
		}

		public string MessageFormat { get; set; }

		public override string Message { 
			get { 
				if (MessageFormat == null)
					return base.Message;
				return MessageFormat.FormatWithObject (this); 
			} 
		}

		public override string ToString ()
		{
			return Message;
		}
	}

	public class ValueError : AppError
	{
		protected object value;

		public virtual object Value {
			get { return value; } 
			set { SetValue (value); }
		}

		protected virtual void SetValue (object value)
		{
			this.value = value;
		}
	}

	public class ValueError<T> : ValueError
	{
		public new T Value { get; set; }

		protected override void SetValue (object value)
		{
			if (!(value is T)) {
				throw new InvalidCastException ();
			}
			this.value = value;
		}
	}
}