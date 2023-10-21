using APICore.Data.Entities.Enums;

namespace APICore.Data.Entities
{
    public class User : BaseEntity
    {
        public User()
        {
            UserTokens = new HashSet<UserToken>();
        }

        public int Id { get; set; }
        public string Identity { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
        public bool IsGeneratedPassChanged { get; set; }
        public string VerificationCode { get; set; }
        public DateTime CreatedCode { get; set; }
        public DateTime BirthDate { get; set; }
        public string FullName { get; set; }
        public GenderEnum Gender { get; set; }
        public SexualOrientationEnum SexualOrientation { get; set; }
        public string Email { get; set; }
        public string PhoneCode { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public StatusEnum Status { get; set; }
        public DateTimeOffset? LastLoggedIn { get; set; }
        public string? Avatar { get; set; }
        public string? AvatarMimeType { get; set; }
        public virtual ICollection<UserToken> UserTokens { get; set; }
        public ICollection<BlockedUsers> Blockeds { get; set; }
        public ICollection<BlockedUsers> Blockers { get; set; }
    }
}