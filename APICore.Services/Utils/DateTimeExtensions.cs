using System;
namespace APICore.Services.Utils
{

public static class DateTimeExtensions
{
    public static string GetZodiacSign(this DateTime date)
    {
        DateTime ariesStart = new DateTime(date.Year, 3, 21);
        DateTime taurusStart = new DateTime(date.Year, 4, 20);
        DateTime geminiStart = new DateTime(date.Year, 5, 21);
        DateTime cancerStart = new DateTime(date.Year, 6, 21);
        DateTime leoStart = new DateTime(date.Year, 7, 23);
        DateTime virgoStart = new DateTime(date.Year, 8, 23);
        DateTime libraStart = new DateTime(date.Year, 9, 23);
        DateTime scorpioStart = new DateTime(date.Year, 10, 23);
        DateTime sagittariusStart = new DateTime(date.Year, 11, 22);
        DateTime capricornStart = new DateTime(date.Year, 12, 22);
        DateTime aquariusStart = new DateTime(date.Year, 1, 20);
        DateTime piscesStart = new DateTime(date.Year, 2, 19);

        if ((date >= ariesStart && date <= new DateTime(date.Year, 4, 19)) ||
            (date >= capricornStart && date <= new DateTime(date.Year, 12, 31)))
        {
            return "Aries";
        }
        else if (date >= taurusStart && date <= new DateTime(date.Year, 5, 20))
        {
            return "Taurus";
        }
        else if (date >= geminiStart && date <= new DateTime(date.Year, 6, 20))
        {
            return "Gemini";
        }
        else if (date >= cancerStart && date <= new DateTime(date.Year, 7, 22))
        {
            return "Cancer";
        }
        else if (date >= leoStart && date <= new DateTime(date.Year, 8, 22))
        {
            return "Leo";
        }
        else if (date >= virgoStart && date <= new DateTime(date.Year, 9, 22))
        {
            return "Virgo";
        }
        else if (date >= libraStart && date <= new DateTime(date.Year, 10, 22))
        {
            return "Libra";
        }
        else if (date >= scorpioStart && date <= new DateTime(date.Year, 11, 21))
        {
            return "Scorpio";
        }
        else if (date >= sagittariusStart && date <= new DateTime(date.Year, 12, 21))
        {
            return "Sagittarius";
        }
        else if (date >= aquariusStart && date <= new DateTime(date.Year, 1, 19))
        {
            return "Aquarius";
        }
        else // From January 20 to February 18
        {
            return "Pisces";
        }
    }
}
}