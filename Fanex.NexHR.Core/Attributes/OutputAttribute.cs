// --------------------------------------------------------------------------
// <copyright file="OutputAttribute.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.12">Created</change>
// </history>

namespace Fanex.NexHR.Core.Attributes
{
    using System;

    /// <summary>
    /// Output Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OutputAttribute : Attribute
    {
        public OutputAttribute()
        {
        }
    }
}
