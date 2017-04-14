using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Microsoft.Extensions.DependencyInjection;

namespace Avend.ApiTests.ControllerTests.CrmConfiguration
{
    public class CrmConfigurationTestBase : IntegrationTest
    {
        private readonly IServiceScope _scope;
        protected readonly CrmSystem Dynamics;
        protected readonly CrmSystem Salesforce;

        public CrmConfigurationTestBase()
        {
            _scope = System.GetServices();
            var db = _scope.GetService<AvendDbContext>();
            Dynamics =
                (from c in db.CrmSystemsTable where c.Abbreviation == CrmSystemAbbreviation.Dynamics365 select c)
                .First();
            Salesforce =
                (from c in db.CrmSystemsTable where c.Abbreviation == CrmSystemAbbreviation.Salesforce select c)
                .First();
        }

        public override void Dispose()
        {
            base.Dispose();
            _scope?.Dispose();
        }

        protected UserCrmDto MakeConfigurationDto(CrmSystem crmSystem, Action<UserCrmDto> postProcessor = null)
        {
            var dto = new UserCrmDto
            {
                Type = crmSystem.Abbreviation,
                SyncFields = new Dictionary<string, bool>
                {
                    {"first_name", true}
                },
                Name = $"Test {crmSystem.Abbreviation}",
                Url =
                    crmSystem.Abbreviation == CrmSystemAbbreviation.Dynamics365
                        ? new Uri("https://some.crm.dynamics.com")
                        : null
            };
            postProcessor?.Invoke(dto);
            return dto;
        }

        protected async Task<UserCrmDto> Create(UserCrmDto dto)
        {
            return await BobTA.PostJsonAsync("crm", dto).AvendResponse<UserCrmDto>();
        }
    }
}