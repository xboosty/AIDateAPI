using APICore.Data.Entities;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;

public class FakeBlockedUsersDataGenerator
{
    public static List<BlockedUsers> GenerateFakeBlockedUsers(int numberOfBlockedUsers, List<User> userList)
    {
        var fakeBlockedUsers = new List<BlockedUsers>();
        var faker = new Faker<BlockedUsers>()
            .RuleFor(bu => bu.BlockDateTime, f => f.Date.Past(1, DateTime.Now))
            .RuleFor(bu => bu.BlockerUserId, f => f.PickRandom(userList).Id)
            .RuleFor(bu => bu.BlockedUserId, f => f.PickRandom(userList).Id);

        for (var i = 0; i < numberOfBlockedUsers; i++)
        {
            var fakeBlockedUser = faker.Generate();
            fakeBlockedUsers.Add(fakeBlockedUser);
        }

        return fakeBlockedUsers;
    }

    public static List<BlockedUsers> GenerateBlockedUsersForUser(string userEmail, int numberOfBlockedUsers, List<User> userList)
    {
        var fakeBlockedUsers = new List<BlockedUsers>();
        var userBlocker = userList.FirstOrDefault(u => u.Email.Equals(userEmail) );

        if (userBlocker == null)
        {
            throw new Exception("User not found for the specified email.");
        }

        var faker = new Faker<BlockedUsers>()
            .RuleFor(bu => bu.BlockDateTime, f => f.Date.Past(1, DateTime.Now))
            .RuleFor(bu => bu.BlockerUserId, userBlocker.Id)
            .RuleFor(bu => bu.BlockedUserId, f => f.PickRandom(userList).Id); 

        for (var i = 0; i < numberOfBlockedUsers; i++)
        {
            var fakeBlockedUser = faker.Generate();
            fakeBlockedUsers.Add(fakeBlockedUser);
        }

        return fakeBlockedUsers;
    }
}
