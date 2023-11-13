using APICore.Data.Entities.Enums;

namespace APICore.Data.Entities
{
    public class User : BaseEntity
    {
        public User()
        {
            UserTokens = new HashSet<UserToken>();
            Blockeds = new HashSet<BlockedUsers>();
            Blockers = new HashSet<BlockedUsers>();
            Reporteds = new HashSet<ReportedUsers>();
            Reporters = new HashSet<ReportedUsers>();
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
        public string LastName { get; set; }
        public GenderEnum Gender { get; set; }
        public SexualOrientationEnum SexualOrientation { get; set; }
        public string Email { get; set; }
        public string PhoneCode { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public StatusEnum Status { get; set; }
        public FrequencyEnum IsSmoker { get; set; }
        public FrequencyEnum ExerciseFrequency { get; set; }
        public RelationshipEnum TypeRelationship { get; set; }
        public ReligionEnum Religions { get; set; }
        public KindRelationshipEnum KindRelationship { get; set; }
        public PositionBedEnum PositionBed { get; set; }
        public DietaryPreferenceEnum DietaryPreference { get; set; }
        public DateTimeOffset? LastLoggedIn { get; set; }
        public string? HabitsAndGoals { get; set; }
        public string? HistoryRelationship { get; set; }
        public string? Pet { get; set; }
        public string? Hobbies { get; set; }
public int Height { get; set; }
        public string? Avatar { get; set; }
        public string? AvatarMimeType { get; set; }
        public string? Pictures { get; set; }
        public bool HaveChildren { get; set; }
        public bool IsVaccinated { get; set; }
        public bool disease { get; set; }
        public bool IsGenderVisible { get; set; }
        public bool IsSexualityVisible { get; set; }
        public virtual ICollection<UserToken> UserTokens { get; set; }
        public ICollection<BlockedUsers> Blockeds { get; set; }
        public ICollection<BlockedUsers> Blockers { get; set; }
        public ICollection<ReportedUsers> Reporteds { get; set; }
        public ICollection<ReportedUsers> Reporters { get; set; }
    }
}