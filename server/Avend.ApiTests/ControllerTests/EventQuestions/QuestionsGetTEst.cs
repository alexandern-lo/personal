using System.Linq;
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
    public class QuestionsGetTest : QuestionsTestBase
    {
        protected EventQuestionDto Q1, Q2;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();

            Q1 = await BobQuestions.AddQuestion(EventQuestionData.MakeSample());
            Q2 = await BobQuestions.AddQuestion(EventQuestionData.MakeSample());
        }

        [TestMethod]
        public async Task GetQuestions()
        {
            var questions = await BobTA.GetJsonAsync($"events/{EventUid}/questions")
                .AvendListResponse<EventQuestionDto>(2);
            questions.Select(x => x.Position).Should().Equal(0, 1);
            questions.Select(x => x.Uid).Should().Equal(Q1.Uid, Q2.Uid);
        }

        [TestMethod]
        public async Task GetQuestion()
        {
            var question = await BobTA.GetJsonAsync($"events/{EventUid}/questions/{Q1.Uid}")
                .AvendResponse<EventQuestionDto>();
            question.Uid.Should().Be(Q1.Uid);
        }

        [TestMethod]
        public async Task EventsArePrivateToOthers()
        {
            await CecileSU.GetJsonAsync($"events/{EventUid}/questions")
                .Response(HttpStatusCode.NotFound, "SU does not have access to TA event");

            await MarcTA.GetJsonAsync($"events/{EventUid}/questions")
                .Response(HttpStatusCode.NotFound, "Marc cannot see other tenant questions");

            await AlexSA.GetJsonAsync($"events/{EventUid}/questions")
                .AvendListResponse<EventQuestionDto>(0, HttpStatusCode.OK, "SA can see own questions for any event");
        }
    }
}