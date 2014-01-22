// --------------------------------------------------------------------------
// <copyright file="ControllerFactory.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Ninject;
    using Ninject.Modules;

    public class ControllerFactory : DefaultControllerFactory
    {
        private IKernel _kernel = new StandardKernel(new FanexModule());

        protected override IController GetControllerInstance(RequestContext context, Type controllerType)
        {
            if (controllerType == null)
            {
                return null;
            }
            else
            {
                return (IController)_kernel.Get(controllerType);
            }
        }

        private class FanexModule : NinjectModule
        {
            public override void Load()
            {
            }
        }
    }
}
