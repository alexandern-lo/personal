using System;
using System.Net;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventQuestions
{
    [TestClass]
    public class QuestionsDeleteTest : QuestionsTestBase
    {
        protected EventQuestionDto Q1, Q2, Q3;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();

            Q1 = await BobQuestions.AddQuestion(EventQuestionData.MakeSample());
            Q2 = await BobQuestions.AddQuestion(EventQuestionData.MakeSample());
            Q3 = await BobQuestions.AddQuestion(EventQuestionData.MakeSample());
        }

        [TestMethod]
        public async Task MassDeleteQuestions()
        {
            await BobTA.GetJsonAsync($"events/{EventUid}/questions")
                .AvendListResponse<EventQuestionDto>(3, format: "3 questions created for Bob event");

            var deleted = await BobTA.PostJsonAsync($"events/{EventUid}/questions/delete", new[] {Q1.Uid, Q3.Uid})
                .AvendResponse<Guid[]>();
            deleted.Should().Contain(new[] {Q1.Uid, Q3.Uid});

            var questions = await BobTA.GetJsonAsync($"events/{EventUid}/questions")
                .AvendListResponse<EventQuestionDto>(1, format: "Bob event has only 1 question left");
            questions[0].Uid.Should().Be(Q2.Uid);
        }

        [TestMethod]
        public async Task CanMassDeleteQuestionWithLeadData()
        {
            await AddLeadData(Q1);
            await BobTA.PostJsonAsync($"events/{EventUid}/questions/delete", new[] {Q1.Uid})
                .Response(HttpStatusCode.OK, "Can mass delete questions when it has lead");
        }

        [TestMethod]
        public async Task CanDeleteWithLeadData()
        {
            await AddLeadData(Q1);
            await BobTA.DeleteJsonAsync($"events/{EventUid}/questions/{Q1.Uid}")
                .Response(HttpStatusCode.OK, "Can delete question when it has lead");
        }

        [TestMethod]
        public async Task OthersCannotDetele()
        {
            await CecileSU.DeleteJsonAsync($"events/{EventUid}/questions/{Q1.Uid}")
                .Response(HttpStatusCode.NotFound, "SU cannot delete user questions");

            await MarcTA.DeleteJsonAsync($"events/{EventUid}/questions/{Q1.Uid}")
                .Response(HttpStatusCode.NotFound, "Other TA cannot delete user questions");

            await AlexSA.DeleteJsonAsync($"events/{EventUid}/questions/{Q1.Uid}")
                .Response(HttpStatusCode.NotFound, "SA cannot delete user questions");
        }
    }
}