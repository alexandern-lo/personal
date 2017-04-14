using System;

namespace Avend.Common.TestBed.Attributes
{
    public enum Context
    {
        RoleIs,
        RequestFor,
        QueryHasParameter,
        General,
        And,
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class WhenAttribute : Attribute
    {
        public Context Context { get; }
        public string Condition { get; }

        public WhenAttribute(string condition)
        {
            Context = Context.General;
            Condition = condition;
        }

        public WhenAttribute(Context context, string condition)
        {
            Context = context;
            Condition = condition;
        }
    }
}