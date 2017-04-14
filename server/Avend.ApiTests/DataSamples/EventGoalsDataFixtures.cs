using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Avend.ApiTests.Infrastructure;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Events.NetworkDTO;

using Qoden.Validation;

namespace Avend.ApiTests.DataSamples
{
    public class EventGoalsDataFixtures
    {
        public TestSystem System { get; }
        public TestUser User { get; }

        public EventDto Event { get; }
        public List<EventUserGoalsDto> EventGoals { get; }

        public EventGoalsDataFixtures(TestUser user, TestSystem system, EventDto eventDto)
        {
            Assert.Argument(user, nameof(user)).NotNull();
            Assert.Argument(system, nameof(system)).NotNull();
            Assert.Argument(eventDto, nameof(eventDto)).NotNull();

            User = user;
            System = system;
            Event = eventDto;

            EventGoals = new List<EventUserGoalsDto>();
        }

        public async Task<EventUserGoalsDto> AddFromDto(EventUserGoalsDto dto)
        {
            using (var http = System.CreateClient(User.Token))
            {
                dto.Uid = await http.PostJsonAsync($"events/{dto.EventUid}/goals", dto).AvendResponse<Guid>();

                EventGoals.Add(dto);
            }

            return dto;
        }

        public async Task<EventUserGoalsDto> AddFromSample(Action<EventUserGoalsDto> postProcessor = null)
        {
            var goalsDto = MakeSample(Event.Uid.Value, postProcessor);

            return await AddFromDto(goalsDto);
        }

        #region Static methods and fields

        private static int goalValue = 10;

        public static async Task<EventGoalsDataFixtures> InitWithSampleGoals(TestUser user, TestSystem system, EventDto eventDto)
        {
            var fixtures = new EventGoalsDataFixtures(user, system, eventDto);

            await fixtures.AddFromSample();

            return fixtures;
        }

        public EventUserGoalsDto MakeSample(Guid eventUid, Action<EventUserGoalsDto> postProcessor = null)
        {
            goalValue += 10;

            var goalsDto = new EventUserGoalsDto()
            {
                EventUid = eventUid,
                LeadsGoal = goalValue,
            };

            postProcessor?.Invoke(goalsDto);

            return goalsDto;
        }

        #endregion
    }
}