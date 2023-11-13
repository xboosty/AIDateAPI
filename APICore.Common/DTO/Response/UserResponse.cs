using System;

namespace APICore.Common.DTO.Response
{
    public class UserResponse
    {
        public int Id { get; set; }
        public DateTime BirthDate { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
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
        public bool IsGenderVisible { get; set; }
        public bool IsSexualityVisible { get; set; }
        public List<string> Pictures { get; set; }
    }
}