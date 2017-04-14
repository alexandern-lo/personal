
using System;
using System.Collections.Generic;
using Android.Content;
using Android.Database;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace LiveOakApp.Droid.CustomViews
{
	public class ExpandedListView : AdapterView<BaseAdapter>
	{
		public ExpandedListView(Context context) :
			base(context)
		{
			Initialize();
		}

		public ExpandedListView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public ExpandedListView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

		void Initialize()
		{
		
		}

		BaseAdapter _adapter;

		public override BaseAdapter Adapter
		{
			get
			{
				return _adapter;
			}

			set
			{
				_adapter = value;

				_adapter.RegisterDataSetObserver(new ListDataSetObserver(this));

				children = new List<View>();
				RemoveAllViewsInLayout();
				SetupChildren();
				RequestLayout();

			}
		}

		View _footerView;

		public void AddFooterView(View v)
		{
			_footerView = v;
		}

		List<View> children;

		void SetupChildren()
		{
			int position = 0;
			while (position < _adapter.Count)
			{
				View child;
				if (position < children.Count)
				{
					child = _adapter.GetView(position, children[position], this);
				}
				else {
					child = _adapter.GetView(position, null, this);
					children.Add(child);
				}
				AddChild(child);
				position++;
			}

			if (_footerView != null)
			{
				AddChild(_footerView);
			}
		}

		void TrimChildren()
		{
            var itemsCount = _adapter.Count;
			var extraViewsCount =  children.Count - itemsCount;
			if (extraViewsCount > 0)
			{
                for (var i = 0; i < extraViewsCount; i++)
				{
                    children.RemoveAt(i);
				}
			}
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

			int height = 0;

			int width = MeasureSpec.GetSize(widthMeasureSpec);

			for (var i = 0; i < ChildCount; i++)
			{
				var view = GetChildAt(i);

				var widthSpec = view.LayoutParameters.Width == LayoutParams.MatchParent ? 
					    widthMeasureSpec :
					    MeasureSpec.MakeMeasureSpec(MeasureSpec.GetSize(widthMeasureSpec), MeasureSpecMode.AtMost);

                var heightSpec = view.LayoutParameters.Height == LayoutParams.WrapContent || view.LayoutParameters.Height == LayoutParams.MatchParent ?
                                     MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified) :
                                     MeasureSpec.MakeMeasureSpec(view.LayoutParameters.Height, MeasureSpecMode.Exactly);

                view.Measure(widthSpec, heightSpec);

				height += view.MeasuredHeight;
			}

			SetMeasuredDimension(width, height);

		}

		protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout(changed, left, top, right, bottom);

			int start = 0;

			for (int index = 0; index < ChildCount; index++)
			{

				View child = GetChildAt(index);

				int width = child.MeasuredWidth;
				int height = child.MeasuredHeight;

				var lp = child.LayoutParameters as ExpandedListView.LayoutParams;

				if (lp != null && lp.AlignRight)
				{
					
					child.Layout(Width - width, start, Width, start + height);

				}
				else
				{
					child.Layout(0, start, width, start + height);
					start += height;
				}
			}
		}

		void AddChild(View child)
		{
			var lp = child.LayoutParameters as ExpandedListView.LayoutParams;

			if (lp == null)
			{
				lp = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
				child.LayoutParameters = lp;
			}

			AddViewInLayout(child, -1, lp, true);
			
		}

		public override View SelectedView
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override void SetSelection(int position)
		{
			throw new NotImplementedException();
		}

		class ListDataSetObserver : DataSetObserver
		{
			ExpandedListView _view;

			public ListDataSetObserver(ExpandedListView view)
			{
				_view = view;
			}

			public override void OnInvalidated()
			{
				base.OnInvalidated();
				_view.Invalidate();
			}

			public override void OnChanged()
			{
				base.OnChanged();
				_view.TrimChildren();
				_view.RemoveAllViewsInLayout();
				_view.SetupChildren();
				_view.RequestLayout();
			}
		}

		new public class LayoutParams : ViewGroup.LayoutParams
		{
			public LayoutParams(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
			{
			}

			public LayoutParams(Context c, IAttributeSet attrs) : base(c, attrs)
			{
			}

			public LayoutParams(ViewGroup.LayoutParams source) : base(source)
			{
			}

			public LayoutParams(int width, int height) : base(width, height)
			{
			}

			public bool AlignRight { get; set; }
		}

	}
}

