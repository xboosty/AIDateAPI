using System;

namespace APICore.Common.DTO.Response
{
    public class UserWithMatchResponse
    {
        public int Id { get; set; }
        public DateTime BirthDate { get; set; }
        public string FullName { get; set; }
        public string Identity { get; set; }
        public int GenderId { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
        public string Avatar { get; set; }
        public string AvatarMimeType { get; set; }
        public string SexualOrientation { get; set; }
        public int SexualityId { get; set; }
        public List<string> Pictures { get; set; }
        public int Age { get; set; }
        public bool IsReported { get; set; }
        public string ReasonReported { get; set; }
        public bool IsBlocked { get; set; }
        public string ZodiacSymbol { get; set; }
        public int matchCompatibility { get; set; }
        public string HabitsAndGoals { get; set; }
        public string HistoryRelationship { get; set; }
        public string Pet { get; set; }
        public List<string> Hobbies { get; set; }
        public int Height { get; set; }

        public bool HaveChildren { get; set; }
        public bool IsVaccinated { get; set; }
        public bool disease { get; set; }

        public string  IsSmoker { get; set; }
        public string  ExerciseFrequency { get; set; }
        public string  TypeRelationship { get; set; }
        public string  Religions { get; set; }
        public string  KindRelationship { get; set; }
        public string  PositionBed { get; set; }
        public string  DietaryPreference { get; set; }

    }
}