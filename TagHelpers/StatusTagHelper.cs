using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("status", Attributes = "value")]
public class StatusTagHelper : TagHelper
{
    public bool Value { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "i";    // Replaces <status> with <i> tag
        output.Attributes.SetAttribute("class", $"fa-solid fa-2x {(Value ? "fa-circle-check text-success" : "fa-circle-xmark text-danger")}");
    }
}
