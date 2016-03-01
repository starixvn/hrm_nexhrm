// --------------------------------------------------------------------------
// <copyright file="BundleEntensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.11.18">Created</change>
// </history>

namespace Fanex.NexHR.WebSite
{
    using System.Web.Optimization;
    using BundleTransformer.Core.Orderers;
    using BundleTransformer.Core.Transformers;

    public static class BundleEntensions
    {
        public static void AddAndInclude(this BundleCollection bundles, string bundleVirtualPath, params string[] virtualPaths)
        {
            if (bundles != null)
            {
                bundles.Add(new Bundle(bundleVirtualPath)
                {
                    Transforms = { new CssMinify() },
                    Orderer = new NullOrderer()
                }.Include(virtualPaths));
            }            
        }
    }
}