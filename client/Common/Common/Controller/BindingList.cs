using System.Collections.Generic;
using System.Linq;

namespace StudioMobile
{
	/// <summary>
	/// Collection of bindings, central place to manage bindings for a particular view. 
	/// BindingList primary purpose is to avoid memory leaks - when view removed bindings list 
	/// can unbind all bindings at once.
	/// </summary>
	public class BindingList : IBinding
	{
		readonly List<IBinding> bindings = new List<IBinding> ();

		public void Add (IBinding binding)
		{
			Check.Argument (binding, "binding").NotNull ();
			binding.Enabled = Enabled;
			bindings.Add (binding);
            if (Bound)
            {
                binding.Bind();
                binding.UpdateTarget();
            }
        }

		public void Remove (IBinding binding)
		{
			Check.Argument (binding, "binding").NotNull ();
			if (bindings.Remove (binding)) {
				binding.Unbind ();
			}
		}

        public void Clear()
        {
            Unbind();
            bindings.Clear();
        }

		public void UpdateTarget ()
		{
			bindings.ForEach (_ => _.UpdateTarget ());
		}

		public void UpdateSource ()
		{
			bindings.ForEach (_ => _.UpdateSource ());
		}

		public void Bind ()
		{
            bindings.ForEach (_ => _.Bind ());
            Bound = true;
		}

		public void Unbind ()
		{
            bindings.ForEach (_ => _.Unbind ());
            Bound = false;
		}

		public bool Enabled { 
			get { return bindings.All (_ => _.Enabled); }
			set { bindings.ForEach (_ => _.Enabled = value); }
		}

		public bool Bound { get; private set; }

        public int Count { get { return bindings.Count; } }
	}

    public class WeakBindingList : IBinding
    {
        readonly WeakCollection<IBinding> bindings = new WeakCollection<IBinding>();

        public void Add(IBinding binding)
        {
            Check.Argument(binding, "binding").NotNull();
            binding.Enabled = Enabled;
            bindings.Add(binding);
            if (Bound)
            {
                binding.Bind();
                binding.UpdateTarget();
            }
        }

        public void Remove(IBinding binding)
        {
            Check.Argument(binding, "binding").NotNull();
            if (bindings.Remove(binding))
            {
                binding.Unbind();
            }
        }

        public void Clear()
        {
            Unbind();
            bindings.Clear();
        }

        public void UpdateTarget()
        {
            bindings.ForEach(_ => _.UpdateTarget());
        }

        public void UpdateSource()
        {
            bindings.ForEach(_ => _.UpdateSource());
        }

        public void Bind()
        {
            bindings.ForEach(_ => _.Bind());
            Bound = true;
        }

        public void Unbind()
        {
            bindings.ForEach(_ => _.Unbind());
            Bound = false;
        }

        public bool Enabled
        {
            get { return bindings.All(_ => _.Enabled); }
            set { bindings.ForEach(_ => _.Enabled = value); }
        }

        public bool Bound { get; private set; }

        public int Count { get { return bindings.CompleteCount; } }
    }
}