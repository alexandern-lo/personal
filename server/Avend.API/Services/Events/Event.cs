using System;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class Event
    {
        private readonly EventsRepository _repo;
        private EventRecord _event;
        private readonly UserContext _userContext;

        public Event(EventsRepository repo, UserContext userContext)
        {
            Assert.Argument(repo, nameof(repo)).NotNull();
            Assert.Argument(userContext, nameof(userContext)).NotNull();
            _repo = repo;
            _userContext = userContext;
            Validator = new Validator();
        }

        public Validator Validator { get; }

        public EventRecord Data => _event;

        public void Find(Guid eventUid)
        {
            Assert.State(_event, nameof(_event)).IsNull();

            Check.Value(_userContext.Role, onError: AvendErrors.Forbidden)
                .In(new[] {UserRole.Admin, UserRole.SeatUser, UserRole.SuperAdmin});

            _repo.Scope = _userContext.OwnEvents();
            _event = _repo.FindEventByUid(eventUid);
            Check.Value(_event, "event", AvendErrors.NotFound).NotNull();
        }

        public void Create()
        {
            _event = _repo.CreateEvent(_userContext.UserUid, _userContext.Subscription?.Uid);
            if (_userContext.Role == UserRole.SuperAdmin)
            {
                _event.Type = EventRecord.EventTypeConference;
            }
            else
            {
                _event.Type = EventRecord.EventTypePersonal;
            }
        }

        public void Update(EventDto dto)
        {
            Assert.Argument(dto, nameof(dto)).NotNull();
            Assert.State(_event, nameof(_event)).NotNull();

            //since super admins create conference events they cannot create 1 day recurring events
            if (_userContext.Role == UserRole.SuperAdmin)
            {
                dto.Recurring = false;
            }

            Validator.Clear();

            Validator.CheckValue(_event.Type, "event_type")
                .In(new[] {EventRecord.EventTypeConference, EventRecord.EventTypePersonal});

            if (dto.LogoUrl != null)
                _event.LogoUrl = dto.LogoUrl;

            if (dto.WebsiteUrl != null)
                _event.WebsiteUrl = dto.WebsiteUrl;

            if (dto.Name != null)
            {
                _event.Name = dto.Name;
                Validator.CheckValue(_event.Name, "name").IsShortText();
            }

            if (dto.VenueName != null)
            {
                _event.VenueName = dto.VenueName;
                Validator.CheckValue(_event.VenueName, "venue_name").IsShortText();
            }

            if (dto.Country != null)
            {
                _event.Country = dto.Country;
                Validator.CheckValue(_event.Country, "country").IsValidCountry();
            }

            if (dto.State != null)
            {
                _event.State = dto.State;
                Validator.CheckValue(_event.State, "state").IsShortText();
            }

            if (dto.City != null)
            {
                _event.City = dto.City;
                Validator.CheckValue(_event.City, "city").IsShortText();
            }

            if (dto.ZipCode != null)
            {
                _event.ZipCode = dto.ZipCode;
                Validator.CheckValue(_event.ZipCode, "zip_code").IsShortText();
            }

            if (dto.Address != null)
            {
                _event.Address = dto.Address;
                Validator.CheckValue(_event.Address, "address").IsShortText();
            }

            if (dto.Description != null)
            {
                _event.Description = dto.Description;
                Validator.CheckValue(_event.Description, "description").NotEmpty().MaxLength(2048);
            }

            if (dto.Industry != null)
            {
                _event.Industry = dto.Industry;
                Validator.CheckValue(_event.Industry, "industry").IsValidIdustry();
            }

            if (dto.StartDate.HasValue)
            {
                var startDate = dto.StartDate.Value;
                dto.StartDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
                _event.StartDate = dto.StartDate.Value;
            }
            Validator.CheckValue(_event.StartDate, "start_date").HasValue();
            
            _event.Recurring = dto.Recurring;
            //Recurring events does not have EndDate
            if (!_event.Recurring)
            {
                DateTime? endDate = null;
                //personal events alway ends same date when started
                if (_event.Type == EventRecord.EventTypePersonal)
                {
                    endDate = _event.StartDate.Value;
                }
                else if (dto.EndDate.HasValue)
                {
                    endDate = dto.EndDate.Value;
                }

                if (endDate.HasValue)
                {
                    _event.EndDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);
                    Validator.CheckValue(_event.EndDate, "end_date")
                        .HasValue()
                        .Greater(_event.StartDate.Value);
                }
            }
            else
            {
                _event.EndDate = null;
            }
        }

        public void Delete()
        {
            Assert.State(_event, nameof(_event)).NotNull();

            _repo.DeleteEvent(_event);
        }
    }
}