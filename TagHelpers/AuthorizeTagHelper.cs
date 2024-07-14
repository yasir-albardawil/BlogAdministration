using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

[HtmlTargetElement(Attributes = "asp-authorize")]
public class AuthorizeTagHelper : TagHelper
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IHtmlGenerator _htmlGenerator;

    public AuthorizeTagHelper(
        IAuthorizationService authorizationService,
        IHtmlGenerator htmlGenerator)
    {
        _authorizationService = authorizationService;
        _htmlGenerator = htmlGenerator;
    }

    [HtmlAttributeName("asp-authorize")]
    public string Role { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrEmpty(Role))
        {
            output.SuppressOutput();
            return;
        }

        var result = await _authorizationService.AuthorizeAsync(ViewContext.HttpContext.User, Role);

        if (!result.Succeeded)
        {
            output.SuppressOutput();
        }
    }
}
