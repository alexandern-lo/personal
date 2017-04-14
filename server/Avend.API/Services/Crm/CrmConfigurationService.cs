using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Helpers;
using Avend.OAuth;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Qoden.Validation;
using Error = Qoden.Validation.Error;

namespace Avend.API.Services.Crm
{
    public class CrmConfigurationService
    {
        private readonly DbContextOptions<AvendDbContext> _dbOptions;
        private readonly UserContext _userContext;
        private readonly UserCrmDtoBuilder _builder;

        public CrmConfigurationService(
            DbContextOptions<AvendDbContext> dbOptions,
            UserContext userContext,
            UserCrmDtoBuilder builder)
        {
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();
            Assert.Argument(userContext, nameof(userContext)).NotNull();
            Assert.Argument(builder, nameof(builder)).NotNull();

            _dbOptions = dbOptions;
            _userContext = userContext;
            _builder = builder;
        }

        public async Task<SearchResult<UserCrmDto>> Search(SearchQueryParams sorting)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var repo = new CrmConfigurationRepository(db)
                {
                    Scope = x => x.UserUid == _userContext.UserUid
                };
                var searchResult = repo.Search(sorting);
                return await searchResult.PaginateAsync(config => _builder.Build(config));
            }
        }

        public async Task<UserCrmDto> FindByUid(Guid uid)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var repo = new CrmConfigurationRepository(db)
                {
                    Scope = x => x.UserUid == _userContext.UserUid
                };
                var config = await Find(uid, repo);
                return _builder.Build(config);
            }
        }


        public async Task<UserCrmDto> Create(UserCrmDto crmDto)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var repo = new CrmConfigurationRepository(db);
                var config = repo.NewCrm(_userContext.UserUid);
                await UpdateRecord(crmDto, config, db);
                await db.SaveChangesAsync();
                return _builder.Build(config);
            }
        }

        public async Task<UserCrmDto> Update(Guid crmConfigUid, UserCrmDto crmDto)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var repo = new CrmConfigurationRepository(db)
                {
                    Scope = x => x.UserUid == _userContext.UserUid
                };
                var config = await Find(crmConfigUid, repo);
                await UpdateRecord(crmDto, config, db);
                await db.SaveChangesAsync();
                return _builder.Build(config);
            }
        }

        public async Task<UserCrmDto> UpdateGrantCode(Guid uid, UserCrmTokenDto userCrmTokenDto)
        {
            Check.Value(userCrmTokenDto, "body").NotNull();

            using (var db = new AvendDbContext(_dbOptions))
            {
                var repo = new CrmConfigurationRepository(db)
                {
                    Scope = x => x.UserUid == _userContext.UserUid
                };
                var config = await Find(uid, repo);
                var crmConnector = _builder.ConnectorFactory.GetConnectorForCrmSystem(config.CrmType);
                try
                {
                    var tokens = await crmConnector.GetTokensByGrantCode(config, userCrmTokenDto.Token);
                    config.AccessToken = tokens[OAuthApi.AccessToken] as string;
                    config.RefreshToken = tokens[OAuthApi.RefreshToken] as string;
                    await db.SaveChangesAsync();
                    return _builder.Build(config);
                }
                catch (OAuthException e)
                {
                    throw new ErrorException(new Error(e.Message));
                }
            }
        }

        public async Task<UserCrmDto> Delete(Guid uid)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var repo = new CrmConfigurationRepository(db)
                {
                    Scope = x => x.UserUid == _userContext.UserUid
                };
                var config = await Find(uid, repo);
                var settings = await db.Settings.FirstOrDefaultAsync(x => x.UserUid == _userContext.UserUid);
                if (settings.DefaultCrmId == config.Id)
                {
                    settings.DefaultCrmId = null;
                }
                repo.Delete(config);
                await db.SaveChangesAsync();
                return _builder.Build(config);
            }
        }

        private static async Task<CrmRecord> Find(Guid crmConfigUid, CrmConfigurationRepository repo)
        {
            var config = await repo.FindByUid(crmConfigUid);
            Check.Value(config, onError: AvendErrors.NotFound).NotNull("Crm configuration not found");
            return config;
        }

        private async Task UpdateRecord(UserCrmDto crmDto, CrmRecord config, AvendDbContext db)
        {
            Check.Value(crmDto, "body").NotNull();

            var validator = new Validator();

            if (crmDto.Name != null)
                config.Name = crmDto.Name;
            validator.CheckColumn(config, x => x.Name).IsShortText();

            if (crmDto.Type != null)
                config.CrmType = crmDto.Type.Value;

            if (crmDto.Url != null)
                config.Url = crmDto.Url?.ToString();

            if (config.CrmType == CrmSystemAbbreviation.Dynamics365)
            {
                var urlCheck = validator.CheckColumn(config, x => x.Url)
                    .NotNull().ConvertTo(x => new Uri(x))
                    .IsAbsoluteUri();
                if (urlCheck.IsValid)
                {
                    if (!("http".Equals(urlCheck.Value.Scheme) || "https".Equals(urlCheck.Value.Scheme)))
                    {
                        var error = new Error("Dynamics URL should be valid 'http' or 'https' URL")
                            .Validator("uri_scheme");
                        urlCheck.Fail(error);
                    }
                }
            }

            validator.Throw();

            //TODO create settings when user inserted into db
            var settings = await db.Settings.FirstOrDefaultAsync(x => x.UserUid == _userContext.UserUid);
            if (settings == null)
            {
                settings = new SettingsRecord
                {
                    UserUid = _userContext.UserUid,
                };
                db.Settings.Add(settings);
            }
            if (crmDto.Default != null)
            {
                if (crmDto.Default.Value)
                {
                    settings.DefaultCrm = config;
                }
                else if (settings.DefaultCrmId == config.Id)
                {
                    settings.DefaultCrm = null;
                }
            }

            crmDto.SyncFields = crmDto.SyncFields ?? new Dictionary<string, bool>();
            var avend2CrmMapping = CrmDefaultsHelper.DefaultCrmMappings[config.CrmType];
            var syncFields = new Dictionary<string, bool>();
            foreach (var kv in avend2CrmMapping)
            {
                if (crmDto.SyncFields.ContainsKey(kv.Key))
                {
                    syncFields.Add(kv.Key, crmDto.SyncFields[kv.Key]);
                }
                else
                {
                    syncFields.Add(kv.Key, false);
                }
            }
            config.SyncFields = JsonConvert.SerializeObject(syncFields);
        }
    }
}