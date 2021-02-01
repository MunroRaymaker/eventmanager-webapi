using AutoMapper;
using EventManager.WebAPI.Model;

namespace EventManager.WebAPI.Mapping
{
    public class ContractToDomainProfile : Profile
    {
        public ContractToDomainProfile()
        {
            CreateMap<EventJobRequest, EventJob>();
        }
    }
}