using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using Avend.ApiTests.Infrastructure;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;

using Qoden.Validation;

namespace Avend.ApiTests.DataSamples
{
    public class EventData : BaseEntityData<EventRecord, EventDto>
    {
        public static async Task<EventData> InitWithSampleEvent(TestUser user, TestSystem system)
        {
            using (var http = system.CreateClient(user.Token))
            {
                var dto = MakeSample();
                var newEventDto = await http.PostJsonAsync("events", dto).AvendResponse<EventDto>();
                return new EventData(user, system)
                {
                    Event = newEventDto
                };
            }
        }

        public EventData(TestUser user, TestSystem system)
            : base(user, system, "events")
        {
            Assert.Argument(user, nameof(user)).NotNull();
            Assert.Argument(system, nameof(system)).NotNull();

            CreateRequests = new List<EventDto>();
        }

        public EventDto Event { get; private set; }

        public override async Task<EventDto> AddFromDto(EventDto dto)
        {
            using (var http = System.CreateClient(User.Token))
            {
                var newEvent = await http.PostJsonAsync("events", dto).AvendResponse<EventDto>();

                CreateRequests.Add(dto);

                if (Event == null)
                    Event = newEvent;

                return newEvent;
            }
        }

        public override async Task<EventDto> AddFromSample(Action<EventDto> postProcessor = null)
        {
            var expenseDto = MakeSample(postProcessor);

            return await AddFromDto(expenseDto);
        }

        public List<EventDto> CreateRequests { get; set; }

        private static int _nameIndex = 100;
        private static int _imageIndex = 0;
        private static int _venueIndex = 0;

        public new static EventDto MakeSample(Action<EventDto> postProcessor = null)
        {
            _nameIndex--;
            _imageIndex++;
            _venueIndex = -(_venueIndex + 1);

            var newEventDto = new EventDto()
            {
                Name = $"Test Event Name [{_nameIndex}]",
                VenueName = "Building 'Test#" + string.Format("{0,0:D4}", _venueIndex) + "'",
                Type = "conference",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(3),
                Address = "Test Address",
                City = "Test City",
                State = "Test State",
                Country = "Russia",
                ZipCode = "123456",
                Industry = "Oil and Gas Industry",
                WebsiteUrl = "http://event.com",
                LogoUrl = "http://event.com/logo_" + string.Format("{0,0:D4}", _imageIndex) + ".png"
            };

            postProcessor?.Invoke(newEventDto);

            return newEventDto;
        }

        public static AttendeeCategoryDto MakeSampleCategory()
        {
            return new AttendeeCategoryDto()
            {
                Name = "Test category",
                Options = new List<AttendeeCategoryOptionDto>()
                {
                    new AttendeeCategoryOptionDto()
                    {
                        Name = "Test option 1-1"
                    },
                    new AttendeeCategoryOptionDto()
                    {
                        Name = "Test option 1-2"
                    },
                },
            };
        }

        public async Task<TestAttendeeCategory> Addcategory(Guid? eventUid, string name,
            params string[] values)
        {
            using (var http = System.CreateClient(TestUser.AlexTester.Token))
            {
                var property = new AttendeeCategoryDto {Name = name, Options = new List<AttendeeCategoryOptionDto>()};
                foreach (var v in values)
                {
                    property.Options.Add(new AttendeeCategoryOptionDto {Name = v});
                }
                var category = await http.PostJsonAsync($"events/{eventUid}/attendee_categories", property)
                    .AvendResponse<AttendeeCategoryDto>();
                return new TestAttendeeCategory(category);
            }
        }

        public static AttendeeDto MakeAttendee(params AttendeeCategoryValueDto[] propertyValues)
        {
            return new AttendeeDto()
            {
                Title = "Dr",
                FirstName = "Test first name",
                LastName = "Test last name",
                Company = "Test company",
                Email = "test@email.com",
                Phone = "+7-TEST-12345",
                AvatarUrl = "http://mypic.com/avatar",
                CategoryValues = new List<AttendeeCategoryValueDto>(propertyValues)
        };
        }

        public async Task<AttendeeDto> AddAttendee(Guid? eventUid, AttendeeDto attendee)
        {
            using (var http = System.CreateClient(User.Token))
            {
                var attendeeUid = await http.PostJsonAsync($"events/{eventUid}/attendees", attendee)
                    .AvendResponse<Guid>();
                return await http.GetJsonAsync($"events/{eventUid}/attendees/{attendeeUid}")
                    .AvendResponse<AttendeeDto>();
            }
        }
    }

    [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
    public class TestAttendeeCategory
    {
        public TestAttendeeCategory(AttendeeCategoryDto dto)
        {
            Category = dto;
        }

        public AttendeeCategoryDto Category { get; }

        public Guid Option(string name)
        {
            return Category.Options.First(x => x.Name == name).Uid.Value;
        }

        public AttendeeCategoryValueDto Value(string optionValue)
        {
            return new AttendeeCategoryValueDto
            {
                CategoryUid = Category.Uid.Value,
                OptionUid = Option(optionValue)
            };
        }
    }
}