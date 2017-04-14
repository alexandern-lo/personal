using System;
using System.Runtime.Serialization;

namespace Avend.API.Model.NetworkDTO
{
    /// <summary>
    /// Network DTO class for passing terms to the user.
    /// </summary>
    [DataContract(Name = "terms")]
    public class TermsDTO
    {
        /// <summary>
        /// Unique identifier representing the specific terms record.
        /// </summary>
        /// <value>Unique identifier representing the specific terms record.</value>
        [DataMember(Name = "terms_uid")]
        public Guid Uid { get; set; }

        /// <summary>
        /// Text of terms.
        /// </summary>
        /// <value>Text of terms</value>
        [DataMember(Name = "text")]
        public string Text { get; set; }

        /// <summary>
        /// Release date of new terms text.
        /// </summary>
        /// <value>Release date of new terms text</value>
        [DataMember(Name = "release_date")]
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Date and time when current user accepted terms.
        /// </summary>
        /// <value>Date and time when current user accepted terms. Could be blank.</value>
        [DataMember(Name = "accepted_date")]
        public DateTime? AcceptedAt { get; set; }

        /// <summary>
        /// Date and time of the record creation.
        /// </summary>
        /// <value>Date and time of the record creation.</value>
        [DataMember(Name = "created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time of last record update.
        /// </summary>
        /// <value>Date and time of last record update. Can be blank.</value>
        [DataMember(Name = "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public static TermsDTO From(Terms termsObj, DateTime? acceptedAt)
        {
            var dto = new TermsDTO()
            {
                Uid = termsObj.Uid,

                Text = termsObj.TermsText,
                ReleaseDate = termsObj.ReleaseDate,
                AcceptedAt = acceptedAt,

                CreatedAt = termsObj.CreatedAt,
                UpdatedAt = termsObj.UpdatedAt,
            };

            return  dto;
        }
    }
}