using APICore.Data.Entities;
using APICore.Data.Entities.Enums;
using Bogus;
using System;
using System.Collections.Generic;
using System.Text.Json;

public class FakeUserDataGenerator
{
    public static List<User> GenerateFakeUsers(int numberOfUsers)
    {
        var fakeUsers = new List<User>();
        var faker = new Faker<User>()
            .RuleFor(u => u.Identity, f => f.Random.Guid().ToString())
            .RuleFor(u => u.IsEmailVerified, f => f.Random.Bool())
            .RuleFor(u => u.IsPhoneVerified, f => f.Random.Bool())
            .RuleFor(u => u.IsGeneratedPassChanged, f => f.Random.Bool())
            .RuleFor(u => u.VerificationCode, f => f.Random.AlphaNumeric(6))
            .RuleFor(u => u.CreatedCode, f => f.Date.Past(1, DateTime.Now))
            .RuleFor(u => u.BirthDate, f => f.Date.Past(18))
            .RuleFor(u => u.FullName, f => f.Name.FullName())
            .RuleFor(u => u.Gender, f => f.PickRandom<GenderEnum>())
            .RuleFor(u => u.SexualOrientation, f => f.PickRandom<SexualOrientationEnum>())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PhoneCode, f => f.PickRandom("US", "CA", "UK"))
            .RuleFor(u => u.Phone, (f, u) => f.Phone.PhoneNumber(u.PhoneCode))
            .RuleFor(u => u.Password, f => f.Internet.Password())
            .RuleFor(u => u.Status, f => f.PickRandom<StatusEnum>())
            .RuleFor(u => u.LastLoggedIn, f => f.Date.Recent(7))
            .RuleFor(u => u.Avatar, f => f.Image.PicsumUrl())
            .RuleFor(u => u.AvatarMimeType, f => f.PickRandom("image/jpeg", "image/png"));

        for (var i = 0; i < numberOfUsers; i++)
        {
            var fakeUser = faker.Generate();
            fakeUsers.Add(fakeUser);
        }

        return fakeUsers;
    }

        public static void UpdateUserList(List<User> userList)
        {
            var userFaker = new Faker<User>()
                .RuleFor(u => u.IsSmoker, f => f.PickRandom<FrequencyEnum>())
                .RuleFor(u => u.ExerciseFrequency, f => f.PickRandom<FrequencyEnum>())
                .RuleFor(u => u.TypeRelationship, f => f.PickRandom<RelationshipEnum>())
                .RuleFor(u => u.Religions, f => f.PickRandom<ReligionEnum>())
                .RuleFor(u => u.KindRelationship, f => f.PickRandom<KindRelationshipEnum>())
                .RuleFor(u => u.PositionBed, f => f.PickRandom<PositionBedEnum>())
                .RuleFor(u => u.DietaryPreference, f => f.PickRandom<DietaryPreferenceEnum>())
                .RuleFor(u => u.HabitsAndGoals, f => f.Lorem.Sentence())
                .RuleFor(u => u.HistoryRelationship, f => f.Lorem.Sentence())
                .RuleFor(u => u.Pet, f => f.Lorem.Word())
                            .RuleFor(u => u.Hobbies, f => f.PickRandom<HobbyEnum>().ToString()+","+f.PickRandom<HobbyEnum>().ToString()+","+f.PickRandom<HobbyEnum>().ToString())
            .RuleFor(u => u.Height, f => f.Random.Number(150, 200))
            .RuleFor(u => u.HaveChildren, f => f.Random.Bool())
            .RuleFor(u => u.IsVaccinated, f => f.Random.Bool())
            .RuleFor(u => u.disease, f => f.Random.Bool());

            foreach (var user in userList)
            {
                userFaker.Populate(user);
            }
        }
    
}
