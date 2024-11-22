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
			CreateMap<DepartmentRespone, Department>().ReverseMap();
			CreateMap<DepartmentRequest, Department>().ReverseMap();
			CreateMap<KeyResultRespone, KeyResults>().ReverseMap();
			CreateMap<KeyResultRequest, KeyResults>().ReverseMap();
			CreateMap<ObjectivesRespone, Objectives>().ReverseMap();
			CreateMap<ObjectivesRequest, Objectives>().ReverseMap();
			CreateMap<ProgressUpdatesRespone, ProgressUpdates>().ReverseMap();
			CreateMap<UserRespone, ApplicationUser>().ReverseMap();
			CreateMap<UserRequest, ApplicationUser>().ReverseMap();
			CreateMap<EvaluateTargetRespone, EvaluateTarget>().ReverseMap();
			CreateMap<DepartmentProgressApprovalRespone, DepartmentProgressApproval>().ReverseMap();
			CreateMap<EvaluateTargetRequest, EvaluateTarget>().ReverseMap();

		}
	}
}
