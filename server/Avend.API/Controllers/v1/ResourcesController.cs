using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Responses;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services;
using Avend.API.Services.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Qoden.Validation;
using Swashbuckle.SwaggerGen.Annotations;

namespace Avend.API.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/v1/users/resources")]
    [Authorize]
    public class ResourcesController : BaseController
    {
        private readonly DbContextOptions<AvendDbContext> _dbOptions;
        private readonly AppSettings _appSettings;
        private readonly ResourcesService _service;

        public ResourcesController(
            ResourcesService service,
            DbContextOptions<AvendDbContext> dbOptions,
            AppSettings appSettings)
            : base(dbOptions)
        {
            Assert.Argument(service, nameof(service)).NotNull();
            _dbOptions = dbOptions;
            _appSettings = appSettings;
            _service = service;
        }

        [NonAction]
        private EventRecord GetEventRecordByUid(Guid? eventUid)
        {
            EventRecord eventRecord;

            if (eventUid == null)
                return null;

            using (var db = new AvendDbContext(_dbOptions))
            {
                var queryEvent = from recEventRecord in db.EventsTable
                    where recEventRecord.Uid == eventUid
                    select recEventRecord;

                eventRecord = queryEvent.FirstOrDefault();
            }

            return eventRecord;
        }

        /// <summary>
        /// Returns resources array
        /// </summary>
        /// <remarks>Returns filtered and sorted array of resources according to filter and other parameters\n</remarks>
        /// <param name="filter">Query string to be matched by first or last name of the resource record.</param>
        /// <param name="user">filter resources by user uid, only valid for TA</param>
        /// <param name="tenant">filter resources which belong to given TA and it SU, only available to SA</param>
        /// <param name="pageNumber">page number</param>
        /// <param name="recordsPerPage">records per page</param>
        /// <param name="sortField">sort field: 'name' or 'created_at' default 'name'</param>
        /// <param name="sortOrder">sort order, default desc</param>
        /// <response code="200">An array of resources</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [SwaggerOperation("GetResources")]
        [ProducesResponseType(typeof(OkListResponse<ResourceDto>), 200)]
        public async Task<OkListResponse<ResourceDto>> GetResources(
            [FromQuery(Name = "q")] string filter,
            [FromQuery(Name = "user")] Guid? user = null,
            [FromQuery(Name = "tenant")] Guid? tenant = null,
            [FromQuery(Name = "page")] int pageNumber = 0,
            [FromQuery(Name = "per_page")] int recordsPerPage = 25,
            [FromQuery(Name = "sort_field")] string sortField = "name",
            [FromQuery(Name = "sort_order")] string sortOrder = "asc")
        {
            var searchQuery = new SearchQueryParams(filter, sortField, sortOrder, pageNumber, recordsPerPage);
            var searchResult = await _service.Search(searchQuery, user, tenant);
            return OkResponse.FromSearchResult(searchResult);
        }

        /// <summary>
        /// Retrieve resource with given uid
        /// </summary>
        /// <remarks>Returns resource record or error if the resource is not found or is not accessible\n</remarks>
        /// <param name="resourceUid">uid identifying the resource</param>
        /// <response code="200">Resource with given uid was found</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet("{resourceUid}")]
        [SwaggerOperation("GetResourceByUID")]
        [ProducesResponseType(typeof(OkResponse<ResourceDto>), 200)]
        public async Task<OkResponse<ResourceDto>> GetResourceByUid([FromRoute] Guid resourceUid)
        {
            var resource = await _service.FindByUid(resourceUid);
            return OkResponse.WithData(resource);
        }

        /// <summary>
        /// Adds a new resource
        /// </summary>
        /// <remarks>Adds a new resource based on parameters. Returns uid for newly created resource.</remarks>
        /// <param name="newResourceDto">Resource object to be added to the database</param>
        /// <response code="200">New record was created successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost]
        [SwaggerOperation("CreateResource")]
        [ProducesResponseType(typeof(OkResponse<ResourceDto>), 200)]
        public async Task<OkResponse<ResourceDto>> CreateResource([FromBody] ResourceDto newResourceDto)
        {
            var resource = await _service.Create(newResourceDto);
            return OkResponse.WithData(resource);
        }

        /// <summary>
        /// Updates the resource identified by uid with new data
        /// </summary>
        /// <remarks>Updates the resource based on parameters. Returns true on success.</remarks>
        /// <param name="resourceUid">uid identifying the resource</param>
        /// <param name="updatedResourceDto">Updated Resource data</param>
        /// <response code="200">Record was updated successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPut("{uid}")]
        [SwaggerOperation("UpdateResource")]
        [ProducesResponseType(typeof(OkResponse<ResourceDto>), 200)]
        public async Task<OkResponse<ResourceDto>> UpdateResource(
            [FromRoute(Name = "uid")] Guid resourceUid,
            [FromBody] ResourceDto updatedResourceDto)
        {
            var resource = await _service.Update(resourceUid, updatedResourceDto);
            return OkResponse.WithData(resource);
        }

        /// <summary>
        /// Deletes resource by its uid
        /// </summary>
        /// <remarks>Returns true if successfull.\n</remarks>
        /// <param name="resourceUid">Uid identifying the resource</param>
        /// <response code="200">Record was deleted successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpDelete("{uid}")]
        [SwaggerOperation("DeleteResource")]
        [ProducesResponseType(typeof(OkResponse<Guid>), 200)]
        public async Task<IActionResult> DeleteResource([FromRoute(Name = "uid")] Guid resourceUid)
        {
            await _service.Delete(resourceUid);
            return Ok(OkResponse.WithData(resourceUid));
        }

        /// <summary>
        /// Mass deletes resources
        /// </summary>
        /// <remarks>Returns true if successfull.\n</remarks>
        /// <param name="resourceUids">resource uids</param>
        /// <response code="200">Record was deleted successfully</response>
        [HttpPost("delete")]
        [SwaggerOperation("MassDeleteResource")]
        [ProducesResponseType(typeof(OkResponse<Guid>), 200)]
        public async Task<IActionResult> MassDeleteResource([FromBody] Guid[] resourceUids)
        {
            await _service.MassDelete(resourceUids);
            return Ok();
        }

        /// <summary>
        /// Creates new shared access token (aka SAS) that allow user to upload a file.
        /// </summary>
        /// <remarks>Creates new shared access token that allow user to upload a file.\n</remarks>
        /// 
        /// <param name="srcFileName">Source filename of the resource</param>
        /// 
        /// <returns>The URL of the resource in storage with attached SAS </returns>
        /// 
        /// <response code="200">Token is generated successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost("upload_token/{file_name}")]
        [SwaggerOperation("CreateUploadToken")]
        [ProducesResponseType(typeof(OkResponse<string>), 200)]
        public IActionResult CreateUploadToken([FromRoute(Name = "file_name")] string srcFileName)
        {
            if (!UserUid.HasValue)
                return UnauthorizedWithCodeAndBody(401, ErrorResponse.GenerateInvalidUser("user_uid"));

            var fileName = Path.GetFileName(srcFileName);
            fileName = UserUid + "/" + Guid.NewGuid() + "-" + fileName;
            var storageAccount = CloudStorageAccount.Parse(_appSettings.StorageConnectionString);
            var container = storageAccount.CreateCloudBlobClient().GetContainerReference("resources");
            var blockBlob = container.GetBlockBlobReference(fileName);
            var url = blockBlob.Uri + blockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions =
                    SharedAccessBlobPermissions.Add | SharedAccessBlobPermissions.Read |
                    SharedAccessBlobPermissions.Write,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(1),
            });

            return Ok(OkResponse.WithData(url));
        }
    }
}