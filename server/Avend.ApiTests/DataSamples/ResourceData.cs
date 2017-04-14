using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Avend.ApiTests.Infrastructure;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;

using Qoden.Validation;

namespace Avend.ApiTests.DataSamples
{
    public class ResourceData
    {
        public TestSystem System { get; }
        public TestUser User { get; }

        public ResourceDto Resource { get; private set; }
        public List<ResourceDto> Resources { get; private set; }

        public ResourceData(TestUser user, TestSystem system)
        {
            Assert.Argument(user, nameof(user)).NotNull();
            Assert.Argument(system, nameof(system)).NotNull();

            User = user;
            System = system;

            Resources = new List<ResourceDto>();
        }

        public async Task<ResourceDto> AddFromDto(ResourceDto dto)
        {
            using (var http = System.CreateClient(User.Token))
            {
                dto = await http.PostJsonAsync($"users/resources", dto).AvendResponse<ResourceDto>();
                Resources.Add(dto);
            }

            return dto;
        }

        public async Task<ResourceDto> Add(Action<ResourceDto> postProcessor = null)
        {
            var resourceDto = MakeSample();
            postProcessor?.Invoke(resourceDto);
            return await AddFromDto(resourceDto);
        }

        #region Static methods and field

        private static int nameIndex = 100;
        private static int imageIndex = 0;
        private static int descriptionIndex = 0;

        public static async Task<ResourceData> InitWithSample(TestUser user, TestSystem system)
        {
            using (var http = system.CreateClient(user.Token))
            {
                var dto = MakeSample();

                var newResourceDto = await http.PostJsonAsync("users/resources", dto).AvendResponse<ResourceDto>();

                return new ResourceData(user, system)
                {
                    Resource = newResourceDto
                };
            }
        }

        public static ResourceDto MakeSample()
        {
            nameIndex--;
            imageIndex++;
            descriptionIndex = -(descriptionIndex + 1);

            return new ResourceDto()
            {
                Name = $"Test Resource Name [{nameIndex}]",
                Description = "Test Description " + string.Format("{0,0:D4}", descriptionIndex),

                Url = "http://avend.com/resources/testimg_" + string.Format("{0,0:D4}", imageIndex) + ".jpg",
                MimeType = "image/jpeg",
            };
        }

        #endregion
    }
}