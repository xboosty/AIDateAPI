using APICore.Data.Entities.Enums;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APICore.Data.Entities
{
    public class ReportedUsersFaker
    {
        public static List<ReportedUsers> GenerateReportedUsersList(List<User> users, int reportCount)
        {
            var faker = new Faker<ReportedUsers>()
                .RuleFor(r => r.ReportDateTime, f => f.Date.Past())
                .RuleFor(r => r.ReporterUserId, f => f.PickRandom(users).Id)
                .RuleFor(r => r.ReportedUserId, (f, r) => f.PickRandom(users.Where(u => u.Id != r.ReporterUserId).Select(u => u.Id)))
                .RuleFor(r => r.Coment, f => f.Lorem.Sentence())
                .RuleFor(r => r.ReporStatus, f => f.PickRandom<ReportStatusEnum>());

            var reportedUsersList = faker.Generate(reportCount);
            return reportedUsersList;
        }

        public static List<ReportedUsers> GenerateReportsByEmail(List<User> users, string reporterEmail, int reportCount)
        {
            var reporterUser = users.FirstOrDefault(u => u.Email == reporterEmail);

            
            var userIds = users.Select(u => u.Id).ToList();
            var faker = new Faker<ReportedUsers>()
                .RuleFor(r => r.ReportDateTime, f => f.Date.Past())
                .RuleFor(r => r.ReporterUserId, reporterUser.Id)
                .RuleFor(r => r.ReportedUserId, (f, r) => f.PickRandom(userIds.Where(id => id != reporterUser.Id)))
                .RuleFor(r => r.Coment, f => f.Lorem.Sentence())
                .RuleFor(r => r.ReporStatus, f => f.PickRandom<ReportStatusEnum>());

            var reportedUsersList = faker.Generate(reportCount);
            return reportedUsersList;
        }
    }
}
