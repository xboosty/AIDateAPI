using APICore.Data;
using APICore.Data.Entities;
using APICore.Data.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using rlcx.suid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace APICore.API.Utils
{
    public class DemoDataGenerator
    {
        public static void CleanDatabase(CoreDbContext dBContext)
        {
            List<string> tableNames = dBContext.Model.GetEntityTypes().Select(t => t.GetTableName()).Distinct().ToList();
            foreach (string table in tableNames)
            {
                Console.WriteLine(table);
                dBContext.Database.ExecuteSqlRaw($"delete from {table}");
            }
        }

        public static void SeedData(CoreDbContext dBContext)
        {
            var userList = FakeUserDataGenerator.GenerateFakeUsers(100);
            if (dBContext.Users.Count() == 0)
            {
               dBContext.Users.AddRange(userList);
               dBContext.SaveChanges();
            }

if (dBContext.BlockedUsers.Count() == 0)
            {
                var usersList = dBContext.Users.ToList();
                var blockedUsersList = FakeBlockedUsersDataGenerator.GenerateBlockedUsersForUser("jeny20050@gmail.com", 10, usersList);
                dBContext.AddRange(blockedUsersList);
                dBContext.SaveChanges();
            }

if (dBContext.ReportedUsers.Count() == 0)
            {
                var users = dBContext.Users.ToList();
                var reportedUsers = ReportedUsersFaker.GenerateReportsByEmail(users, "jeny20050@gmail.com", 10);
                dBContext.AddRange(reportedUsers);
                dBContext.SaveChanges();
            }
}
}
}