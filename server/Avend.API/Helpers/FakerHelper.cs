using System;
using System.Linq;

using Avend.API.Model;
using Avend.API.Services;

using Bogus;

namespace Avend.API.Helpers
{
    /// <summary>
    /// Helper class to encapsulate all rules for entity generation when using Faker.
    /// </summary>
    public class FakerHelper
    {
        /// <summary>
        /// Constructs Faker object for EventAgendaItem entity.
        /// </summary>
        public static Faker<EventAgendaItem> AgendaItemsFaker(AvendDbContext db, Random random, long eventId, EventRecord eventRecord)
        {
            if (eventRecord == null)
            {
                eventRecord = new EventRecord()
                {
                    Id = eventId,
                };
            }

            var faker = new Faker<EventAgendaItem>()
                .CustomInstantiator(f =>
                {
                    var attendee = new EventAgendaItem()
                    {
                        EventId = eventId,
                        EventRecord = eventRecord,
                    };

                    return attendee;
                })
                .RuleFor(agendaItem => agendaItem.Name, f => $"{f.Commerce.Department()} {f.Commerce.Product()}")
                .RuleFor(agendaItem => agendaItem.Description, f => f.Lorem.Paragraph())
                .RuleFor(agendaItem => agendaItem.Location, f => $"{f.Address.StreetName()} {f.Address.BuildingNumber()}")
                .RuleFor(agendaItem => agendaItem.Date, f => f.Date.Between(eventRecord?.StartDate ?? DateTime.Today, eventRecord?.EndDate ?? DateTime.Today.Add(TimeSpan.FromDays(10))))
                .RuleFor(agendaItem => agendaItem.StartTime, f => f.Date.Timespan(TimeSpan.FromHours(10).Add(TimeSpan.FromHours(2))))
                .RuleFor(agendaItem => agendaItem.EndTime, f => f.Date.Timespan(TimeSpan.FromHours(10).Add(TimeSpan.FromHours(12))))
                .RuleFor(agendaItem => agendaItem.DetailsUrl, f => f.Internet.Avatar())
                .RuleFor(agendaItem => agendaItem.LocationUrl, f => f.Internet.Url())
                .RuleFor(agendaItem => agendaItem.Uid, Guid.NewGuid)
                ;

            return faker;
        }

        /// <summary>
        /// Constructs Faker object for AttendeeRecord entity.
        /// </summary>
        public static Faker<AttendeeRecord> AttendeesFaker(AvendDbContext db, Random random, long eventId, EventRecord eventRecord)
        {
            if (eventRecord == null)
            {
                eventRecord = new EventRecord()
                {
                    Id = eventId,
                };
            }

            var faker = new Faker<AttendeeRecord>()
                .CustomInstantiator(f =>
                {
                    var attendee = new AttendeeRecord()
                    {
                        EventId = eventId,
                        EventRecord = eventRecord,
                    };

                    var attendeeCategoryValues = from attendeeCategory in db.AttendeeCategories
                        where attendeeCategory.EventId == (eventId != 0 ? eventId : eventRecord.Id)
                        select attendeeCategory;

                    attendee.Values = attendeeCategoryValues.ToList()
                        .Select(categoryObj => AssignAttendeeCategoryValue(db, random, categoryObj, attendee))
                        .ToList();

                    return attendee;
                })
                .RuleFor(eventAttendee => eventAttendee.FirstName, f => f.Name.FirstName())
                .RuleFor(eventAttendee => eventAttendee.LastName, f => f.Name.LastName())
                .RuleFor(eventAttendee => eventAttendee.Company, f => f.Company.CompanyName())
                .RuleFor(eventAttendee => eventAttendee.Email, f => f.Internet.Email())
                .RuleFor(eventAttendee => eventAttendee.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(eventAttendee => eventAttendee.Title, f => f.Name.JobTitle())
                .RuleFor(eventAttendee => eventAttendee.AvatarUrl, f => f.Internet.Avatar())
                .RuleFor(eventAttendee => eventAttendee.Uid, Guid.NewGuid)
                ;

            return faker;
        }

        /// <summary>
        /// Auxilliary method to assign proper attendee category value to an event attendee being generated.
        /// </summary>
        public static AttendeeCategoryValue AssignAttendeeCategoryValue(AvendDbContext db, Random random, AttendeeCategoryRecord category, AttendeeRecord attendee)
        {
            var attendeeCategoryOptions = from categoryOption in db.AttendeeCategoryOptions
                where categoryOption.CategoryId == category.Id
                select categoryOption;

            var optionsCount = attendeeCategoryOptions.Count();

            AttendeeCategoryOption attendeeCategoryOption;

            switch (optionsCount)
            {
                case 0:
                    return null;

                case 1:
                    attendeeCategoryOption = attendeeCategoryOptions.First();
                    break;

                default:
                    attendeeCategoryOption = attendeeCategoryOptions.Skip(random.Next(optionsCount)).First();
                    break;
            }

            var value = new AttendeeCategoryValue()
            {
                Attendee = attendee,
                Category = category,
                CategoryId = category?.Id,
                AttendeeCategoryOption = attendeeCategoryOption,
                CategoryOptionId = attendeeCategoryOption?.Id,
                CategoryValue = category?.Name,
                OptionValue = attendeeCategoryOption?.Name
            };

            return value;
        }

        /// <summary>
        /// Constructing Faker object for EventRecord entity.
        /// </summary>
        public static Faker<EventRecord> EventsFaker(Random random, Faker<AttendeeCategoryRecord> categoriesFaker, Faker<EventQuestionRecord> questionsFaker, DateTime? eventStartDate, DateTime? eventEndDate, int minCategories, int minQuestions)
        {
            var eventsFaker = new Faker<EventRecord>()
                .CustomInstantiator(f =>
                {
                    DateTime startDate;
                    DateTime endDate;

                    if (eventStartDate.HasValue)
                    {
                        startDate = eventStartDate.Value.ToUniversalTime().Date;

                        endDate = eventEndDate?.ToUniversalTime().Date ?? eventStartDate.Value.ToUniversalTime().Date;
                    }
                    else
                    {
                        startDate = new DateTime(2016 + random.Next(1), 1 + random.Next(11), 1 + random.Next(27));

                        endDate = startDate + new TimeSpan(random.Next(5));
                    }

                    var eventRecord = new EventRecord()
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        AttendeeCategories = categoriesFaker.Generate(minCategories + random.Next(5 - minCategories)).ToList(),
                        Questions = questionsFaker.Generate(minQuestions + random.Next(5 - minQuestions)).ToList(),
                    };

                    eventRecord.AttendeeCategories.ForEach(record => record.EventRecord = eventRecord);

                    return eventRecord;
                })
                .RuleFor(eventRecord => eventRecord.Type, f =>
                {
                    var typeCode = random.Next(10);

                    if (typeCode <= 8)
                        return EventRecord.EventTypeConference;

                    return EventRecord.EventTypePersonal;
                })
                .RuleFor(eventRecord => eventRecord.LogoUrl, f => f.Image.Business())
                .RuleFor(eventRecord => eventRecord.WebsiteUrl, f => f.Internet.Url())
                .RuleFor(eventRecord => eventRecord.Name, f => f.Commerce.Product() + " Annual Event")
                .RuleFor(eventRecord => eventRecord.VenueName, f => f.Address.SecondaryAddress())
                .RuleFor(eventRecord => eventRecord.Country, f => f.Address.Country())
                .RuleFor(eventRecord => eventRecord.State, f => f.Address.State())
                .RuleFor(eventRecord => eventRecord.City, f => f.Address.City())
                .RuleFor(eventRecord => eventRecord.Industry, f => f.Commerce.Department())
                .RuleFor(eventRecord => eventRecord.Address, f => f.Address.StreetAddress())
                .RuleFor(eventRecord => eventRecord.ZipCode, f => f.Address.ZipCode())
                .RuleFor(eventRecord => eventRecord.Uid, Guid.NewGuid)
                ;

            return eventsFaker;
        }

        /// <summary>
        /// Constructing Faker object for AttendeeCategoryRecord entity.
        /// </summary>
        public static Faker<AttendeeCategoryRecord> AttendeeCategoriesFaker(Random random, Faker<AttendeeCategoryOption> optionsFaker)
        {
            var categoriesFaker = new Faker<AttendeeCategoryRecord>()
                .CustomInstantiator(f =>
                {
                    var attendeeCategory = new AttendeeCategoryRecord()
                    {
                        Options = optionsFaker.Generate(2 + random.Next(5)).ToList(),
                    };

                    attendeeCategory.Options.ForEach(record => record.AttendeeCategory = attendeeCategory);

                    return attendeeCategory;
                })
                .RuleFor(attendeeCategory => attendeeCategory.Name, f => f.Commerce.Categories(1).First())
                .RuleFor(attendeeCategory => attendeeCategory.Uid, Guid.NewGuid);

            return categoriesFaker;
        }

        /// <summary>
        /// Constructing Faker object for AttendeeCategoryOption entity.
        /// </summary>
        public static Faker<AttendeeCategoryOption> AttendeeCategoryOptionsFaker()
        {
            var optionsFaker = new Faker<AttendeeCategoryOption>()
                .RuleFor(categoryOption => categoryOption.Name, f => f.Commerce.ProductAdjective())
                .RuleFor(categoryOption => categoryOption.Uid, Guid.NewGuid);

            return optionsFaker;
        }

        /// <summary>
        /// Constructing Faker object for EventQuestionRecord entity.
        /// </summary>
        public static Faker<EventQuestionRecord> EventQuestionFaker(long userId, Random random, Faker<AnswerChoiceRecord> answersFaker, int cntAsnwers = 0)
        {
            var questionFaker = new Faker<EventQuestionRecord>()
                .CustomInstantiator(f =>
                {
                    var eventQuestion = new EventQuestionRecord()
                    {
                        UserId = userId,
                        Choices = answersFaker.Generate(cntAsnwers > 0 ? cntAsnwers : 2 + random.Next(5)).ToList(),
                    };

                    eventQuestion.Choices.ForEach(record => record.Question = eventQuestion);

                    return eventQuestion;
                })
                .RuleFor(eventQuestion => eventQuestion.Uid, Guid.NewGuid)
                .RuleFor(eventQuestion => eventQuestion.Text, f => f.Commerce.Product())
                ;

            return questionFaker;
        }

        /// <summary>
        /// Constructing Faker object for AnswerChoiceRecord entity.
        /// </summary>
        public static Faker<AnswerChoiceRecord> EventQuestionAnswerFaker()
        {
            var answerFaker = new Faker<AnswerChoiceRecord>()
                .RuleFor(questionAnswer => questionAnswer.Uid, Guid.NewGuid)
                .RuleFor(questionAnswer => questionAnswer.Text, f => f.Commerce.ProductMaterial())
                ;

            return answerFaker;
        }
    }
}