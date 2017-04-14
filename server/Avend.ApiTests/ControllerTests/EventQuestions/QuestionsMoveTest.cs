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
    public class QuestionsMoveTest : QuestionsTestBase
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
        public async Task MoveQuestion()
        {
            var questions = await BobTA.PatchJsonAsync($"events/{EventUid}/questions/{Q2.Uid}/move", 0)
                .AvendListResponse<EventQuestionDto>();
            questions.Select(x => x.Uid).Should().ContainInOrder(Q2.Uid, Q1.Uid);
        }

        [TestMethod]
        public async Task MoveOutOfBounds()
        {
            await BobTA.PatchJsonAsync($"events/{EventUid}/questions/{Q2.Uid}/move", 2)
                .Response(HttpStatusCode.BadRequest, "out of bounds");
        }

        [TestMethod]
        public async Task MoveNegative()
        {
            await BobTA.PatchJsonAsync($"events/{EventUid}/questions/{Q2.Uid}/move", -1)
                .Response(HttpStatusCode.BadRequest, "negative");
        }

        [TestMethod]
        public async Task CanMoveWithLeadData()
        {
            await AddLeadData(Q2);
            await BobTA.PatchJsonAsync($"events/{EventUid}/questions/{Q2.Uid}/move", 0)
                .Response(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task OthersCannotMove()
        {
            await MarcTA.PatchJsonAsync($"events/{EventUid}/questions/{Q2.Uid}/move", 0)
                .Response(HttpStatusCode.NotFound, "Other TA cannot move user questions");

            await CecileSU.PatchJsonAsync($"events/{EventUid}/questions/{Q2.Uid}/move", 0)
                .Response(HttpStatusCode.NotFound, "Other SU cannot move user questions");

            await AlexSA.PatchJsonAsync($"events/{EventUid}/questions/{Q2.Uid}/move", 0)
                .Response(HttpStatusCode.NotFound, "Other SA cannot move user questions");
        }
    }
}