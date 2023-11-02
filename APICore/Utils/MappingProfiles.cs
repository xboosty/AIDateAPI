using APICore.Common.DTO.Response;
using APICore.Data.Entities;
using APICore.Services.Utils;
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
                .ForMember(d => d.Pictures, opts => opts.MapFrom(source => (string.IsNullOrEmpty(source.Pictures)) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(source.Pictures)))
                .ForMember(d => d.SexualityId, opts => opts.MapFrom(s => (s.IsSexualityVisible)? (int)s.SexualOrientation : -1))
                .AfterMap<UserPictureAction>();

            CreateMap<User, UserWithMatchResponse>()
                .ForMember(d => d.StatusId, opts => opts.MapFrom(source => (int)source.Status))
                .ForMember(d => d.Status, opts => opts.MapFrom(source => source.Status.ToString()))
                .ForMember(d => d.GenderId, opts => opts.MapFrom(source => (source.IsGenderVisible) ? (int)source.Gender : -1))
                .ForMember(d => d.Gender, opts => opts.MapFrom(source => (source.IsGenderVisible) ? source.Gender.ToString() : ""))
                .ForMember(d => d.SexualOrientation, opts => opts.MapFrom(source => (source.IsSexualityVisible) ? source.SexualOrientation.ToString() : ""))
                .ForMember(d => d.Pictures, opts => opts.MapFrom(source => (string.IsNullOrEmpty(source.Pictures)) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(source.Pictures)))
                .ForMember(d => d.SexualityId, opts => opts.MapFrom(s => (s.IsSexualityVisible) ? (int)s.SexualOrientation : -1))
                .ForMember(d => d.Age, opt => opt.MapFrom(s => (s.BirthDate != null) ? DateTime.Now.Year-s.BirthDate.Year : 0))
                .ForMember(d => d.ZodiacSymbol, opts => opts.MapFrom(s => (s.BirthDate != null) ? s.BirthDate.GetZodiacSign() : "unknown"))
                .ForMember(d => d.IsSmoker, opts => opts.MapFrom(s => s.IsSmoker.ToString()))
                .ForMember(d => d.ExerciseFrequency, opts => opts.MapFrom(s => s.ExerciseFrequency.ToString()))
                .ForMember(d => d.DietaryPreference, opts => opts.MapFrom(s => s.DietaryPreference.ToString()))
                .ForMember(d => d.Religions, opts => opts.MapFrom(s => s.Religions.ToString()))
                .ForMember(d => d.TypeRelationship, opts => opts.MapFrom(s => s.TypeRelationship.ToString()))
                .ForMember(d => d.KindRelationship, opts => opts.MapFrom(s => s.KindRelationship.ToString()))
                .ForMember(d => d.PositionBed, opts => opts.MapFrom(s => s.PositionBed.ToString()))
.ForMember(d => d.Hobbies, opts => opts.MapFrom(s => (s.Hobbies != null) ? JsonConvert.DeserializeObject<List<string>>(s.Hobbies) :new List<string>()))
.ForMember(d => d.HistoryRelationship, opts => opts.MapFrom(s => (!string.IsNullOrEmpty(s.HistoryRelationship)) ? s.HistoryRelationship : ""))
.ForMember(d => d.HabitsAndGoals, opts => opts.MapFrom(s => (!string.IsNullOrEmpty(s.HabitsAndGoals)) ? s.HabitsAndGoals : ""))
.ForMember(d => d.Pet, opts => opts.MapFrom(s => (!string.IsNullOrEmpty(s.Pet)) ? s.Pet : ""));


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