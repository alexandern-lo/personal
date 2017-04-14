using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avend.ApiTests.Infrastructure;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using Qoden.Validation;

namespace Avend.ApiTests.DataSamples
{
    public class EventQuestionData
    {
        public static async Task<EventQuestionData> Create(TestUser user, TestSystem system, EventDto @event = null)
        {
            if (@event == null)
            {
                var data = await EventData.InitWithSampleEvent(user, system);
                @event = data.Event;
            }
            return new EventQuestionData(@event, user, system);
        }

        public EventQuestionData(EventDto @event, TestUser user, TestSystem system)
        {
            Assert.Argument(@event, nameof(@event)).NotNull();
            Assert.Argument(user, nameof(user)).NotNull();
            Assert.Argument(system, nameof(system)).NotNull();
            Event = @event;
            User = user;
            System = system;
        }

        public EventDto Event { get; set; }
        public TestSystem System { get; set; }
        public TestUser User { get; set; }
        public List<EventQuestionDto> Questions { get; } = new List<EventQuestionDto>();

        public async Task<EventQuestionDto> AddQuestion(EventQuestionDto data)
        {
            using (var browser = System.CreateClient(User.Token))
            {
                var question = await browser.PostJsonAsync($"events/{Event.Uid}/questions", data)
                    .AvendResponse<EventQuestionDto>();
                Questions.Add(question);
                return question;
            }
        }

        public static EventQuestionDto MakeSample(Action<EventQuestionDto> postProcessor = null)
        {
            var dto = new EventQuestionDto
            {
                Text = "Definition of continuous function?",
                Choices = new List<AnswerChoiceDto>
                {
                    new AnswerChoiceDto
                    {
                        Text = "When you can draw function graph in a single pass"
                    },
                    new AnswerChoiceDto
                    {
                        Text = "When for every open-set Oy you can find open-set Ox such that f(Ox) is in Oy"
                    },
                    new AnswerChoiceDto
                    {
                        Text = "When f(a*x + b*y) = a*f(x) + b*f(y)"
                    }
                }
            };
            postProcessor?.Invoke(dto);
            return dto;
        }

        public static AnswerChoiceDto MakeSampleChoice()
        {
            return new AnswerChoiceDto
            {
                Text = "Pick me! I'm valid answer!"
            };
        }
    }
}
