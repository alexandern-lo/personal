using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using SL4N;

namespace StudioMobile
{
	public interface IDataContext : INotifyPropertyChanged, INotifyDataErrorInfo	
	{
		IErrorList Errors { get; }

		bool Validate();
	}

	public static class DataContextExtensions 
	{
		public static bool HasErrorsForKey<T>(this T obj, string key)
			where T : IDataContext
		{
			return obj.Errors.HasErrorsForKey (key);
		}

		public static bool HasErrorsForKey<T, K>(this T obj, Expression<Func<T,K>> key)
			where T : IDataContext
		{
			return HasErrorsForKey (obj, PropertySupport.ExtractPropertyName (key));
		}
	}

    public class DataContext : IDataContext
	{
        ILogger _log;
        protected ILogger LOG { get { return _log ?? (_log = LoggerFactory.GetLogger(GetType().Name)); } }

        public DataContext ()
		{
			errors.ThrowImmediately = false;
		}

		protected void MakeStrict()
		{
			errors.ThrowImmediately = true;
		}

		readonly protected ErrorList errors = new ErrorList ();

		public IErrorList Errors { get { return errors; } }

		public bool IsStrict { get { return errors.ThrowImmediately; } }

		public bool Validating { get; private set; }

		public bool Validate ()
		{
			Validating = true;
			var properties = Inspection.InstanceProperties (GetType ());
			try {
				Errors.Clear();
				//trigger all properties to generate errors
				foreach (var p in properties) {
					if (!p.HasAttribute<IgnoreAttribute> () && p.CanWrite && p.CanRead) {
						var val = Inspection.GetValue (this, p);
						Inspection.SetValue (this, p, val);
					}
				}
				OnValidate();
			} finally {
				foreach (var p in properties) {
					if (Errors.HasErrorsForKey (p.Name)) {
						RaisePropertyChanged (p.Name);
					}
				}
				Validating = false;
			}
			return Errors.HasErrors;
		}

		protected virtual void OnValidate()
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged (PropertyChangedEventArgs key)
		{
			if (PropertyChanged != null) {
				PropertyChanged (this, key);
			}
		}

        protected void RaisePropertyChanged([CallerMemberName] string key = null)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(key));
        }

        protected void RaisePropertyChanged<T> (Expression<Func<T>> key)
		{
			RaisePropertyChanged (PropertySupport.ExtractPropertyName(key));
		}

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged {
			add { Errors.ErrorsChanged += value; }
			remove { Errors.ErrorsChanged -= value; }
		}

		public System.Collections.IEnumerable GetErrors (string propertyName)
		{
			return Errors.GetErrors (propertyName);
		}

		public bool HasErrors {
			get { return Errors.HasErrors; }
		}

		List<object> fields;
		List<object> Fields {
			get { 
				if (fields == null) {
					fields = new List<object> ();
				}
				return fields;
			}
		}

		protected Field<T> Value<T> (T value)
		{
			var memberField = new Field<T> (value, this);
			Fields.Add (memberField);
			return memberField;
		}

		protected Field<T> Value<T> ()
		{
			var memberField = new Field<T> (default(T), this);
			Fields.Add (memberField);
			return memberField;
		}

		public class Field<T>
		{
			protected T value;
			protected IKeyValueCoding kvc;
			protected readonly DataContext owner;

			internal Field (DataContext owner)
			{
				if (owner == null)
					throw new ArgumentNullException (nameof(owner));
				this.owner = owner;
			}

			internal Field (T value, DataContext owner) : this (owner)
			{
				Value = value;
			}

			public virtual T Value {
				get { return value; }
				set {					
					this.value = value;
					if (value is ValueType) {
						kvc = KeyValueCoding.Impl (value);
					} else {						
						kvc = ReferenceEquals (null, value) ? null : KeyValueCoding.Impl (value);
					}
				}
			}

			public virtual Value Get<Value> ([CallerMemberName] string key = null)
			{
				if (!kvc.ContainsKey (value, key)) {
					throw new KeyNotFoundException ();
				}
				return DoGet<Value> (key);
			}

			protected virtual Value DoGet<Value>(string key)
			{
				return (Value)kvc.Get (value, key);
			}

			public void SetValue (T newValue, bool raiseEvent = true, [CallerMemberName] string key = null)
			{
				if (owner.Validating)
					return;
				Value = newValue;
				if (raiseEvent)
					owner.RaisePropertyChanged (new PropertyChangedEventArgs (key));
			}

			public void Set<Value> (Expression<Func<T,Value>> property, Value newValue, bool raiseEvent = true)
			{
				Set<Value> (newValue, raiseEvent, PropertySupport.ExtractPropertyName (property));
			}

			public void Set<Value> (Value newValue, bool raiseEvent = true, [CallerMemberName] string key = null)
			{			
				if (owner.Validating)
					return;
				if (!kvc.ContainsKey (value, key)) {
					throw new KeyNotFoundException ();
				}
				DoSetValue (newValue, key);
				if (raiseEvent)
					owner.RaisePropertyChanged (new PropertyChangedEventArgs (key));
			}

			protected virtual void DoSetValue<Value>(Value newValue, string key)
			{
				kvc.Set (value, key, newValue);
			}

			public static implicit operator T(Field<T> field)
			{
				return field.Value;
			}
		}

        BindingList bindings;
        public BindingList Bindings
        {
            get
            {
                if (bindings == null)
                {
                    bindings = new BindingList();
                }
                return bindings;
            }
            set
            {
                Check.Argument(value, "value").NotNull();
                bindings = value;
            }
        }
    }

}
