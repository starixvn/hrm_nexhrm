// --------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.WebPages;
    using Fanex.NexHR.Core.Attributes;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// Extension class.
    /// </summary>
    public static class Extensions
    {
        private static readonly object _O = new object();

        /// <summary>
        /// Convert Command enumerable to string.
        /// </summary>
        /// <param name="value">Command enumerable.</param>
        /// <returns>Command string.</returns>
        public static string EnumToString(this Command value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attribs.Length > 0 ? attribs[0].ToString() : null;
        }

        /// <summary>
        /// Convert ENUM to string.
        /// </summary>
        /// <param name="value">The enumerable value.</param>
        /// <returns>The return string.</returns>
        public static string EnumToString(this Role value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attribs.Length > 0 ? attribs[0].ToString() : null;
        }

        /// <summary>
        /// Convert ENUM to string.
        /// </summary>
        /// <param name="value">The enumerable value.</param>
        /// <returns>The return string.</returns>
        public static string EnumToString(this RequestType value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attribs.Length > 0 ? attribs[0].ToString() : null;
        }

        /// <summary>
        /// Radio button list.
        /// </summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="listOfValues">List of value.</param>
        /// <returns>MVC HTML string.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed. Suppression is OK here.")]
        public static MvcHtmlString RadioButtonList<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> listOfValues)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var sb = new StringBuilder();

            if (listOfValues != null)
            {
                // Create a radio button for each item in the list 
                foreach (SelectListItem item in listOfValues)
                {
                    // Generate an id to be given to the radio button field 
                    var id = string.Format("{0}_{1}", metaData.PropertyName, item.Value);

                    // Create and populate a radio button using the existing html helpers 
                    var label = htmlHelper.Label(id, HttpUtility.HtmlEncode(item.Text));
                    var radio = htmlHelper.RadioButtonFor(expression, item.Value, new { id = id }).ToHtmlString();

                    // Create the html string that will be returned to the client 
                    // e.g. <input data-val="true" data-val-required="You must select an option" id="TestRadio_1" name="TestRadio" type="radio" value="1" />
                    // <label for="TestRadio_1">Line1</label> 
                    sb.AppendFormat("<div class=\"radioButton\">{0}{1}</div>", radio, label);
                }
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        /// <summary>
        /// Begin get form.
        /// </summary>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <returns>The MVC form.</returns>
        public static MvcForm BeginGetForm(this HtmlHelper htmlHelper)
        {
            return htmlHelper.BeginForm(null, null, FormMethod.Get);
        }

        /// <summary>
        /// Date picker.
        /// </summary>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <param name="name">The element name.</param>
        /// <param name="date">The date value.</param>
        /// <param name="minDate">The min date.</param>
        /// <param name="maxDate">The max date.</param>
        /// <param name="formatDate">The format date.</param>
        /// <param name="unavailDates">The unavailable dates.</param>
        /// <returns>MVC HTML string.</returns>
        public static MvcHtmlString DatePicker(
            this HtmlHelper htmlHelper,
            string name,
            DateTime? date,
            DateTime? minDate = null,
            DateTime? maxDate = null,
            string formatDate = "dd MMM, yyyy",
            IEnumerable<DateTime> unavailDates = null)
        {
            string disabledDate = (unavailDates != null && unavailDates.Count() > 0)
                ? string.Join(",", unavailDates.Select(d => d.ToString("dd MMM yyyy")))
                : string.Empty;

            var htmlAttribute = new
                {
                    @class = "datepicker",
                    mindate = minDate != null && minDate != DateTime.MinValue ? ((DateTime)minDate).ToString("yyyy/MM/dd") : "null",
                    maxdate = maxDate != null && maxDate != DateTime.MinValue ? ((DateTime)maxDate).ToString("yyyy/MM/dd") : "null",
                    disabledDate = disabledDate
                };

            var dateValue = date.HasValue ? date.Value.ToString(formatDate) : string.Empty;

            var textbox = htmlHelper.TextBox(name, dateValue, htmlAttribute);

            return MvcHtmlString.Create(textbox.ToString());
        }

        /// <summary>
        /// Date picker.
        /// </summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="html">HTML helper.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="minDate">The min date.</param>
        /// <param name="maxDate">The max date.</param>
        /// <param name="formatDate">The format date.</param>
        /// <param name="unavailDates">The unavailable dates.</param>
        /// <returns>MVC HTML string.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed. Suppression is OK here.")]
        public static MvcHtmlString DatePickerFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            DateTime? minDate = null,
            DateTime? maxDate = null,
            string formatDate = "dd MMM, yyyy",
            IEnumerable<DateTime> unavailDates = null)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            DateTime? value = DateTime.Now;

            if (metaData.Model is DateTime? || metaData.Model is DateTime)
            {
                value = (DateTime?)metaData.Model;
            }
            else
            {
                value = null;
            }

            return html.DatePicker(
                ExpressionHelper.GetExpressionText(expression), 
                value, 
                minDate, 
                maxDate, 
                formatDate, 
                unavailDates);
        }

        /// <summary>
        /// Get max date.
        /// </summary>
        /// <param name="date">The date value.</param>
        /// <returns>The return date time.</returns>
        public static DateTime GetMaxDate(this DateTime date)
        {
            if (date.AddMonths(1).Year > DateTime.Now.Year)
            {
                return DateTime.Now.AddYears(1);
            }

            return date;
        }

        /// <summary>
        /// Year picker.
        /// </summary>
        /// <param name="htmlHelper">HTML Helper.</param>
        /// <param name="name">Input name.</param>
        /// <param name="fromYear">Default from year 2000.</param>
        /// <param name="currentYear">Default currentYear year 2000.</param>
        /// <returns>MVC HTML string.</returns>
        public static MvcHtmlString YearPicker(
            this HtmlHelper htmlHelper, 
            string name, 
            int fromYear = 2000, 
            int currentYear = 2000)
        {
            var list = new List<SelectListItem>();
            var toYear = DateTime.Now.GetMaxDate().Year;

            for (var i = toYear; i >= fromYear; i--)
            {
                list.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = i == currentYear
                });
            }

            var dropDownList = htmlHelper.DropDownList(name, list, new { @class = "yearpicker" });
            return MvcHtmlString.Create(dropDownList.ToString());
        }

        /// <summary>
        /// Year picker.
        /// </summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="html">HTML helper.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="fromYear">The from year.</param>
        /// <returns>MVC HTML string.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed. Suppression is OK here.")]
        public static MvcHtmlString YearPickerFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            int fromYear = 2000)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var currentYear = (int)metaData.Model;

            return html.YearPicker(metaData.PropertyName, fromYear, currentYear);
        }

        /// <summary>
        /// Render section.
        /// </summary>
        /// <param name="page">The web page.</param>
        /// <param name="sectionName">The section name.</param>
        /// <param name="defaultContent">The default content.</param>
        /// <returns>Helper result.</returns>
        public static HelperResult RenderSection(
            this WebPageBase page,
            string sectionName,
            Func<object, HelperResult> defaultContent)
        {
            if (page.IsSectionDefined(sectionName))
            {
                return page.RenderSection(sectionName);
            }
            else
            {
                return defaultContent(_O);
            }
        }

        /// <summary>
        /// Check box list.
        /// </summary>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <param name="name">The element name.</param>
        /// <returns>MVC HTML string.</returns>
        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name)
        {
            return CheckBoxList(htmlHelper, name, null);
        }

        /// <summary>
        /// Check box list.
        /// </summary>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <param name="name">The element name.</param>
        /// <param name="selectList">Select item list.</param>
        /// <returns>MVC HTML string.</returns>
        public static MvcHtmlString CheckBoxList(
            this HtmlHelper htmlHelper, 
            string name, 
            IEnumerable<SelectListItem> selectList)
        {
            return CheckBoxList(htmlHelper, name, selectList, (IDictionary<string, object>)null);
        }

        /// <summary>
        /// Check box list.
        /// </summary>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <param name="name">The element name.</param>
        /// <param name="selectList">Select item list.</param>
        /// <param name="htmlAttributes">HTML attributes.</param>
        /// <returns>MVC HTML string.</returns>
        public static MvcHtmlString CheckBoxList(
            this HtmlHelper htmlHelper, 
            string name, 
            IEnumerable<SelectListItem> selectList, 
            object htmlAttributes)
        {
            return CheckBoxList(htmlHelper, name, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Check box list.
        /// </summary>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <param name="name">The element name.</param>
        /// <param name="selectList">Select item list.</param>
        /// <param name="htmlAttributes">HTML attributes.</param>
        /// <returns>MVC HTML string.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public static MvcHtmlString CheckBoxList(
            this HtmlHelper htmlHelper, 
            string name, 
            IEnumerable<SelectListItem> selectList, 
            IDictionary<string, object> htmlAttributes)
        {
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

            if (string.IsNullOrEmpty(fullHtmlFieldName))
            {
                throw new ArgumentException("name cannot be null or empty.", "name");
            }

            bool selectListFromViewData = false;

            if (selectList == null)
            {
                selectList = htmlHelper.ViewData.Eval(fullHtmlFieldName) as IEnumerable<SelectListItem>;
                selectListFromViewData = true;
            }

            if (selectList == null)
            {
                throw new ArgumentNullException("selectList");
            }

            object values = null;
            ModelState state;

            if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out state)
               && (state.Value != null))
            {
                values = state.Value.ConvertTo(typeof(string[]), null);
            }

            if (!selectListFromViewData && (values == null))
            {
                values = htmlHelper.ViewData.Eval(fullHtmlFieldName);
            }

            if (values != null)
            {
                var valueList = values.GetType() == typeof(string[])
                ? values as string[]
                : values.ToString().Split(new[] { ',' });

                var list = new List<SelectListItem>();

                foreach (SelectListItem item in selectList)
                {
                    item.Selected = (item.Value != null) && valueList.Contains(item.Value);
                    list.Add(item);
                }

                selectList = list;
            }

            var sb = new StringBuilder();

            htmlAttributes = (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes);
            htmlAttributes.Remove("checked");

            foreach (var item in selectList)
            {
                var input = new TagBuilder("input");

                input.MergeAttributes(htmlAttributes);
                input.MergeAttribute("type", "checkbox");
                input.MergeAttribute("id", name + item.Value);
                input.MergeAttribute("name", fullHtmlFieldName, replaceExisting: true);
                input.MergeAttribute("value", item.Value ?? item.Text ?? string.Empty, replaceExisting: true);

                if (item.Selected)
                {
                    input.MergeAttribute("checked", "checked");
                }

                var label = new TagBuilder("label");
                label.InnerHtml = input.ToString(TagRenderMode.SelfClosing) + item.Text;

                sb.Append(label.ToString());
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed. Suppression is OK here.")]
        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper, 
            Expression<Func<TModel, TProperty>> expression)
        {
            return CheckBoxListFor(htmlHelper, expression, null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed. Suppression is OK here.")]
        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper, 
            Expression<Func<TModel, TProperty>> expression, 
            IEnumerable<SelectListItem> selectList)
        {
            return CheckBoxListFor(htmlHelper, expression, selectList, (IDictionary<string, object>)null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed. Suppression is OK here.")]
        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper, 
            Expression<Func<TModel, TProperty>> expression, 
            IEnumerable<SelectListItem> selectList, 
            object htmlAttributes)
        {
            return CheckBoxListFor(htmlHelper, expression, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Reviewed. Suppression is OK here.")]
        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper, 
            Expression<Func<TModel, TProperty>> expression, 
            IEnumerable<SelectListItem> selectList, 
            IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            return CheckBoxList(htmlHelper, ExpressionHelper.GetExpressionText(expression), selectList, htmlAttributes);
        }

        #region Business
        /// <summary>
        /// Convert status to display string.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>The return string.</returns>
        public static string ToText(this byte status)
        {
            switch (status)
            {
                case 1:
                    return Labels.Pending;
                case 2:
                    return Labels.Approved;
                case 3:
                    return Labels.Rejected;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Convert request type to display string.
        /// </summary>
        /// <param name="type">The request type.</param>
        /// <returns>The return string.</returns>
        public static string ToText(this short type)
        {
            switch (type)
            {
                case 1:
                    return Labels.CheckIn;
                case 2:
                    return Labels.CheckOut;
                case 3:
                    return Labels.LateIn;
                case 4:
                    return Labels.EarlyOut;
                case 5:
                    return Labels.ChangeShift;
                case 6:
                    return Labels.Leave;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Format a date value to string.
        /// </summary>
        /// <param name="date">The date value.</param>
        /// <returns>The return string.</returns>
        public static string ToFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToFormatedDate();
            }

            return string.Empty;
        }

        /// <summary>
        /// Format a date value to string.
        /// </summary>
        /// <param name="date">The date value.</param>
        /// <returns>The return string.</returns>
        public static string ToFormatedDate(this DateTime date)
        {
            return date.ToString("dd MMM, yyyy hh:mm tt");
        }

        public static string ToShortFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToShortFormatedDate();
            }

            return string.Empty;
        }

        /// <summary>
        /// Convert to short format date.
        /// </summary>
        /// <param name="date">The date value.</param>
        /// <returns>The return string.</returns>
        public static string ToShortFormatedDate(this DateTime date)
        {
            string dateStr = date.ToString(@"dd MMM, yyyy - HH") + "h" + date.ToString(@"mm\'");
            return dateStr;
        }

        /// <summary>
        /// Format a date value to string.
        /// </summary>
        /// <param name="date">The date value.</param>
        /// <returns>The return string.</returns>
        public static string ToMonthAndDayFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToMonthAndDayFormatedDate();
            }

            return string.Empty;
        }

        /// <summary>
        /// Format a date value to string.
        /// </summary>
        /// <param name="date">The date value.</param>
        /// <returns>The return string.</returns>
        public static string ToMonthAndDayFormatedDate(this DateTime date)
        {
            return date.ToString("dd-MMM");
        }

        /// <summary>
        /// Format a date value to string (MMM-yyyy).
        /// </summary>
        /// <param name="date">The date value.</param>
        /// <returns>The return string.</returns>
        public static string ToMonthAndYearFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToMonthAndYearFormatedDate();
            }

            return string.Empty;
        }

        /// <summary>
        /// Format a date value to string (MMM-yyyy).
        /// </summary>
        /// <param name="date">The date value.</param>
        /// <returns>The return string.</returns>
        public static string ToMonthAndYearFormatedDate(this DateTime date)
        {
            return date.ToString("MMM-yyyy");
        }

        public static string ToDateMonthYearFormatedDate(this DateTime date)
        {
            return date.ToString("dd MMM, yyyy");
        }

        public static string ToDateMonthYearFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToDateMonthYearFormatedDate();
            }

            return string.Empty;
        }

        /// <summary>
        /// Drop down list.
        /// </summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="selectList">Select item list.</param>
        /// <param name="firstItem">The first item.</param>
        /// <param name="htmlAttributes">HTML attributes.</param>
        /// <returns>MVC HTML string.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed. Suppression is OK here.")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            SelectListItem firstItem,
            object htmlAttributes)
        {
            if (firstItem != null)
            {
                if (firstItem.Value != null)
                {
                    var list = new List<SelectListItem>();
                    list.Add(firstItem);

                    if (selectList != null)
                    {
                        foreach (SelectListItem item in selectList)
                        {
                            list.Add(item);
                        }
                    }

                    selectList = list;
                }
            }

            return SelectExtensions.DropDownListFor(htmlHelper, expression, selectList, htmlAttributes);
        }

        #endregion
    }
}
