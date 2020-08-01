using System;
namespace TrackWorkout.Pages.SharedClasses
{
    public class Valid
    {
        public static bool IsOnlyNumbers(string valueToCheck)
        {
            foreach (char c in valueToCheck)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsValidDate(string valueToCheck)
        {
            DateTime dt;
            if (!DateTime.TryParseExact(valueToCheck, "MM/dd/yyyy",
                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt))
            {
                return false;
            }

            return true;
        }
    }
}
