// <copyright file="CustomModelBinder.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved. 
// </copyright>
//
// <summary>
//  Custom Model Binder
// </summary>
//
// <history>
//  <change who="Anders" date="2011.09.14">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.ModelBinder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1125:UseShorthandForNullableTypes", Justification = "Reviewed.")]
    public class CustomModelBinder : DefaultModelBinder
    {
        protected Nullable<T> GetValue<T>(ModelBindingContext bindingContext, string key) where T : struct
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            ValueProviderResult result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + key);

            if (result == null && bindingContext.FallbackToEmptyPrefix)
            {
                result = bindingContext.ValueProvider.GetValue(key);
            }

            if (result == null)
            {
                return null;
            }

            return (Nullable<T>)result.ConvertTo(typeof(T));
        }
    }
}
