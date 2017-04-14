using System;
using System.Collections.Generic;
using System.Linq;
using Avend.API.Model;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class QuestionsRepository
    {
        private AvendDbContext _db;

        public QuestionsRepository(AvendDbContext db)
        {
            Assert.Argument(db, nameof(db)).NotNull();
            _db = db;
        }

        public IEnumerable<EventQuestionRecord> EventQuestions(EventRecord @event, long? userId)
        {
            return _db.Questions
                .Include(x => x.Choices)
                .NotDeleted()
                .Where(x => x.EventId == @event.Id && x.UserId == userId)
                .OrderBy(x => x.Position);
        }

        public EventQuestionRecord EventQuestion(EventRecord @event, Guid questionUid, long? userId)
        {
            return _db.Questions
                .Include(x => x.Choices)
                .NotDeleted()
                .FirstOrDefault(x => x.EventId == @event.Id && x.Uid == questionUid && x.UserId == userId);
        }

        public int TotalQuestions(EventRecord @event, long? userId)
        {
            return _db.Questions
                .NotDeleted()
                .Count(x => x.EventId == @event.Id && x.UserId == userId);
        }

        public EventQuestionRecord CreateQuestion(EventRecord @event, long? userId)
        {
            var entity = _db.Questions.Add(new EventQuestionRecord()
            {
                Uid = Guid.NewGuid(),
                Event = @event,
                UserId = userId.GetValueOrDefault()
            });
            return entity.Entity;
        }

        public AnswerChoiceRecord CreateChoice(EventQuestionRecord question, AnswerChoiceRecord choice)
        {
            question.Uid = Guid.NewGuid();
            choice.Question = question;
            choice.Uid = Guid.NewGuid();
            choice.CreatedAt = DateTime.Now;
            var entity = _db.AnswerChoices.Add(choice);
            return entity.Entity;
        }

        public void DeleteQuestion(EventQuestionRecord question)
        {
            if (QuestionHasAnswers(question))
            {
                question.Deleted = true;
            }
            else
            {
                _db.Questions.Remove(question);    
            }
        }

        public void DeleteChoice(AnswerChoiceRecord choice)
        {
            if (ChoiceHasAnswers(choice))
            {
                _db.AnswerChoices.Remove(choice);
            }
            else
            {
                choice.Deleted = true;
            }
        }

        private bool ChoiceHasAnswers(AnswerChoiceRecord choice)
        {
            return _db.LeadQuestionAnswersTable.Any(x => x.EventAnswerId == choice.Id);
        }

        public bool QuestionHasAnswers(EventQuestionRecord question)
        {
            return _db.LeadQuestionAnswersTable.Any(x => x.EventQuestionId == question.Id);
        }
    }
}