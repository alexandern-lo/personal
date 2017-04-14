using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class AttendeeCsvImporter : IDisposable
    {
        private static readonly CsvConfiguration CsvReaderConfiguration = new CsvConfiguration()
        {
            TrimHeaders = true,
            TrimFields = true,
            AllowComments = true,
            IgnoreBlankLines = true,
            IgnoreReferences = true
        };

        private readonly EventRecord _event;
        public ImportReport Errors { get; private set; }
        public bool NoErrors => Errors.InvalidAttendees.Count == 0;

        private int _line;
        private CsvReader _csvReader;
        private readonly AttendeeCategoriesRepository _categoriesRepo;
        private Dictionary<string, AttendeeCategoryRecord> _categories;

        public AttendeeCsvImporter(AvendDbContext db, Guid eventUid)
        {
            Assert.Argument(db, nameof(db)).NotNull();

            var eventsRepository = new EventsRepository(db);
            _categoriesRepo = new AttendeeCategoriesRepository(db);
            _event = eventsRepository.FindEventByUid(eventUid);
            Check.Value(_event, "event", AvendErrors.NotFound).NotNull("{Key} not found");
        }

        public async Task PrepareToImport(IFormFile file)
        {
            Assert.Argument(file, nameof(file)).NotNull();
            Assert.State(_csvReader, "csvReader").IsNull("PrepareToImport already called");

            Check.Value(file.Length, "attendees").LessOrEqualTo(1024 * 1024 * 2, "CSV file maximum size is 2Mb");
            var reader = new StreamReader(file.OpenReadStream());
            try
            {
                var chars = new char[file.Length];
                await reader.ReadBlockAsync(chars, 0, chars.Length);
                _csvReader = new CsvReader(new StringReader(new string(chars)), CsvReaderConfiguration);
                var hasHeader = _csvReader.ReadHeader();
                Check.Value(hasHeader, "attendees").IsTrue("CSV file must have header");
                _categories = await PrepareCategories();
                Errors = new ImportReport();
                _line = 1;
            }
            catch (CsvHelperException e)
            {
                var error = new Error("attendee", $"Cannot parse CSV file. Error: {e.Message}");
                throw new ErrorException(error);
            }
            catch (Exception)
            {
                reader.Dispose();
                throw;
            }
        }

        public bool Next()
        {
            Assert.State(_csvReader, "csvReader").NotNull();
            return _csvReader.Read();
        }

        public async Task ImportRecord()
        {
            try
            {
                var attendee = new Attendee(_categoriesRepo.Db);
                attendee.New(_event);

                var record = new AttendeeDto()
                {
                    FirstName = _csvReader.GetField("first_name"),
                    LastName = _csvReader.GetField("last_name"),
                    Title = _csvReader.GetField("title"),
                    Company = _csvReader.GetField("company"),
                    Phone = _csvReader.GetField("phone"),
                    Email = _csvReader.GetField("email"),
                    Country = _csvReader.GetField("country"),
                    State = _csvReader.GetField("state"),
                    City = _csvReader.GetField("city"),
                    ZipCode = _csvReader.GetField("zipcode"),
                };

                attendee.Update(record);

                if (attendee.Validator.IsValid)
                {
                    attendee.Create();
                }
                else
                {
                    var errors = attendee.Validator.Errors
                        .Select(x => x.ToAvendError())
                        .ToList();
                    Errors.InvalidAttendees.Add(new AttendeeAndError(errors, _line, record));
                }

                foreach (var header in _csvReader.FieldHeaders)
                {
                    //All relevant categories MUST be loaded/created in PrepareToImport
                    if (!_categories.ContainsKey(header)) continue;
                    var category = _categories[header];
                    var categoryOptionName = _csvReader.GetField(header);
                    var categoryOption = category.Options.FirstOrDefault(x => x.Name == categoryOptionName);
                    if (categoryOption == null)
                    {
                        categoryOption = await _categoriesRepo.CreateCategoryOption(category, categoryOptionName);
                    }
                    attendee.AddOption(category, categoryOption);
                }

                _line++;
            }
            catch (CsvHelperException e)
            {
                var error = new Error("attendee", $"Cannot parse CSV file. Error: {e.Message}");
                throw new ErrorException(error);
            }
        }

        public void Dispose()
        {
            _csvReader?.Dispose();
        }

        /// <summary>
        /// Load or create categories listed in CSV file header. Category is anything which is not <see cref="AttendeeCategoryRecord"/> column.
        /// </summary>
        /// <returns>Mapping from category name to category record</returns>
        private async Task<Dictionary<string, AttendeeCategoryRecord>> PrepareCategories()
        {
            var attendeeFields = Attendee.Fields()
                .Select(x => x.GetCustomAttribute<ColumnAttribute>().Name);
            var categoryNames = _csvReader.FieldHeaders
                .Where(x => !attendeeFields.Contains(x))
                .ToList();

            var categories = await _categoriesRepo.FindCategoriesByNames(_event, categoryNames);
            foreach (var category in categoryNames)
            {
                if (categories.All(x => x.Name != category))
                {
                    var categoryRecord = _categoriesRepo.CreateCategory(_event);
                    categoryRecord.Name = category;
                    categories.Add(categoryRecord);
                }
            }

            return categories.ToDictionary(x => x.Name);
        }
    }
}