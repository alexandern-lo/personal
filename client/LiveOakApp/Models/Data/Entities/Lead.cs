using System.Diagnostics;
using SL4N;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Data.Records;
using LiveOakApp.Models.Services;

namespace LiveOakApp.Models.Data.Entities
{
    public class Lead
    {
        static ILogger LOG = LoggerFactory.GetLogger<LeadsService>();

        JsonService JsonService { get; }

        public LeadRecord LeadRecord { get; }

        LeadDTO leadDTO;
        public LeadDTO LeadDTO
        {
            get
            {
                if (leadDTO == null)
                {
                    var jsonWatch = Stopwatch.StartNew();
                    leadDTO = JsonService.Deserialize<LeadDTO>(LeadRecord.LeadJson);
                    jsonWatch.Stop();
                    LOG.Trace("deserialized leadJson in {0} ms", jsonWatch.ElapsedMilliseconds);
                }
                return leadDTO;
            }
        }

        public FileResource PhotoResource
        {
            get
            {
                if (LeadRecord != null)
                {
                    return new FileResource(LeadRecord.LocalPhotoPath, LeadRecord.RemotePhotoUrl);
                }
                return new FileResource(null, LeadDTO.PhotoUrl);
            }
        }

        public FileResource CardFrontResource
        {
            get
            {
                if (LeadRecord != null)
                {
                    return new FileResource(LeadRecord.LocalCardFrontPath, LeadRecord.RemoteCardFrontUrl);
                }
                return new FileResource(null, LeadDTO.BusinessCardFrontUrl);
            }
        }

        public FileResource CardBackResource
        {
            get
            {
                if (LeadRecord != null)
                {
                    return new FileResource(LeadRecord.LocalCardBackPath, LeadRecord.RemoteCardBackUrl);
                }
                return new FileResource(null, LeadDTO.BusinessCardBackUrl);
            }
        }

        public Lead(LeadDTO dto) : this(null, dto)
        {
        }

        public Lead(LeadRecord record) : this(record, null)
        {
        }

        public Lead(LeadRecord record, LeadDTO dto)
        {
            JsonService = ServiceLocator.Instance.JsonService;
            LeadRecord = record;
            leadDTO = dto;
        }
    }
}
