using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class QuestionsService
    {
        private readonly DbContextOptions<AvendDbContext> _dbOptions;
        private readonly UserContext _user;

        public QuestionsService(DbContextOptions<AvendDbContext> dbOptions, UserContext user)
        {
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();
            Assert.Argument(user, nameof(user)).NotNull();
            _dbOptions = dbOptions;
            _user = user;
        }

        public List<EventQuestionDto> EventQuestions(Guid eventUid)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var questions = new EventQuestions(db, _user);
                questions.Find(eventUid);
                return questions.Questions()
                    .Select(x => EventQuestionDto.From(x, eventUid))
                    .ToList();
            }
        }

        public EventQuestionDto EventQuestion(Guid eventUid, Guid questionUid)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var questions = new EventQuestions(db, _user);
                questions.Find(eventUid);
                var x = questions.Question(questionUid);
                return EventQuestionDto.From(x, eventUid);
            }
        }

        public async Task<EventQuestionDto> CreateQuestion(Guid eventUid, EventQuestionDto newEventQuestionDto)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var questions = new EventQuestions(db, _user);
                questions.Find(eventUid);
                var question = questions.AddQuestion(newEventQuestionDto);
                await db.SaveChangesAsync();
                return EventQuestionDto.From(question, eventUid);
            }
        }

        public async Task<EventQuestionDto> UpdateQuestion(Guid eventUid, Guid questionUid, EventQuestionDto dto)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var questions = new EventQuestions(db, _user);
                questions.Find(eventUid);
                var question = questions.UpdateQuestion(questionUid, dto);
                await db.SaveChangesAsync();
                return EventQuestionDto.From(question, eventUid);
            }
        }

        public async Task<EventQuestionDto> DeleteQuestion(Guid eventUid, Guid questionUid)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var questions = new EventQuestions(db, _user);
                questions.Find(eventUid);
                var question = questions.DeleteQuestion(questionUid);
                await db.SaveChangesAsync();
                return EventQuestionDto.From(question, eventUid);
            }
        }

        public async Task<List<EventQuestionDto>> Move(Guid eventUid, Guid questionUid, int position)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var questions = new EventQuestions(db, _user);
                questions.Find(eventUid);
                questions.Move(questionUid, position);
                await db.SaveChangesAsync();
                return questions.Questions()
                    .Select(x => EventQuestionDto.From(x, eventUid))
                    .ToList();
            }
        }

        public async Task<EventQuestionDto[]> DeleteQuestions(Guid eventUid, Guid[] questionUids)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var questions = new EventQuestions(db, _user);
                questions.Find(eventUid);
                var deletedQuestions = questions.Delete(questionUids);
                await db.SaveChangesAsync();
                return deletedQuestions.Select(x => EventQuestionDto.From(x, eventUid)).ToArray();
            }
        }
    }
}