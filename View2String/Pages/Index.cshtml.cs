using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using View2String.Components;
using View2String.Services;

namespace View2String.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IViewRenderService _viewRender;

        public IndexModel(IViewRenderService viewRender)
        {
            _viewRender = viewRender;
        }

        public async Task OnGet()
        {
            Model = await _viewRender.RenderToStringAsync(
                "Test",
                new TestModel { StrToPrint = "Print From Service" });
        }

        public string Model { get; set; }
    }
}
