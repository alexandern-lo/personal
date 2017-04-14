using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Responses;
using Qoden.Validation;
using Error = Qoden.Validation.Error;

namespace Avend.API.Infrastructure.Validation
{
    public static class IndustryValidation
    {
        private static readonly string[] Industries = new string[]
        {
            "Aerospace & Defense",
            "Agency",
            "Agriculture",
            "Automotive",
            "Business & Professional Services",
            "Chemicals",
            "Conference & Event Services",
            "Construction",
            "Consumer Goods & Services",
            "Electric Power Industry",
            "Energy Industry",
            "Financial Services",
            "Firearms & Explosives",
            "Food & Beverage",
            "Government",
            "Health Care",
            "Housing & Real Estate",
            "IT consulting",
            "IT services",
            "Manufacturing",
            "Mining & Drilling",
            "Nuclear Power Industry",
            "Oil and Gas Industry",
            "Pharmaceuticals & Biotechnology",
            "Printing & Publishing",
            "Software",
            "Technology (other)",
            "Telecommunications & Media",
            "Transportation & Logistics",
            "Other"
        };

        public static Check<string> IsValidIdustry(this Check<string> check,
            string message = "{Key} should be a valid industry name", Action<Error> onError = null)
        {
            return check.In(Industries, message, onError);
        }
    }
}