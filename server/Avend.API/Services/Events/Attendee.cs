using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Validation.Util;
using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class Attendee
    {
        private readonly AvendDbContext _db;

        public static PropertyInfo[] Fields()
        {
            return typeof(AttendeeRecord).GetProperties()
                .Where(x => x.GetCustomAttribute<ColumnAttribute>() != null)
                .ToArray();
        }

        public Attendee(AvendDbContext db)
        {
            Assert.Argument(db, nameof(db)).NotNull();
            _db = db;
            Validator = new Validator();
        }

        public Validator Validator { get; set; }
        public AttendeeRecord Data { get; set; }

        public void New(EventRecord @event)
        {
            Assert.Argument(@event, "event").NotNull();
            Data = new AttendeeRecord
            {
                Uid = Guid.NewGuid(),
                EventRecord = @event
            };
        }

        public void Update(AttendeeDto dto)
        {
            Assert.State(Data, nameof(Data)).NotNull();

            dto.FirstName = dto.FirstName ?? Data.FirstName;
            Data.FirstName = Validator.CheckDataMember(dto, x => x.FirstName).IsShortText().Value;

            dto.LastName = dto.LastName ?? Data.LastName;
            Data.LastName = Validator.CheckDataMember(dto, x => x.LastName).IsShortText().Value;

            if (dto.Email != null)
            {
                Data.Email = Validator.CheckDataMember(dto, x => x.Email).IsEmail().Value;
            }
            if (dto.Title != null)
            {
                Data.Title = Validator.CheckDataMember(dto, x => x.Title).NotEmpty().IsShortText().Value;
            }
            if (dto.Company != null)
            {
                Data.Company = Validator.CheckDataMember(dto, x => x.Company).NotEmpty().IsShortText().Value;
            }
            if (dto.Phone != null)
            {
                Data.Phone = Validator.CheckDataMember(dto, x => x.Phone).NotEmpty().IsShortText().Value;
            }
            if (dto.Country != null)
            {
                Data.Country = Validator.CheckDataMember(dto, x => x.Country).IsValidCountry().Value;
            }
            if (dto.State != null)
            {
                Data.State = Validator.CheckDataMember(dto, x => x.State).NotEmpty().IsShortText().Value;
            }
            if (dto.City != null)
            {
                Data.City = Validator.CheckDataMember(dto, x => x.City).NotEmpty().IsShortText().Value;
            }
            if (dto.ZipCode != null)
            {
                Data.ZipCode = Validator.CheckDataMember(dto, x => x.ZipCode).NotEmpty().IsShortText().Value;
            }
        }

        public void Create()
        {
            _db.Attendees.Add(Data);
        }

        public void AddOption(AttendeeCategoryRecord category, AttendeeCategoryOption categoryOption)
        {
            Assert.State(Data, nameof(Data)).NotNull();

            Data.Values.Add(new AttendeeCategoryValue()
            {
                Category = category,
                AttendeeCategoryOption = categoryOption
            });
        }
    }
}