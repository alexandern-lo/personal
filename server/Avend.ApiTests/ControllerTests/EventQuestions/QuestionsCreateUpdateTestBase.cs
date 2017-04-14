using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventQuestions
{
    public abstract class QuestionsCreateUpdateTestBase : QuestionsTestBase
    {
        protected EventQuestionDto QuestionRequest;

        protected abstract HttpRequestMessage MakeRequest();

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            QuestionRequest = EventQuestionData.MakeSample();
        }

        [TestMethod]
        public async Task QuestionWithoutText()
        {
            var testRequest = MakeRequest();
            var invalidText = EventQuestionData.MakeSample(e => e.Text = " ");
            await BobTA.SendJsonAsync(testRequest, invalidText)
                .Response(HttpStatusCode.BadRequest, $"cannot {testRequest.Method} question without text");
        }

        [TestMethod]
        public async Task ChoiceWithoutText()
        {
            var testRequest = MakeRequest();
            var invalidAnswer = EventQuestionData.MakeSample(e => e.Choices[0].Text = " ");
            await BobTA.SendJsonAsync(testRequest, invalidAnswer)
                .Response(HttpStatusCode.BadRequest,
                    $"cannot {testRequest.Method} question which has a choice without text");
        }

        [TestMethod]
        public async Task TooManyAnswers()
        {
            var testRequest = MakeRequest();
            var tooMuchAnswers = EventQuestionData.MakeSample(e =>
            {
                for (var i = 0; i < 20; ++i)
                {
                    e.Choices.Add(EventQuestionData.MakeSampleChoice());
                }
            });
            await BobTA.SendJsonAsync(testRequest, tooMuchAnswers)
                .Response(HttpStatusCode.BadRequest, $"cannot {testRequest.Method} question with too many answers");
        }

        [TestMethod]
        public async Task OnlyOneAnswer()
        {
            var testRequest = MakeRequest();
            var noAnswers =
                EventQuestionData.MakeSample(
                    e => { e.Choices = new List<AnswerChoiceDto>(new[] {EventQuestionData.MakeSampleChoice()}); });
            await BobTA.SendJsonAsync(testRequest, noAnswers)
                .Response(HttpStatusCode.BadRequest, $"cannot {testRequest.Method} question with only one answer");
        }
    }
}