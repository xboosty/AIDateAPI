using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace APICore.Common.Helpers
{
    public static class Extensions
    {
        public static bool MatchWithPasswordPolicy(this string word)
        {
            return ((word.Length > 8) &&
                    (new Regex("[a-z]{1}").IsMatch(word)) &&
                    (new Regex("[A-Z]{1}").IsMatch(word)) &&
                    !(new Regex("^[a-zA-Z0-9 ]*$").IsMatch(word)));
        }
    }
}