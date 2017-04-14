using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace StudioMobile
{
	public interface IEditableDataContext : IDataContext, IEditableObject
	{
		bool Editing { get; }
	}

	public class EditableDataContext : DataContext, IEditableDataContext
	{
		public bool Editing { get; private set; }

		public void BeginEdit ()
		{			
			OnBeginEdit ();
			Editing = true;
			editableFields.ForEach (_ => _.BeginEdit ());
		}

		protected virtual void OnBeginEdit()
		{
		}

		public void CancelEdit ()
		{
			OnCancelEdit ();
			editableFields.ForEach (_ => _.CancelEdit ());
			Editing = false;
		}

		protected virtual void OnCancelEdit()
		{
		}

		public void EndEdit ()
		{
			OnEndEdit ();
			editableFields.ForEach (_ => _.EndEdit ());
			Editing = false;
		}

		protected virtual void OnEndEdit()
		{
		}

		List<IEditableObject> editableFields;
		List<IEditableObject> EditableFields {
			get { 
				if (editableFields == null) {
					editableFields = new List<IEditableObject> ();
				}
				return editableFields;
			}
		}

		protected EditableField<T> EditableValue<T> (T value)
		{
			var memberField = new EditableField<T> (value, this);
			EditableFields.Add (memberField);
			return memberField;
		}

		protected EditableField<T> EditableValue<T> ()
		{
			var memberField = new EditableField<T> (default(T), this);
			EditableFields.Add (memberField);
			return memberField;
		}

		public class EditableField<T> : Field<T>, IEditableObject
		{
			T original;
			Dictionary<string, object> editing;

			internal EditableField (EditableDataContext owner) : base(owner)
			{
			}

			internal EditableField (T value, EditableDataContext owner) : base (value, owner)
			{
			}

			public override T Value {
				get { return base.Value; }
				set { 
					if (IsEditing) {
						editing.Clear ();
					} 
					base.Value = value;
				}
			}

			public bool IsEditing { get { return editing != null; } }

			public void BeginEdit ()
			{
				if (IsEditing)
					return;
				editing = new Dictionary<string, object> ();
				original = value;
			}

			public void CancelEdit ()
			{	
				if (!IsEditing)
					return;
				editing = null;
				value = original;
			}

			public void EndEdit ()
			{
				if (!IsEditing)
					return;
				try {
					foreach (var kv in editing) {
						kvc.Set (value, kv.Key, kv.Value);
					}
				} finally {
					editing = null;
					original = default(T);
				}
			}

			protected override Value DoGet<Value>(string key)
			{
				if (IsEditing) {
					object propertyValue;
					if (editing.TryGetValue (key, out propertyValue)) {
						return (Value)propertyValue;
					}
				}
				return base.DoGet<Value> (key);
			}

			protected override void DoSetValue<Value>(Value newValue, string key)
			{
				if (IsEditing) {
					editing [key] = newValue;
				} else {
					base.DoSetValue (newValue, key);
				}
			}
			public static implicit operator T(EditableField<T> field)
			{
				return field.Value;
			}
		}
	}

	public static class MemberFieldExtensions 
	{
		public static void BeginListEdit<T>(this EditableDataContext.EditableField<List<T>> field)
		{
			var list = field.Value;
			var copy = new List<T> (list ?? Enumerable.Empty<T>());
			field.BeginEdit ();
			field.Value = copy;
		}
	}

}

