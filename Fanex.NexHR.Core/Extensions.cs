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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.WebPages;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1615:ElementReturnValueMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1614:ElementParameterDocumentationMustHaveText", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1125:UseShorthandForNullableTypes", Justification = "Reviewed.")]
    public static class Extensions
    {
        private static readonly object _O = new object();

        /// <summary>
        /// Convert Command enumerable to string
        /// </summary>
        /// <param name="value">command enum</param>
        /// <returns>command string</returns>
        public static string EnumToString(this Commands value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];
            return attribs.Length > 0 ? attribs[0].ToString() : null;
        }

        public static string EnumToString(this RolesEnum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];
            return attribs.Length > 0 ? attribs[0].ToString() : null;
        }

        public static string EnumToString(this RequestTypes value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];
            return attribs.Length > 0 ? attribs[0].ToString() : null;
        }

        public static string Pager(this HtmlHelper htmlHelper, int pageNumber, int pageSize, int totalRow, string url)
        {
            var totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);

            var predot = pageNumber > 5;
            var nextdot = totalPage - pageNumber > 4;

            if (totalPage < 2)
            {
                return string.Empty;
            }

            var pager = new StringBuilder();
            if (!predot)
            {
                for (var i = 1; i < pageNumber; i++)
                {
                    pager.Append(CreatePageNode(url, i));
                }
            }
            else
            {
                pager.Append(CreatePageNode(url, 1));
                pager.Append(CreatePageNode(url, 2));
                pager.Append("<li>...</li>");
                pager.Append(CreatePageNode(url, pageNumber - 2));
                pager.Append(CreatePageNode(url, pageNumber - 1));
            }

            pager.Append(CreatePageNode(url, pageNumber, true));
            if (!nextdot)
            {
                for (var i = pageNumber + 1; i <= totalPage; i++)
                {
                    pager.Append(CreatePageNode(url, i));
                }
            }
            else
            {
                pager.Append(CreatePageNode(url, pageNumber + 1));
                pager.Append(CreatePageNode(url, pageNumber + 2));
                pager.Append("<li>...</li>");
                pager.Append(CreatePageNode(url, totalPage - 1));
                pager.Append(CreatePageNode(url, totalPage));
            }

            return pager.ToString();
        }

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
                    // e.g. <input data-val="true" data-val-required="You must select an option" id="TestRadio_1" name="TestRadio" type="radio" value="1" /><label for="TestRadio_1">Line1</label> 
                    sb.AppendFormat("<div class=\"radioButton\">{0}{1}</div>", radio, label);
                }
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcForm BeginGetForm(this HtmlHelper htmlHelper)
        {
            return htmlHelper.BeginForm(null, null, FormMethod.Get);
        }

        public static MvcHtmlString DatePicker(
            this HtmlHelper htmlHelper,
            string name,
            DateTime? date,
            DateTime? minDate = null,
            DateTime? maxDate = null,
            string formatDate = "dd MMM, yyyy",
            IEnumerable<DateTime> unavailDates = null)
        {
            if (string.IsNullOrWhiteSpace(formatDate))
            {
                throw new ArgumentNullException("formatDate cannot be null");
            }

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

            if (metaData.Model is Nullable<DateTime> || metaData.Model is DateTime)
            {
                value = (DateTime?)metaData.Model;
            }
            else
            {
                value = null;
            }

			return html.DatePicker(ExpressionHelper.GetExpressionText(expression), value, minDate, maxDate, formatDate, unavailDates);
		}

        public static DateTime GetMaxDate(this DateTime date)
        {
            if (date.AddMonths(1).Year > DateTime.Now.Year)
            {
                return DateTime.Now.AddYears(1);
            }

            return date;
        }

        /// <summary>
        /// year picker
        /// </summary>
        /// <param name="htmlHelper">html Helper</param>
        /// <param name="name">in name</param>
        /// <param name="fromYear">default from year 2000</param>
        /// <param name="currentYear">>default currentYear year 2000</param>
        /// <returns></returns>
        public static MvcHtmlString YearPicker(this HtmlHelper htmlHelper, string name, int fromYear = 2000, int currentYear = 2000)
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

        public static MvcHtmlString YearPickerFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            int fromYear = 2000)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var currentYear = (int)metaData.Model;

            return html.YearPicker(metaData.PropertyName, fromYear, currentYear);
        }

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

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name)
        {
            return CheckBoxList(htmlHelper, name, null);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList)
        {
            return CheckBoxList(htmlHelper, name, selectList, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return CheckBoxList(htmlHelper, name, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
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
                var valueList = values is string[]
                ? values as string[]
                : values.ToString().Split(new[] { ',' });

                IEnumerable source = values as IEnumerable;

                HashSet<string> set = new HashSet<string>(
                   from value in source.Cast<object>()
                   select Convert.ToString(value, CultureInfo.CurrentCulture), 
                   StringComparer.OrdinalIgnoreCase);

                List<SelectListItem> list = new List<SelectListItem>();

                foreach (SelectListItem item in selectList)
                {
                    item.Selected = (item.Value != null) && valueList.Contains(item.Value);
                    list.Add(item);
                }

                selectList = list;
            }

            StringBuilder sb = new StringBuilder();

            htmlAttributes = (htmlAttributes == null) ?
               new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes);

            htmlAttributes.Remove("checked");

            foreach (var item in selectList)
            {
                TagBuilder input = new TagBuilder("input");
                input.MergeAttributes(htmlAttributes);
                input.MergeAttribute("type", "checkbox");
                input.MergeAttribute("id", name + item.Value);
                input.MergeAttribute("name", fullHtmlFieldName, replaceExisting: true);
                input.MergeAttribute("value", item.Value ?? item.Text ?? string.Empty, replaceExisting: true);

                if (item.Selected)
                {
                    input.MergeAttribute("checked", "checked");
                }

                TagBuilder label = new TagBuilder("label");
                label.InnerHtml = input.ToString(TagRenderMode.SelfClosing) + item.Text;

                sb.Append(label.ToString());
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return CheckBoxListFor(htmlHelper, expression, null);
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            return CheckBoxListFor(htmlHelper, expression, selectList, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return CheckBoxListFor(htmlHelper, expression, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            return CheckBoxList(htmlHelper, ExpressionHelper.GetExpressionText(expression), selectList, htmlAttributes);
        }

        #region Business
        /// <summary>
        /// convert status to display string
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
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
        /// convert request type to display string
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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
        /// format a nullable datetime value to string (MMM dd, yyyy hh:mm)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToFormatedDate();
            }

            return string.Empty;
        }

        /// <summary>
        /// format a datetime value to string (MMM dd, yyyy hh:mm)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
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
        /// <example>http://www.geekzilla.co.uk/View00FF7904-B510-468C-A2C8-F859AA20581F.htm</example>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToShortFormatedDate(this DateTime date)
        {
            string dateStr = date.ToString(@"dd MMM, yyyy - HH") + "h" + date.ToString(@"mm\'");
            return dateStr;
        }

        /// <summary>
        /// Format a nullable datetime value to string (dd-MMM)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToMonthAndDayFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToMonthAndDayFormatedDate();
            }

            return string.Empty;
        }

        /// <summary>
        /// Foarmat da datatime value to string (dd-MMM)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToMonthAndDayFormatedDate(this DateTime date)
        {
            return date.ToString("dd-MMM");
        }

        /// <summary>
        /// Format a nullable datetime value to string (MMM-yyyy)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToMonthAndYearFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToMonthAndYearFormatedDate();
            }

            return string.Empty;
        }

        /// <summary>
        /// Foarmat da datatime value to string (MMM-yyyy)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
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
                    List<SelectListItem> list = new List<SelectListItem>();
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
        private static string CreatePageNode(string url, int pageNumber, bool isActive = false)
        {
            return isActive
                ? "<li><a class='active' href='javascript:;'>" + pageNumber + "</a></li>"
                : string.Format("<li><a rel='external' href='" + url + "'>{0}</a></li>", pageNumber);
        }
    }
}
