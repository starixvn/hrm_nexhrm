// --------------------------------------------------------------------------
// <copyright file="EmployeeControllerTest.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Site.Tests.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed.")]
    [TestClass]
    public class EmployeeControllerTest
    {
        private TestContext _testContextInstance;

        public EmployeeControllerTest()
        {
        }

        public TestContext TestContext
        {
            get
            {
                return this._testContextInstance;
            }

            set
            {
                this._testContextInstance = value;
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
