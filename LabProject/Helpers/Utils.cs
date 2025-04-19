/*
week 7 prompt:
"Create a C# helper class called Utils inside the namespace ClassManApp.Helpers. This class should:

    Be a singleton with a private constructor and a public static Instance property.

    Contain a generic method ToJson<T> that:

        Accepts an IEnumerable<T> data and an optional IEnumerable<string> selectedColumns.

        If selectedColumns is null or empty, serialize the entire data object to JSON with indented formatting.

        If selectedColumns is provided, iterate over each item in data:

            Use reflection to get properties of T.

            For each item, only include properties whose name matches an entry in selectedColumns.

            Store selected values in a dictionary and serialize the list of dictionaries.

Use System.Text.Json and ensure the output JSON is pretty-printed.
Return only the full Utils.cs file."
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