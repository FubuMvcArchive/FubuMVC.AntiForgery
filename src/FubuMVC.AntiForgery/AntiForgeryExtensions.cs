﻿using FubuMVC.Core;
using FubuMVC.Core.UI;

namespace FubuMVC.AntiForgery
{
    public class AntiForgeryExtensions : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Import<HtmlConventionRegistry>(x => x.Forms.Add(new AntiForgeryTagModifier()));
        }
    }
}