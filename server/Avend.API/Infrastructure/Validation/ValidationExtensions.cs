using System;
using Qoden.Validation;

namespace Avend.API.Infrastructure.Validation
{
    public static class ValidationExtensions
    {
        public static Error Validator(this Error error, string name)
        {
            error.Add("Validator", name);
            return error;
        }

        /// <summary>
        /// Convert checked value from one type to another.
        /// </summary>
        /// <remarks>If converter throws then exception is supressed and check failed with error</remarks>
        /// <typeparam name="T">Original type</typeparam>
        /// <typeparam name="TRet">Target value type</typeparam>
        /// <param name="check">checker</param>
        /// <param name="converter">converter function</param>
        /// <param name="forceRun">run converter even if checker has error</param>
        /// <returns></returns>
        public static Check<TRet> ConvertTo<T, TRet>(this Check<T> check, Func<T, TRet> converter, bool forceRun = false)
        {
            if (check.HasError && !forceRun)
                return new Check<TRet>(default(TRet), check.Key, check.Validator, check.OnErrorAction);
            
            try
            {
                var value = converter(check.Value);
                return new Check<TRet>(value, check.Key, check.Validator, check.OnErrorAction);
            }
            catch (Exception e)
            {
                var newCheck = new Check<TRet>(default(TRet), check.Key, check.Validator, check.OnErrorAction);
                var error = new Error(e.Message)
                {
                    {"Value", check.Value},
                    {"Exception", e}
                };
                newCheck.FailValidator(error, newCheck.OnErrorAction);
                return newCheck;
            }
        }

    }
}