using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Responses;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;
using Error = Avend.API.Infrastructure.Responses.Error;

namespace Avend.API.Services.Events
{
    [DataContract]
    public class AttendeeAndError : ErrorResponse
    {
        [DataMember(Name = "line")] public int Line;
        [DataMember(Name = "attendee")] public AttendeeDto Attendee;

        public AttendeeAndError(List<Error> errors, int line, AttendeeDto attendee) : base(errors)
        {
            Line = line;
            Attendee = attendee;
        }
    }

    [DataContract]
    public class ImportReport
    {
        [DataMember(Name = "invalid_attendees")]
        public List<AttendeeAndError> InvalidAttendees { get; set; } = new List<AttendeeAndError>();
    }

    public class AttendeesService
    {
        private readonly DbContextOptions<AvendDbContext> _dbOptions;
        private readonly UserContext _user;

        public AttendeesService(DbContextOptions<AvendDbContext> dbOptions, UserContext user)
        {
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();
            Assert.Argument(user, nameof(user)).NotNull();
            _dbOptions = dbOptions;
            _user = user;
        }

        public async Task<ImportReport> ImportFromCsv(Guid eventUid, IFormFile attendees)
        {
            Check.Value(_user.Role, onError: AvendErrors.Forbidden).EqualsTo(UserRole.SuperAdmin);
            Check.Value(attendees, "attendees").NotNull();

            using (var db = new AvendDbContext(_dbOptions))
            {
                using (var importer = new AttendeeCsvImporter(db, eventUid))
                {
                    await importer.PrepareToImport(attendees);
                    while (importer.Next())
                    {
                        await importer.ImportRecord();
                    }
                    if (importer.NoErrors)
                    {
                        await db.SaveChangesAsync();
                    }
                    return importer.Errors;
                }
            }
        }
    }
}