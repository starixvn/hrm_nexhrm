// --------------------------------------------------------------------------
// <copyright file="ITOEICService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.05.31">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using Fanex.NexHR.Models;

	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
	public interface ITOEICService
	{
		TOEIC GetById(int empId);

		int Update(TOEIC model);

		int NewUpdate(TOEIC model);
	}
}
