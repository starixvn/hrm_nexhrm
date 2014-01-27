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
    using System.Web.Mvc;

    /// <summary>
    /// Custom model binder.
    /// </summary>
    public class CustomModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// Get value.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="bindingContext">The binding context.</param>
        /// <param name="key">The key value.</param>
        /// <returns>The return value.</returns>
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
