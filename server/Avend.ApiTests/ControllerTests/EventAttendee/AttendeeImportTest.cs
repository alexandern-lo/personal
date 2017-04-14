using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Events;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Avend.ApiTests.ControllerTests.EventAttendee
{
    [TestClass]
    public class AttendeeImportTest : AttendeesControllerTestBase
    {
        protected CsvFileBuilder CsvBuilder;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            CsvBuilder = new CsvFileBuilder();
            CsvBuilder.Reset();
        }

        [TestMethod]
        public async Task ImportAttendee()
        {
            var dto = SampleAttendee();
            CsvBuilder.Add(dto);
            var response = await AlexSA.PostAsync($"events/{EventUid}/attendees/import", CsvBuilder.CsvContent);
            response.IsSuccessStatusCode.Should().BeTrue();

            var attendees = await AlexSA.GetJsonAsync($"events/{EventUid}/attendees")
                .AvendListResponse<AttendeeDto>(1, HttpStatusCode.OK, "1 attendee created");
            attendees.Count.Should().Be(1);
            var attendee = attendees[0];
            attendee.FirstName.Should().Be(dto.FirstName);
            attendee.LastName.Should().Be(dto.LastName);
            attendee.Title.Should().Be(dto.Title);
            attendee.Company.Should().Be(dto.Company);
            attendee.Phone.Should().Be(dto.Phone);
            attendee.Email.Should().Be(dto.Email);
            attendee.Country.Should().Be(dto.Country);
            attendee.State.Should().Be(dto.State);
            attendee.City.Should().Be(dto.City);
            attendee.ZipCode.Should().Be(dto.ZipCode);
        }

        [TestMethod]
        public async Task CategoriesCreatedDuringImport()
        {
            var categories = new[] {Prop1.Category.Name, Prop2.Category.Name, "Category 3"};
            var options = new[] {"New Option in Category 1", "Opt2.1", "New Opt in Category 3"};
            CsvBuilder.Reset(categories);
            CsvBuilder.Add(SampleAttendee(), options);
            CsvBuilder.Add(SampleAttendee(), options);

            await AlexSA.SendDataAsync(CsvBuilder.CsvRequest($"events/{EventUid}/attendees/import"))
                .Response();

            var attendees = await AlexSA.GetJsonAsync($"events/{EventUid}/attendees")
                .AvendListResponse<AttendeeDto>(2, HttpStatusCode.OK, "2 attendee created");

            var attendee1 = attendees[0].CategoryValues;
            attendee1.Select(x => x.CategoryName).Should()
                .BeEquivalentTo(categories);
            attendee1.Select(x => x.OptionName).Should()
                .BeEquivalentTo(options);

            var attendee2 = attendees[0].CategoryValues;
            attendee2.Select(x => x.CategoryName).Should()
                .BeEquivalentTo(categories);
            attendee2.Select(x => x.OptionName).Should()
                .BeEquivalentTo(options);

            attendee1.Select(x => x.CategoryUid).Should()
                .BeEquivalentTo(attendee2.Select(x => x.CategoryUid), "categories are shared between all attendees");
            attendee1.Select(x => x.OptionUid).Should()
                .BeEquivalentTo(attendee2.Select(x => x.OptionUid), "all attendees use same options");
        }

        [TestMethod]
        public async Task RequiredFields()
        {
            CsvBuilder.Add(SampleAttendee(x => x.FirstName = ""));
            await BadRequest();

            CsvBuilder.Add(SampleAttendee(x => x.LastName = ""));
            await BadRequest();
        }

        [TestMethod]
        public async Task ErrorReport()
        {
            CsvBuilder.Add(SampleAttendee());
            CsvBuilder.Add(SampleAttendee(x =>
            {
                x.FirstName = "";
                x.Title = "Invalid 1";
            }));
            CsvBuilder.Add(SampleAttendee(x =>
            {
                x.LastName = "";
                x.Title = "Invalid 2";
            }));

            var response = await AlexSA.PostAsync($"events/{EventUid}/attendees/import", CsvBuilder.CsvContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await AlexSA.GetJsonAsync($"events/{EventUid}/attendees")
                .AvendListResponse<AttendeeDto>(0, HttpStatusCode.OK, "no attendees created");

            var responseBody = await response.Content.ReadAsStringAsync();
            var errorReport = JsonConvert.DeserializeObject<ImportReport>(responseBody);
            var errors = errorReport.InvalidAttendees;
            errors.Count.Should().Be(2);
            errors.Select(x => x.Line).Should().Equal(2, 3);
            errors.Select(x => x.Attendee.Title).Should().Equal("Invalid 1", "Invalid 2");
        }

        [TestMethod]
        public async Task OnlySaCanImport()
        {
            CsvBuilder.Add(SampleAttendee());

            var response = await BobTA.PostAsync($"events/{EventUid}/attendees/import", CsvBuilder.CsvContent);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            response = await CecileSU.PostAsync($"events/{EventUid}/attendees/import", CsvBuilder.CsvContent);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        private static AttendeeDto SampleAttendee(Action<AttendeeDto> postProcessor = null)
        {
            var attendee = new AttendeeDto()
            {
                FirstName = "Andrew",
                LastName = "Verbin",
                Title = "CEO",
                Company = "Studio Mobile",
                Phone = "+79219736635",
                Email = "andery@studiomobile.ru",
                Country = "Russia",
                State = "LEN",
                City = "Saint-Peterburg",
                ZipCode = "197198"
            };
            postProcessor?.Invoke(attendee);
            return attendee;
        }


        private async Task BadRequest()
        {
            var response = await AlexSA.PostAsync($"events/{EventUid}/attendees/import", CsvBuilder.CsvContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            CsvBuilder.Reset();
        }

        public class CsvFileBuilder
        {
            public readonly string[] Header =
            {
                "first_name", "last_name", "title", "company", "phone", "email",
                "country", "state", "city", "zipcode"
            };

            private List<string[]> _lines;
            private string[] _categories;

            public CsvFileBuilder()
            {
                Reset();
            }

            public void Reset(params string[] categories)
            {
                _categories = categories;
                _lines = new List<string[]>();
            }

            public void Add(AttendeeDto attendee, params string[] categories)
            {
                if (categories.Length != _categories.Length) throw new ArgumentException(nameof(categories));
                var attendeeColumns = new[]
                {
                    attendee.FirstName, attendee.LastName, attendee.Title, attendee.Company, attendee.Phone,
                    attendee.Email, attendee.Country, attendee.State, attendee.City, attendee.ZipCode
                };
                _lines.Add(attendeeColumns.Concat(categories).ToArray());
            }

            public string Csv
            {
                get
                {
                    var header = Header.Concat(_categories).ToArray().Join(",");
                    var csv = new StringBuilder();
                    csv.AppendLine(header);
                    foreach (var line in _lines)
                    {
                        csv.AppendLine(line.Join(","));
                    }
                    return csv.ToString();
                }
            }

            public HttpRequestMessage CsvRequest(string url)
            {
                return new HttpRequestMessage()
                {
                    RequestUri = new Uri(url, UriKind.Relative),
                    Content = CsvContent,
                    Method = HttpMethod.Post
                };
            }

            public MultipartFormDataContent CsvContent
            {
                get
                {
                    var csvContent = new MultipartFormDataContent();
                    var data = new MemoryStream(Encoding.UTF8.GetBytes(Csv));
                    csvContent.Add(new StreamContent(data), "attendees", "attendees.csv");
                    return csvContent;
                }
            }
        }
    }
}