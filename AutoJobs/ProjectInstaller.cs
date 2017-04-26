// --------------------------------------------------------------------------
// <copyright file="ProjectInstaller.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.10.17">Created</change>
// </history>

namespace AutoJobs
{
    using System.ComponentModel;
    using System.Configuration.Install;

    /// <summary>
    /// Project Installer.
    /// </summary>
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
