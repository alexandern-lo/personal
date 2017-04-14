using System;
using System.Collections.Generic;
using System.Linq;

using Avend.API.Model.NetworkDTO;

namespace Avend.ApiTests.DataSamples
{
    public static class EventAttendeeDataSamples
    {
        public static AttendeeDto GetTestObject()
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
                CategoryValues = new List<AttendeeCategoryValueDto>()
                {
                    new AttendeeCategoryValueDto()
                    {
                        CategoryName = "Test category 1",
                        OptionName = "Test option 1-1",
                    },
                    new AttendeeCategoryValueDto()
                    {
                        CategoryName = "Test category 2",
                        OptionName = "Test option 2-2",
                    },
                }
            };
        }

        public static AttendeeDto GetTestObjectForEvent(EventDto eventDto)
        {
            var eventAttendeeDto = GetTestObject();
            eventAttendeeDto.CategoryValues.ForEach(catValueRecord =>
            {
                var attendeeCategoryDto = eventDto.AttendeeCategories.FirstOrDefault(catRecord => catRecord.Name == catValueRecord.CategoryName);
                if (attendeeCategoryDto != null)
                {
                    catValueRecord.CategoryUid = attendeeCategoryDto.Uid ?? Guid.Empty;
                    var categoryOptionDto = attendeeCategoryDto.Options.FirstOrDefault(optiontRecord => optiontRecord.Name == catValueRecord.OptionName);
                    if (categoryOptionDto != null)
                        catValueRecord.OptionUid = categoryOptionDto.Uid ?? Guid.Empty;
                }
            });

            return eventAttendeeDto;
        }

        public static AttendeesFilterRequestDTO GetPositiveFilterObjectForTestObject(EventDto eventDto, AttendeeDto attendeeDto)
        {
            var filterDto = new AttendeesFilterRequestDTO()
            {
                Query = "Test",
                Categories = new List<AttendeesFilterCategoryValuesDTO>()
                {
                    new AttendeesFilterCategoryValuesDTO()
                    {
                        Uid = attendeeDto.CategoryValues[0].CategoryUid,
                        Values = new List<Guid>()
                        {
                            attendeeDto.CategoryValues[0].OptionUid,
                        }
                    },
                    new AttendeesFilterCategoryValuesDTO()
                    {
                        Uid = attendeeDto.CategoryValues[1].CategoryUid,
                        Values = new List<Guid>()
                        {
                            attendeeDto.CategoryValues[1].OptionUid,
                        }
                    }
                }
            };

            return filterDto;
        }

        public static AttendeesFilterRequestDTO GetNegativeFilterObjectForTestObject(EventDto eventDTO, AttendeeDto attendeeDTO)
        {
            var filterDTO = new AttendeesFilterRequestDTO()
            {
                Query = "Test",
                Categories = new List<AttendeesFilterCategoryValuesDTO>()
                {
                    new AttendeesFilterCategoryValuesDTO()
                    {
                        Uid = attendeeDTO.CategoryValues[0].CategoryUid,
                        Values = new List<Guid>()
                        {
                            Guid.NewGuid(),
                        }
                    },
                    new AttendeesFilterCategoryValuesDTO()
                    {
                        Uid = attendeeDTO.CategoryValues[1].CategoryUid,
                        Values = new List<Guid>()
                        {
                            attendeeDTO.CategoryValues[1].OptionUid,
                        }
                    }
                }
            };

            return filterDTO;
        }
    }
}