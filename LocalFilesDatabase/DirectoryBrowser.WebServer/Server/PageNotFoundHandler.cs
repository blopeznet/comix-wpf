using Nancy;
using Nancy.ErrorHandling;
using Nancy.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryBrowser.WebServer.Server
{
    public class PageNotFoundHandler : DefaultViewRenderer, IStatusCodeHandler
    {
        public PageNotFoundHandler(IViewFactory factory) : base(factory)
        {
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound;
        }

        public void Handle(Nancy.HttpStatusCode statusCode, NancyContext context)
        {
            var response = RenderView(context, "PageNotFound");
            response.StatusCode = HttpStatusCode.NotFound;
            context.Response = response;
        }
    }
}
