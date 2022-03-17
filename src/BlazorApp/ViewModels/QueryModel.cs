using System.Text.RegularExpressions;

namespace BlazorApp.ViewModels;

public class ParameterModel
{
    public string Name { get; set; }
    public string Value { get; set; }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return Name.Equals(obj is ParameterModel parm && parm.Name.Equals(Name));
    }
}

public class QueryModel
{
    private string query;
    public HashSet<ParameterModel> Parameters { get; set; } = new();

    public string Query
    {
        get { return query; }
        set
        {
            var matches = Regex.Matches(value, @":([\w]*)", RegexOptions.Compiled);

            foreach (var match in matches)
            {
                var key = match?.ToString()?.Substring(1);
                if (!string.IsNullOrWhiteSpace(key) && !Parameters.Any(x => x.Name == key))
                {
                    Parameters.Add(new ParameterModel
                    {
                        Name = key,
                        Value = string.Empty
                    });
                }
            }

            foreach (var parameter in Parameters)
            {
                if (!matches.Any(x => x?.ToString() == parameter.Name))
                {
                    Parameters.Remove(parameter);
                }
            }

            query = value;
        }
    }
}