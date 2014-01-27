// <copyright file="ReflectionModelBinder.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved. 
// </copyright>
//
// <summary>
//  Reflection Model Binder
// </summary>
//
// <history>
//  <change who="Anders" date="2011.09.14">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.ModelBinder
{
    using System;
    using System.Reflection;
    using System.Web.Mvc;

    /// <summary>
    /// Reflection model binder.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    public class ReflectionModelBinder<T> : CustomModelBinder where T : new()
    {
        /// <summary>
        /// Bind model.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="bindingContext">The binding context.</param>
        /// <returns>The return object.</returns>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var viewModel = (T)Activator.CreateInstance(typeof(T));
            Map(viewModel, bindingContext);

            return viewModel;
        }

        /// <summary>
        /// Map view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="bindingContext">The binding context.</param>
        /// <returns>The return object.</returns>
        public object Map(T viewModel, ModelBindingContext bindingContext)
        {
            // Not check property readonly
            PropertyInfo[] propertyList = viewModel.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in propertyList)
            {
                if (propertyInfo.CanWrite)
                {
                    if (bindingContext.PropertyMetadata.ContainsKey(propertyInfo.Name))
                    {
                        ValueProviderResult result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + propertyInfo.Name);

                        if (result == null && bindingContext.FallbackToEmptyPrefix)
                        {
                            result = bindingContext.ValueProvider.GetValue(propertyInfo.Name);
                        }

                        // just allow bind value type
                        if (result != null)
                        {
                            propertyInfo.SetValue(viewModel, result.ConvertTo(propertyInfo.PropertyType), null);
                        }
                    }
                }
            }

            return null;
        }
    }
}
