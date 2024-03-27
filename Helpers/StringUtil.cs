using System.Text.RegularExpressions;


namespace BackendJPMAnalysis.Helpers
{
    public static class StringUtil
    {
        /// <summary>
        /// The SnakeCase function converts a given input string to snake_case format by replacing
        /// spaces, dashes, commas, and other characters with underscores.
        /// </summary>
        /// <param name="input">
        /// The `SnakeCase` method takes a string input and converts it to
        /// snake_case format by replacing spaces, hyphens, commas, periods, parentheses, and other
        /// special characters with underscores. It also trims leading and trailing spaces and converts
        /// the string to lowercase.
        /// </param>
        /// <returns>
        /// The SnakeCase version of the input string is being returned.
        /// </returns>
        public static string SnakeCase(string input)
        {
            // return Regex.Replace(
            //         input.Trim().ToLower(),
            //         @"\s+-\s+|,\s+|\s+|\ufeff|\(|\)|\.",
            //         "_"
            //     ).ToLower().Replace("(", string.Empty).Replace(")", string.Empty);
            return Regex.Replace(input, @"[^a-zA-Z0-9]+", "_").ToLower().Trim('_');
        }


        /// <summary>
        /// The function `RemoveUnnecessarySpaces` removes extra spaces and commas from a given input
        /// string.
        /// </summary>
        /// <param name="input">The `RemoveUnnecessarySpaces` method takes a string input and removes
        /// unnecessary spaces by replacing double spaces with single spaces and removing
        /// commas.</param>
        /// <returns>
        /// The method `RemoveUnnecessarySpaces` takes a string input and removes any double spaces by
        /// replacing them with single spaces, as well as removing any commas. The modified string is
        /// then returned.
        /// </returns>
        public static string RemoveUnnecessarySpaces(string input)
        {
            return input.Replace("  ", " ").Replace(",", "");
        }


        /// <summary>
        /// The function `ParseDateTime` attempts to parse a string input into a DateTime object using a
        /// specific date and time format, returning the parsed DateTime or throwing an exception if the
        /// input is not in the expected format.
        /// </summary>
        /// <param name="input">The `ParseDateTime` method takes a nullable string `input` as a
        /// parameter. This method attempts to parse the input string into a `DateTime` object using the
        /// specified date and time format ("MM/dd/yyyy hh:mm:ss tt"). If the parsing is successful, it
        /// returns the parsed `DateTime`</param>
        /// <returns>
        /// The method `ParseDateTime` returns a nullable `DateTime` value.
        /// </returns>
        public static DateTime? ParseDateTime(string? input)
        {
            if (string.IsNullOrEmpty(input)) return null;

            if (DateTime.TryParseExact(
                    input, "MM/dd/yyyy hh:mm:ss tt",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDate))
            {
                return parsedDate;
            }
            else
            {
                throw new ArgumentException($"La fecha original '{input}', no está en el formato esperado.");
            }
        }
    }
}