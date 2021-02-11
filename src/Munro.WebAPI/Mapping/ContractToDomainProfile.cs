using AutoMapper;
using EventManager.WebAPI.Model;

namespace EventManager.WebAPI.Mapping
{
    public class ContractToDomainProfile : Profile
    {
        public ContractToDomainProfile()
        {
            CreateMap<EventJobRequest, EventJob>()
                .ForMember(d => d.Id, m => m.Ignore())
                .ForMember(d => d.TimeStamp, m => m.Ignore())
                .ForMember(d => d.Duration, m => m.Ignore())
                .ForMember(d => d.IsCompleted, m => m.Ignore())
                .ForMember(d => d.IsFailed, m => m.Ignore());
        }
    }
}