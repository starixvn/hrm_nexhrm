// --------------------------------------------------------------------------
// <copyright file="StringValueAttribute.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Core.Attributes
{
    using System;

    /// <summary>
    /// String value attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class StringValueAttribute : Attribute
    {
        private readonly string _value;

        public StringValueAttribute(string value)
        {
            _value = value;
        }

        public string Value => _value;

        public override string ToString()
        {
            return _value;
        }
    }
}
