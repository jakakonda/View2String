using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace View2String.Components
{
    public class TestModel
    {
        public string StrToPrint { get; set; }
    }

    public class TestViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(TestModel model)
        {
            return View(model);
        }
    }
}
