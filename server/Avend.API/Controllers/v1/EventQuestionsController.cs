using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Responses;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qoden.Validation;
using Swashbuckle.SwaggerGen.Annotations;

namespace Avend.API.Controllers.v1
{
    [Authorize]
    [Route("api/v1/events/{event_uid}/questions")]
    public class EventQuestionsController : Controller
    {
        public EventQuestionsController(QuestionsService service)
        {
            Assert.Argument(service, nameof(service)).NotNull();
            EventsQuestionsService = service;
        }

        public QuestionsService EventsQuestionsService { get; set; }

        /// <summary>
        /// Returns event questions array
        /// </summary>
        /// <remarks>Returns filtered and sorted array of event questions according to filter and other parameters\n</remarks>
        /// <param name="eventUid">Event Guid parsed as string to properlyreport on parsing error.</param>
        /// <response code="200">An array of event questions with their option values</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [SwaggerOperation("GetEventQuestions")]
        [ProducesResponseType(typeof(OkListResponse<EventQuestionDto>), 200)]
        public IActionResult GetEventQuestions([FromRoute(Name = "event_uid")] Guid eventUid)
        {
            var questions = EventsQuestionsService.EventQuestions(eventUid);
            return Ok(OkResponse.WithList(questions, questions.Count));
        }

        /// <summary>
        /// Retrieve event question with given uid
        /// </summary>
        /// <remarks>Returns event question record or error if the event question is not found or is not accessible\n</remarks>
        /// <param name="eventUid">Guid identifying the event</param>
        /// <param name="questionUid">Guid identifying the event question</param>
        /// <response code="200">Event question with given uid was found</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet("{question_uid}")]
        [SwaggerOperation("GetEventQuestionByUID")]
        [ProducesResponseType(typeof(OkResponse<EventQuestionDto>), 200)]
        public IActionResult GetEventQuestionByUid(
            [FromRoute(Name = "event_uid")] Guid eventUid,
            [FromRoute(Name = "question_uid")] Guid questionUid)
        {
            var question = EventsQuestionsService.EventQuestion(eventUid, questionUid);
            return Ok(OkResponse.WithData(question));
        }

        /// <summary>
        /// Adds a new event question
        /// </summary>
        /// <remarks>Adds a new event question based on parameters. Returns uid for newly created event question.</remarks>
        /// <param name="eventUid">Guid identifying the event</param>
        /// <param name="newEventQuestionDto">Event question object to be added to the database</param>
        /// <response code="200">New record was created successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost]
        [SwaggerOperation("CreateEventQuestion")]
        [ProducesResponseType(typeof(OkResponse<Guid>), 200)]
        public async Task<IActionResult> CreateEventQuestion(
            [FromRoute(Name = "event_uid")] Guid eventUid,
            [FromBody] EventQuestionDto newEventQuestionDto)
        {
            var questions = await EventsQuestionsService.CreateQuestion(eventUid, newEventQuestionDto);
            return Ok(OkResponse.WithData(questions));
        }

        /// <summary>
        /// Updates the event question identified by uid with new data
        /// </summary>
        /// <remarks>Updates the event question based on parameters. Returns true on success.</remarks>
        /// <param name="eventUid">Guid identifying the event</param>
        /// <param name="questionUid">Guid identifying the event question</param>
        /// <param name="updatedQuestionDto">Updated event question data</param>
        /// <response code="200">Record was updated successfully</response>
        [HttpPut("{question_uid}")]
        [SwaggerOperation("UpdateEventQuestion")]
        [ProducesResponseType(typeof(OkResponse<string>), 200)]
        public async Task<IActionResult> UpdateEventQuestion(
            [FromRoute(Name = "event_uid")] Guid eventUid,
            [FromRoute(Name = "question_uid")] Guid questionUid,
            [FromBody] EventQuestionDto updatedQuestionDto)
        {
            var question = await EventsQuestionsService.UpdateQuestion(eventUid, questionUid, updatedQuestionDto);
            return Ok(OkResponse.WithData(question));
        }

        /// <summary>
        /// Deletes event question by its uid
        /// </summary>
        /// <remarks>Returns true if successfull.\n</remarks>
        /// 
        /// <param name="eventUid">Uid identifying the event</param>
        /// <param name="questionUid">Uid identifying the event question</param>
        /// 
        /// <response code="200">Record was deleted successfully</response>
        [HttpDelete("{question_uid}")]
        [SwaggerOperation("DeleteEventQuestion")]
        [ProducesResponseType(typeof(OkResponse<string>), 200)]
        public async Task<IActionResult> DeleteEventQuestion(
            [FromRoute(Name = "event_uid")] Guid eventUid,
            [FromRoute(Name = "question_uid")] Guid questionUid)
        {
            var question = await EventsQuestionsService.DeleteQuestion(eventUid, questionUid);
            return Ok(OkResponse.WithData(question));
        }

        /// <summary>
        /// Moves event question to new position
        /// </summary>
        /// 
        /// <param name="eventUid">Uid identifying the event</param>
        /// <param name="questionUid">Uid identifying the event question</param>
        /// <param name="newPosition">New event question position</param>
        /// 
        /// <response code="200">Record updated</response>
        [HttpPatch("{question_uid}/move")]
        [SwaggerOperation("MoveEventQuestion")]
        [ProducesResponseType(typeof(OkListResponse<EventQuestionDto>), 200)]
        public async Task<IActionResult> MoveEventQuestion(
            [FromRoute(Name = "event_uid")] Guid eventUid,
            [FromRoute(Name = "question_uid")] Guid questionUid,
            [FromBody] int newPosition)
        {
            var questions = await EventsQuestionsService.Move(eventUid, questionUid, newPosition);
            return Ok(OkResponse.WithList(questions, questions.Count));
        }

        /// <summary>
        /// Mass delete event question
        /// </summary>
        /// <param name="eventUid">Uid identifying the event</param>
        /// <param name="questionUids">Uids identifying the event question</param>
        /// <response code="200">Record updated</response>
        [HttpPost("delete")]
        [SwaggerOperation("MassDeleteEventQuestions")]
        [ProducesResponseType(typeof(OkResponse<string>), 200)]
        public async Task<IActionResult> MassDeleteEventQuestions(
            [FromRoute(Name = "event_uid")] Guid eventUid,
            [FromBody] Guid [] questionUids)
        {
            var questions = await EventsQuestionsService.DeleteQuestions(eventUid, questionUids);
            return Ok(OkResponse.WithData(questions.Select(x => x.Uid)));
        }


    }
}
