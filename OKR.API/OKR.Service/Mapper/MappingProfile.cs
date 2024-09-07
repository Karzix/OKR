using AutoMapper;
using OKR.DTO;
using OKR.Models.Entity;


namespace OKR.Service.Mapper
{
    public class MappingProfile :Profile
	{
		public MappingProfile()
		{
			CreateMap();
		}

		public void CreateMap()
		{
			CreateMap<DepartmentDto, Department>().ReverseMap();
			CreateMap<KeyResultDto, KeyResults>().ReverseMap();
			CreateMap<ObjectiveDto, Objectives>().ReverseMap();
			CreateMap<SidequestsDto, Sidequests>().ReverseMap();
			CreateMap<ProgressUpdatesDto, ProgressUpdates>().ReverseMap();
			CreateMap<UserDto, ApplicationUser>().ReverseMap();
			CreateMap<EvaluateTargetDto, EvaluateTarget>().ReverseMap();
		}
	}
}
