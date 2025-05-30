namespace Fanex.NexHR.Core.Attributes;

using System;

[AttributeUsage(AttributeTargets.All)]
public sealed class StringValueAttribute(string value) : Attribute
{
    public string Value => value;

    public override string ToString() => value;
}