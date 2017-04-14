using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventQuestions
{
    [TestClass]
    public class QuestionsUpdateTest : QuestionsCreateUpdateTestBase
    {
        protected EventQuestionDto Question;

        protected override HttpRequestMessage MakeRequest()
        {
            return new HttpRequestMessage(HttpMethod.Put, $"events/{EventUid}/questions/{Question.Uid}");
        }

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            Question = await BobQuestions.AddQuestion(QuestionRequest);
        }

        [TestMethod]
        public async Task OtherUsersCannotUpdate()
        {
            await MarcTA.PutJsonAsync($"events/{EventUid}/questions/{Question.Uid}", EventQuestionData.MakeSample())
                .Response(HttpStatusCode.NotFound, "Other TA does not see user questions");

            await CecileSU.PutJsonAsync($"events/{EventUid}/questions/{Question.Uid}", EventQuestionData.MakeSample())
                .Response(HttpStatusCode.NotFound, "SU does not see user questions");

            await AlexSA.PutJsonAsync($"events/{EventUid}/questions/{Question.Uid}", EventQuestionData.MakeSample())
                .Response(HttpStatusCode.NotFound, "SA does not see user questions");
        }

        [TestMethod]
        public async Task CannotUpdateWithLeadData()
        {
            await AddLeadData(Question);
            await BobTA.PutJsonAsync($"events/{EventUid}/questions/{Question.Uid}", EventQuestionData.MakeSample())
                .Response(HttpStatusCode.Forbidden, "Cannot update question when event it has lead");
        }

        [TestMethod]
        public async Task CannotUpdatePosition()
        {
            Question.Position = 2;
            var updated = await BobTA.PutJsonAsync($"events/{EventUid}/questions/{Question.Uid}", Question)
                .AvendResponse<EventQuestionDto>();
            updated.Position.Should().Be(0);
        }
    }
}