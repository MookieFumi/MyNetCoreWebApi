using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;

namespace mywebapi.Infrastructure
{
    /// <summary>
    /// http://kurtdowswell.com/software-development/asp-net-core-mvc-feature-folders/
    /// https://github.com/kdowswell/asp-net-core-mvc-feature-folders
    /// </summary>
    public class FeatureFolderViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (viewLocations == null)
            {
                throw new ArgumentNullException(nameof(viewLocations));
            }

            // {0} - Action Name
            // {1} - Controller Name
            // {2} - Area name

            //Features
            yield return "/Features/{1}/{0}.cshtml";
            yield return "/Features/{1}/Views/{0}.cshtml";

            //Feature Areas
            yield return "/Features/{2}/{1}/{0}.cshtml";
            yield return "/Features/{2}/{1}/Views/{0}.cshtml";
            yield return "/Features/{2}/Shared/{0}.cshtml";

            //Shared
            yield return "/Features/Shared/{0}.cshtml";
            yield return "/Views/Shared/{0}.cshtml";
        }
    }
}