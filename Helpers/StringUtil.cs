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
            return Regex.Replace(
                    input.Trim().ToLower(),
                    @"\s+-\s+|,\s+|\s+|\ufeff|\(|\)|\.",
                    "_"
                ).ToLower();
        }
    }
}