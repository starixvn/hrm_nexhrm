// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the
// Code Analysis results, point to "Suppress Message", and click
// "In Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Globalization",
    "CA1308:NormalizeStringsToUppercase",
    Justification = "Pending")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Globalization",
    "CA1308:NormalizeStringsToUppercase",
    Scope = "member",
    Target = "Fanex.NexHR.Services.RequestService.#SendMailForCheckInOut(Fanex.NexHR.ViewModels.CheckInOutRequest.CheckInOutRequestViewModel)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Sonar Code Smell",
    "S131:\"switch/Select\" statements should end with \"default/Case Else\" clauses",
    Justification = "Need only 3 cases",
    Scope = "member",
    Target = "~M:Fanex.NexHR.Services.RequestService.SendMailToRelatedPeople(Fanex.NexHR.Models.Request.RequestEntity,System.String,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Sonar Code Smell",
    "S2325:Methods and properties that don't access instance data should be static",
    Justification = "Required to run",
    Scope = "member",
    Target = "~M:Fanex.NexHR.WebSite.MvcApplication.Application_Start")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Globalization",
    "CA1308:NormalizeStringsToUppercase",
    Scope = "member",
    Target = "Fanex.NexHR.Core.Utilities.#RandomString(System.Int32)")]

