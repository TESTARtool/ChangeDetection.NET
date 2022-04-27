﻿namespace Testar.ChangeDetection.Core.Graph;

public record struct PropertyValue(string Value)
{
    public PropertyValue[] AsArray() => Value
        .Replace("[", "")
        .Replace("]", "")
        .Split(',')
        .Select(x => new PropertyValue(x))
        .ToArray();

    public bool AsBool() => bool.Parse(Value);
}