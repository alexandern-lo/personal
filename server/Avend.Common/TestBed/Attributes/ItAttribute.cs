using System;

namespace Avend.Common.TestBed.Attributes
{
    /// <summary>
    /// Enumeration of possible requirements for test outcome.
    /// </summary>
    public enum Should
    {
        ReturnHttpCode,
        ReturnResponse,
        ReturnData,
        ReturnList,
        ReturnMessage,
        Return,
    }

    /// <summary>
    /// Can be used to describe expected test outcome.
    /// Annotates a test method, could be used multiple times.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ItAttribute : Attribute
    {
        public Should Should { get; }
        public string Result { get; }

        public ItAttribute(string result)
        {
            Should = Should.Return;
            Result = result;
        }

        public ItAttribute(Should should, string result)
        {
            Should = should;
            Result = result;
        }

        public ItAttribute(Should should, int code)
        {
            Should = should;
            Result = code.ToString();
        }
    }
}