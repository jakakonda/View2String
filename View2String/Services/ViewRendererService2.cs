using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace View2String.Services
{


    public class MyViewComponentContext 
    {
        public HttpContext HttpContext { get; set; }
        public ActionContext ActionContext { get; set; }
        public ViewDataDictionary ViewData { get; set; }
        public ITempDataDictionary TempData { get; set; }
    }

    public class ViewRendererService2 : IViewRenderService
    {
        private ITempDataProvider _tempDataProvider;
        private IHttpContextAccessor _httpContextAccessor;
        private IActionContextAccessor _actionContext;

        public ViewRendererService2( ITempDataProvider tempDataProvider, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContext)
        {
            _tempDataProvider = tempDataProvider;
            _httpContextAccessor = httpContextAccessor;
            _actionContext = actionContext;
        }
        private class NullView : IView
        {
            public static readonly NullView Instance = new NullView();
            public string Path => string.Empty;
            public Task RenderAsync(ViewContext context)
            {
                if (context == null) { throw new ArgumentNullException(nameof(context)); }
                return Task.CompletedTask;
            }
        }
        private IViewComponentHelper GetViewComponentHelper(MyViewComponentContext context, StringWriter sw)
        { 
            var viewContext = new ViewContext(context.ActionContext, NullView.Instance, context.ViewData, context.TempData, sw, new HtmlHelperOptions());
            var helper = context.HttpContext.RequestServices.GetRequiredService<IViewComponentHelper>();
            (helper as IViewContextAware)?.Contextualize(viewContext);
            return helper;
        }
        private async Task<string> Render( MyViewComponentContext myViewComponentContext,string viewComponentName,object args) {
            using (var writer = new StringWriter())
            { 
                var helper = this.GetViewComponentHelper(myViewComponentContext, writer);
                var result = await helper.InvokeAsync(viewComponentName, args);
                result.WriteTo(writer, HtmlEncoder.Default);
                await writer.FlushAsync();
                return writer.ToString();
            }
        }

        public Task<string> RenderToStringAsync(string viewName, object model)
        {
            var httpContext = this._httpContextAccessor.HttpContext;
            var actionContext = new ActionContext( httpContext, httpContext.GetRouteData(), _actionContext.ActionContext.ActionDescriptor );
            var context = new MyViewComponentContext {
                HttpContext = httpContext,
                ActionContext = actionContext,
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                },
                TempData = new TempDataDictionary(
                    httpContext,
                    _tempDataProvider
                ),
            };
            return this.Render(context, viewName, model);
        }
    }
}