using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventQuestions
{
    [TestClass]
    public class QuestionsCreateTest : QuestionsCreateUpdateTestBase
    {
        protected override HttpRequestMessage MakeRequest()
        {
            return new HttpRequestMessage(HttpMethod.Post, $"events/{EventUid}/questions");
        }

        [TestMethod]
        public async Task Create()
        {
            var question = await BobQuestions.AddQuestion(EventQuestionData.MakeSample());
            question.Uid.Should().NotBeEmpty();
            question.Text.Should().Be(QuestionRequest.Text);
            question.Position.Should().Be(0);
            question.Choices.Count.Should().Be(3);
            question.Choices.Select(x => x.Position).Should().Equal(new[] {0, 1, 2});
            question.Choices.Select(x => x.Text).Should()
                .Equal(QuestionRequest.Choices.Select(x => x.Text));
        }

        [TestMethod]
        public async Task Create_TooMuchQuestions()
        {
            for (var i = 0; i < 10; ++i)
            {
                await BobQuestions.AddQuestion(EventQuestionData.MakeSample());
            }
            await BobTA.PostJsonAsync($"events/{EventUid}/questions", EventQuestionData.MakeSample())
                .Response(HttpStatusCode.BadRequest, "Cannot create more than 10 questions");
        }
    }
}