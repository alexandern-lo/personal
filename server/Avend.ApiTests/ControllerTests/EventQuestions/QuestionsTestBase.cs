using System;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Leads.NetworkDTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventQuestions
{
    public class QuestionsTestBase : IntegrationTest
    {
        protected Guid EventUid;
        protected EventQuestionData BobQuestions;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();

            var eventData = await EventData.InitWithSampleEvent(TestUser.BobTester, System);
            BobQuestions = await EventQuestionData.Create(TestUser.BobTester, System, eventData.Event);
            EventUid = eventData.Event.Uid.GetValueOrDefault();
        }

        protected async Task AddLeadData(EventQuestionDto question)
        {
            var leadData = LeadData.Init(TestUser.BobTester, EventUid, System);
            await leadData.Add(x =>
            {
                x.QuestionAnswers.Add(new LeadQuestionAnswerDto()
                {
                    EventQuestionUid = question.Uid,
                    EventAnswerUid = question.Choices[0].Uid
                });
            });
        }
    }
}