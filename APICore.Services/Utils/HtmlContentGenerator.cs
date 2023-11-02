
using APICore.Data.Entities;
using APICore.Data.Migrations;
using System.Text;
using System.Text.RegularExpressions;

namespace APICore.Services.Utils
{
    public static class HtmlContentGenerator
    {
        public static string GenerateReportHtml(List<APICore.Data.Entities.ReportedUsers> reportedUsersList)
        {
            APICore.Data.Entities.ReportedUsers reported = reportedUsersList.FirstOrDefault();
            StringBuilder html = new StringBuilder();

            html.Append("<html>");

            html.Append("<head><title>Reported Users</title></head>");

            html.Append("<body>");

            html.Append("<h1>Reported Users</h1>");
            if (reported != null)
                html.Append($"<h2>Reported User: {reported.ReportedUser.FullName}</h2>");

            html.Append("<table border='1'>");

            foreach (var reportedUser in reportedUsersList)
            {

                html.Append("<tr><th>Reporter User</th><th>Comment</th></tr>");

                html.Append("<tr>");
                html.Append($"<td>{reportedUser.ReporterUser.FullName}</td>");
                html.Append($"<td>{reportedUser.Coment}</td>");
                html.Append("</tr>");

            }
            html.Append("</table>");

            html.Append("</body>");
            html.Append("</html>");

            return html.ToString();
        }
        public static string GenerateRowToReport(List<APICore.Data.Entities.ReportedUsers> reportedUsersList)
        {
            StringBuilder html = new StringBuilder();

            foreach (var reportedUser in reportedUsersList)
            {
                html.Append("<tr>");
                html.Append($"<td>{reportedUser.ReporterUser.FullName}</td>");
                html.Append($"<td>{reportedUser.Coment}</td>");
                html.Append("</tr>");
            }

            return html.ToString();
        }

        public static string FillTemplateFromFile(string templatePath, Dictionary<string, string> tokens)
        {
            string template = File.ReadAllText(templatePath);
            return template.ReplaceTemplateTokens(tokens);
        }

        public static string ReplaceTemplateTokens(this string template, Dictionary<string, string> tokenReplacements)
        {
            foreach (var kvp in tokenReplacements)
            {
                template = template.Replace(kvp.Key, kvp.Value);
            }

            return template;
        }

    }
}
