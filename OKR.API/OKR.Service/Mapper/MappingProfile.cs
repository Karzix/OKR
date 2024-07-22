﻿using AutoMapper;
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
		}
	}
}