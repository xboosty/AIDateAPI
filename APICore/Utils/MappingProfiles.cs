﻿using APICore.Common.DTO.Response;
using APICore.Data.Entities;
using AutoMapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System.Net;

namespace APICore.Utils
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserResponse>()
                .ForMember(d => d.StatusId, opts => opts.MapFrom(source => (int)source.Status))
                .ForMember(d => d.Status, opts => opts.MapFrom(source => source.Status.ToString()))
                .ForMember(d => d.GenderId, opts => opts.MapFrom(source => (source.IsGenderVisible) ? (int)source.Gender : -1))
                .ForMember(d => d.Gender, opts => opts.MapFrom(source => (source.IsGenderVisible) ? source.Gender.ToString() : ""))
                .ForMember(d => d.SexualOrientation, opts => opts.MapFrom(source => (source.IsSexualityVisible) ? source.SexualOrientation.ToString() : ""))
                .ForMember(d => d.Pictures, opts => opts.MapFrom(source =>(string.IsNullOrEmpty(source.Pictures))?new List<string>() :JsonConvert.DeserializeObject<List<string>>(source.Pictures)));

            CreateMap<HealthReportEntry, HealthCheckResponse>()
                .ForMember(d => d.Description, opts => opts.MapFrom(source => source.Description))
                .ForMember(d => d.Duration, opts => opts.MapFrom(source => source.Duration.TotalSeconds))
                .ForMember(d => d.ServiceStatus, opts => opts.MapFrom(source => source.Status == HealthStatus.Healthy ?
                                                                                HttpStatusCode.OK :
                                                                                (source.Status == HealthStatus.Degraded ? HttpStatusCode.OK : HttpStatusCode.ServiceUnavailable)))
                .ForMember(d => d.Exception, opts => opts.MapFrom(source => source.Exception == null ? "" : source.Exception.Message));

            CreateMap<Setting, SettingResponse>();
            CreateMap<Log, LogResponse>()
               .ForMember(d => d.LogType, opts => opts.MapFrom(source => source.LogType.ToString()))
               .ForMember(d => d.EventType, opts => opts.MapFrom(source => source.EventType.ToString()));

            CreateMap<ReportedUsers, ReportedUserResponse>();

        }
    }
}