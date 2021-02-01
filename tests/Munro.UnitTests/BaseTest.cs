using AutoMapper;
using EventManager.WebAPI.Mapping;

namespace Munro.UnitTests
{
    public class BaseTest
    {
        protected IConfigurationProvider Configuration;
        protected IMapper Mapper;


        public BaseTest()
        {
            this.Configuration = new MapperConfiguration(cfg => { cfg.AddProfile<ContractToDomainProfile>(); });

            this.Mapper = this.Configuration.CreateMapper();
        }
    }
}