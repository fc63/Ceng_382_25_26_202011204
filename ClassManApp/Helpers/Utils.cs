/*
week7 prompt:
"Write a C# utility class named Utils under the ClassManApp.Helpers namespace.

This class must:

    Be implemented as a singleton, with a private constructor and a static Instance property.

    Contain a public generic method ToJson<T>(...) that takes:

        IEnumerable<T> data as the main input list

        IEnumerable<string>? selectedColumns as an optional list of selected property names

Behavior of ToJson<T>:

    If selectedColumns is null or empty, serialize the entire object as-is using System.Text.Json.

    If selectedColumns is provided, serialize only those properties that are present in the list. Use reflection to filter and build a dictionary for each item accordingly.

    The final output must be a JSON-formatted string with indented formatting enabled (WriteIndented = true).

Use only built-in .NET libraries, no external packages.
Return the complete class code including using directives."
*/

using System.Text.Json;

namespace ClassManApp.Helpers
{
    public sealed class Utils
    {
        private static readonly Utils _instance = new Utils();
        public static Utils Instance => _instance;

        private Utils() { }

        public string ToJson<T>(IEnumerable<T> data, IEnumerable<string>? selectedColumns = null)
        {
            if (selectedColumns == null || !selectedColumns.Any())
                return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            var filtered = data.Select(item =>
            {
                var obj = new Dictionary<string, object?>();
                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    if (selectedColumns.Contains(prop.Name))
                    {
                        obj[prop.Name] = prop.GetValue(item);
                    }
                }
                return obj;
            });

            return JsonSerializer.Serialize(filtered, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}