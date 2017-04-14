using System;
using System.Collections.Generic;
using System.Linq;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class EventQuestions
    {
        private readonly UserContext _user;
        private readonly AvendDbContext _db;
        private readonly QuestionsRepository _repo;
        private EventRecord _event;

        public EventQuestions(AvendDbContext db, UserContext user)
        {
            _user = user;
            _repo = new QuestionsRepository(db);
            _db = db;
        }

        public void Find(Guid eventUid)
        {
            var events = new EventsRepository(_db) {Scope = _user.AvailableEvents()};
            _event = events.FindEventByUid(eventUid);
            Check.Value(_event, "event").NotNull("event not found", AvendErrors.NotFound);
        }

        public EventQuestionRecord AddQuestion(EventQuestionDto dto)
        {
            Assert.State(_event).NotNull();

            var total = _repo.TotalQuestions(_event, _user.UserId);
            Check.Value(total).Less(10, "Event can only have {Max} questions");

            var question = _repo.CreateQuestion(_event, _user.UserId);
            question.Position = total;
            return UpdateQuestion(dto, question);
        }

        public EventQuestionRecord UpdateQuestion(Guid questionUid, EventQuestionDto dto)
        {
            Assert.State(_event).NotNull();

            var question = _repo.EventQuestion(_event, questionUid, _user.UserId);
            Check.Value(question, "question", AvendErrors.NotFound).NotNull();
            Check.Value(_repo.QuestionHasAnswers(question), "question")
                .IsFalse("Event has lead and it questions locked for modification", AvendErrors.Forbidden);

            foreach (var choice in question.Choices)
            {
                _repo.DeleteChoice(choice);
            }

            UpdateQuestion(dto, question);
            return question;
        }

        public EventQuestionRecord DeleteQuestion(Guid questionUid)
        {
            Assert.State(_event).NotNull();

            var questions = Questions().ToList();
            var question = questions.Find(x => x.Uid == questionUid);
            Check.Value(question, "question", AvendErrors.NotFound).NotNull();
            questions.Remove(question);
            _repo.DeleteQuestion(question);
            FixOrder(questions);

            return question;
        }

        public EventQuestionRecord[] Delete(Guid[] questionUids)
        {
            Assert.State(_event).NotNull();

            var questions = Questions().Where(x => questionUids.Contains(x.Uid)).ToList();
            foreach (var question in questions)
            {
                _repo.DeleteQuestion(question);
            }
            return questions.ToArray();
        }

        public EventQuestionRecord Move(Guid questionUid, int newPosition)
        {
            Assert.State(_event).NotNull();

            var questions = Questions().ToList();
            var question = questions.Find(x => x.Uid == questionUid);
            Check.Value(question, "question", AvendErrors.NotFound).NotNull();

            Check.Value(newPosition, "new_position")
                .Less(TotalQuestions())
                .GreaterOrEqualTo(0);

            questions.RemoveAt(question.Position);
            questions.Insert(newPosition, question);
            FixOrder(questions);
            question.Position = newPosition;
            return question;
        }

        private static void FixOrder(List<EventQuestionRecord> questions)
        {
            for (var i = 0; i < questions.Count; ++i)
            {
                questions[i].Position = i;
            }
        }

        public int TotalQuestions()
        {
            Assert.State(_event).NotNull();

            return _repo.TotalQuestions(_event, _user.UserId);
        }

        public IEnumerable<EventQuestionRecord> Questions()
        {
            Assert.State(_event).NotNull();

            return _repo.EventQuestions(_event, _user.UserId);
        }

        public EventQuestionRecord Question(Guid questionUid)
        {
            Assert.State(_event).NotNull();

            var question = Questions().FirstOrDefault(x => x.Uid == questionUid);
            Check.Value(question, "question", AvendErrors.NotFound).NotNull();
            return question;
        }

        private EventQuestionRecord UpdateQuestion(EventQuestionDto dto, EventQuestionRecord question)
        {
            var validator = new Validator();
            validator.CheckValue(dto.Text, "text").IsShortText();
            validator.CheckValue(dto.Choices, "answers").MinLength(2).MaxLength(10);
            for (var i = 0; i < dto.Choices.Count; ++i)
            {
                var choice = dto.Choices[i];
                validator.CheckValue(choice.Text, $"answers[{i}].text").IsShortText();
            }
            validator.Throw();

            question.Text = dto.Text;
            question.Choices = dto.Choices
                .Select((x, i) => _repo.CreateChoice(question, new AnswerChoiceRecord()
                {
                    Text = x.Text,
                    Position = i
                }))
                .ToList();
            return question;
        }
    }
}