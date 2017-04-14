using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Avend.API.Model;

namespace Avend.API.Infrastructure.Responses
{
    /// <summary>
    /// Encapsulates error response data and provides static methods for generation of common error responses.
    /// </summary>
    [DataContract]
    public class ErrorResponse : BaseResponse, IEquatable<ErrorResponse>
    {
        public static readonly string UnknownError = "unknown error";

        public static readonly string ErrorMessageNotFound = "The {0} with UID={1} is not found";
        public static readonly string ErrorMessageInvalidUser = "The user record is not valid";
        public static readonly string ErrorMessageSubscriptionExpired = "The subscription is expired on {0}";
        public static readonly string ErrorMessageSubscriptionAbsent = "The subscription for this user is not found";
        public static readonly string ErrorMessageInvalidParameter = "{0} parameter ({1}) is not valid ({2})";
        public static readonly string ErrorMessageRequiredParameter = "The parameter {1} of type {0} is required";
        public static readonly string ErrorMessageAlreadyAccepted = "Terms with UID={0} already accepted";

        public static readonly string TypeUnknown = "unknown";
        public static readonly string TypeUser = "user";
        public static readonly string TypeUserSubscription = "subscription";
        public static readonly string TypeUserSettings = "settings";
        public static readonly string TypeUserCrmConfiguration = "CRM account";
        public static readonly string TypeUserTerms = "terms";
        public static readonly string TypeResource = "resource";
        public static readonly string TypeEvent = "event";
        public static readonly string TypeEventAttendee = "event attendee";
        public static readonly string TypeEventAttendeeCategory = "attendee property";
        public static readonly string TypeEventQuestion = "event question";
        public static readonly string TypeEventQuestionAnswer = "event question answer";
        public static readonly string TypeLead = "lead";
        public static readonly string TypeLeadPhone = "lead phone";
        public static readonly string TypeLeadEmail = "lead email";
        public static readonly string TypeLeadQuestionAnswer = "lead question answer";


        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResponse" /> class.
        /// </summary>
        /// <param name="errors">Errors (required).</param>
        public ErrorResponse(List<Error> errors)
            : base(false)
        {
            // "Errors" is required (not null)
            if (errors == null)
            {
                throw new InvalidDataException("Errors is a required property for ErrorResponse and cannot be null");
            }

            Errors = errors;
        }

        /// <summary>
        /// Gets or Sets Errors
        /// </summary>
        [DataMember(Name = "errors", Order = 10)]
        public List<Error> Errors { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ErrorResponse {\n");
            sb.Append("  Success: ").Append(Success).Append("\n");
            sb.Append("  Errors: ").Append(Errors).Append("\n");

            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ErrorResponse) obj);
        }

        /// <summary>
        /// Returns true if ErrorResponse instances are equal
        /// </summary>
        /// <param name="other">Instance of ErrorResponse to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ErrorResponse other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
            (
                Success == other.Success ||
                Success.Equals(other.Success)
            ) && (
                Errors == other.Errors ||
                Errors != null &&
                Errors.SequenceEqual(other.Errors)
            );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;
                // Suitable nullity checks etc, of course :)

                hash = hash * 59 + Success.GetHashCode();

                if (Errors != null)
                    hash = hash * 59 + Errors.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(ErrorResponse left, ErrorResponse right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ErrorResponse left, ErrorResponse right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

        public static string GetEntityTypeStringByType(Type entityClass)
        {
            var entityTypeStr = TypeUnknown;
            if (entityClass == typeof(EventRecord))
            {
                entityTypeStr = TypeEvent;
            }
            else if (entityClass == typeof(EventQuestionRecord))
            {
                entityTypeStr = TypeEventQuestion;
            }
            else if (entityClass == typeof(AnswerChoiceRecord))
            {
                entityTypeStr = TypeEventQuestionAnswer;
            }
            else if (entityClass == typeof(AttendeeRecord))
            {
                entityTypeStr = TypeEventAttendee;
            }
            else if (entityClass == typeof(AttendeeCategoryRecord))
            {
                entityTypeStr = TypeEventAttendeeCategory;
            }
            else if (entityClass == typeof(LeadRecord))
            {
                entityTypeStr = TypeLead;
            }
            else if (entityClass == typeof(LeadPhone))
            {
                entityTypeStr = TypeLeadPhone;
            }
            else if (entityClass == typeof(LeadEmail))
            {
                entityTypeStr = TypeLeadEmail;
            }
            else if (entityClass == typeof(LeadQuestionAnswer))
            {
                entityTypeStr = TypeLeadQuestionAnswer;
            }
            else if (entityClass == typeof(Terms))
            {
                entityTypeStr = TypeUserTerms;
            }
            else if (entityClass == typeof(SubscriptionRecord))
            {
                entityTypeStr = TypeUserSubscription;
            }
            else if (entityClass == typeof(SettingsRecord))
            {
                entityTypeStr = TypeUserSettings;
            }
            else if (entityClass == typeof(CrmRecord))
            {
                entityTypeStr = TypeUserCrmConfiguration;
            }

            return entityTypeStr;
        }

        public static ErrorResponse GenerateNotFound(Type entityClass, Guid recordUid, string field)
        {
            return GenerateNotFound(entityClass, recordUid.ToString(), field);
        }

        public static ErrorResponse GenerateNotFound(Type entityClass, string recordUid, string field)
        {
            var entityTypeStr = GetEntityTypeStringByType(entityClass);

            var error = new Error(ErrorCodes.CodeNotFound, string.Format(ErrorMessageNotFound, entityTypeStr, recordUid),
                new[] {field});

            return new ErrorResponse(new List<Error>
            {
                error
            });
        }

        public static ErrorResponse GenerateNotFound(Type entityClass, IEnumerable<Guid> recordUids, string field)
        {
            var entityTypeStr = GetEntityTypeStringByType(entityClass);

            var errors = new List<Error>();

            foreach (var recordUid in recordUids)
            {
                var error = new Error(ErrorCodes.CodeNotFound,
                    string.Format(ErrorMessageNotFound, entityTypeStr, recordUid),
                    new[] {field});

                errors.Add(error);
            }

            return new ErrorResponse(errors);
        }

        public static ErrorResponse GenerateInvalidUser(string field)
        {
            Error error = GenerateInvalidUserError(field);

            return new ErrorResponse(new List<Error>
            {
                error
            });
        }

        public static Error GenerateInvalidUserError(string field)
        {
            return new Error(ErrorCodes.CodeInvalidUser, string.Format(ErrorMessageInvalidUser), new[] {field});
        }

        public static ErrorResponse GenerateTermsAlreadyAccepted(Guid recordUid, string field)
        {
            var error = new Error(ErrorCodes.CodeTermsAlreadyAccepted,
                string.Format(ErrorMessageAlreadyAccepted, recordUid),
                new[] {field});

            return new ErrorResponse(new List<Error>
            {
                error
            });
        }

        public static ErrorResponse GenerateInvalidParameter(Type entityClass, Guid recordUid, string details,
            string field)
        {
            return GenerateInvalidParameter(entityClass, recordUid.ToString(), details, field);
        }

        public static ErrorResponse GenerateInvalidParameter(Type entityClass, string recordUid, string details,
            string field)
        {
            Error error = GenerateInvalidParameterError(entityClass, recordUid, details, field);

            return new ErrorResponse(new List<Error>
            {
                error
            });
        }

        public static Error GenerateInvalidParameterError(Type entityClass, string propertyName, string details,
            string field)
        {
            return GenerateInvalidParameterError(GenerateInvalidParameterMessage(entityClass, propertyName, details),
                field);
        }


        public static Error GenerateInvalidParameterError(string dto, string propertyName, string details, string field)
        {
            return GenerateInvalidParameterError(GenerateInvalidParameterMessage(dto, propertyName, details), field);
        }

        public static Error GenerateInvalidParameterError(string message, string field)
        {
            return new Error(ErrorCodes.CodeInvalidParameter, message, new[] {field});
        }

        public static string GenerateInvalidParameterMessage(string dto, string propertyName, string details)
        {
            return string.Format(ErrorMessageInvalidParameter, dto, propertyName, details ?? UnknownError);
        }

        public static string GenerateInvalidParameterMessage(Type entityClass, string recordUid, string details)
        {
            var entityTypeStr = GetEntityTypeStringByType(entityClass);
            return GenerateInvalidParameterMessage(entityTypeStr, recordUid, details);
        }

        public static ErrorResponse GenerateRequiredParameter(Type entityClass, string field)
        {
            var entityTypeStr = GetEntityTypeStringByType(entityClass);

            var error = new Error(ErrorCodes.CodeInvalidParameter,
                string.Format(ErrorMessageRequiredParameter, entityTypeStr, field), new[] {field});

            return new ErrorResponse(new List<Error>
            {
                error
            });
        }

        public static ErrorResponse GenerateSubscriptionExpired(DateTime? expiredOn)
        {
            return new ErrorResponse(new List<Error>
            {
                GenerateSubscriptionExpiredError(expiredOn)
            });
        }

        [SuppressMessage("ReSharper", "ConvertIfStatementToConditionalTernaryExpression")]
        public static Error GenerateSubscriptionExpiredError(DateTime? expiredOn)
        {
            Error error;

            if (expiredOn.HasValue)
                error = new Error(ErrorCodes.SubscriptionExpired,
                    string.Format(ErrorMessageSubscriptionExpired, expiredOn.Value), new[] {"expired_at"});
            else
                error = new Error(ErrorCodes.SubscriptionExpired, string.Format(ErrorMessageSubscriptionAbsent),
                    new[] {"subscription_uid"});

            return error;
        }
    }
}