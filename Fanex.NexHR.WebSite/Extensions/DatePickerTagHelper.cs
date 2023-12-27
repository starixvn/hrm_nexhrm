namespace Fanex.NexHR.WebSite.Extensions
{
    using System.Text;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement("date-picker")]
    public class DatePickerTagHelper : TagHelper
    {
        [HtmlAttributeName("name")]
        public string Name { get; set; } = string.Empty;

        [HtmlAttributeName("value")]
        public DateTime? Date { get; set; }

        [HtmlAttributeName("min-date")]
        public DateTime? MinDate { get; set; }

        [HtmlAttributeName("max-date")]
        public DateTime? MaxDate { get; set; }

        [HtmlAttributeName("format")]
        public string Format { get; set; } = "dd MMM, yyyy";

        [HtmlAttributeName("disabled-dates")]
        public IEnumerable<DateTime>? InvaidDates { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;

            string disabledDate = (InvaidDates != null && InvaidDates.Any())
                ? string.Join(",", InvaidDates.Select(d => d.ToString("dd MMM yyyy")))
                : string.Empty;

            var minDateString = MinDate != null && MinDate != DateTimeOffset.MinValue.DateTime
                ? ((DateTime)MinDate).ToString("yyyy/MM/dd")
                : "null";

            var maxDateString = MaxDate != null && MaxDate != DateTimeOffset.MinValue.DateTime
                ? ((DateTime)MaxDate).ToString("yyyy/MM/dd")
                : "null";

            var dateValue = Date?.ToString(Format) ?? string.Empty;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("<input type=\"text\" class=\"datepicker\" id=\"{0}\" name=\"{0}\" minDate=\"{1}\" maxDate=\"{2}\" disabledDate=\"{3}\" value=\"{4}\"></input>",
                    Name,
                    minDateString,
                    maxDateString,
                    disabledDate,
                    dateValue);
            output.Content.SetHtmlContent(stringBuilder.ToString());
        }
    }
}