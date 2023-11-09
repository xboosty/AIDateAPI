using System;

namespace APICore.Common.DTO.Response
{
    public class EditProfileRequest
    {
        public DateTime BirthDate { get; set; }
        public string FullName { get; set; }
        public int GenderId { get; set; }
        public string Gender { get; set; }
        public int SexualOrientationId { get; set; }
        public string SexualOrientation { get; set; }
        public bool IsGenderVisible { get; set; }
        public bool IsSexualityVisible { get; set; }
    }
}