﻿using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Common.Constants;
using Models;

namespace Web.Filters
{
    public class OpenGraphPageFilter : IAsyncPageFilter
    {
        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            // do something before the action executes
            var resultContext = await next();
            // do something after the action executes; resultContext.Result will be set

            if (resultContext.HandlerInstance is PageModel result)
            {
                var metaTags = result.ViewData[ViewDataConstants.OpenGraphViewModel] as OpenGraphViewModel;
                if (metaTags == null)
                {
                    metaTags = new OpenGraphViewModel();
                }

                if (!string.IsNullOrEmpty(metaTags.Title))
                {
                    metaTags.Title = "Web";
                }
                if (!string.IsNullOrEmpty(metaTags.Description))
                {
                    metaTags.Description = "SCL Application";
                }
                metaTags.Url = context.HttpContext.Request.GetDisplayUrl();
                metaTags.Type = "website";

                result.ViewData[ViewDataConstants.OpenGraphViewModel] = metaTags;
            }
        }

        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {

        }
    }
}
