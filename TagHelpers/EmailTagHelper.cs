using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace PieShop.TagHelpers
{
    public class EmailTagHelper : TagHelper
    {
        public string Address { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
        public string Content { get; set; }

        public string Text { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";    // Replaces <email> with <a> tag

            output.Attributes.SetAttribute("href", $"mailto: {Address}?subject={Subject}&&body={Body}");
            output.Content.SetContent(Content);
        }
    }
}