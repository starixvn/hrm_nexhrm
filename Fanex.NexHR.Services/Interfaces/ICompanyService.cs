using System.Collections.Generic;
using Fanex.NexHR.Models;

namespace Fanex.NexHR.Services.Interfaces
{
    public interface ICompanyService
    {
         List<CompanyModel> GetAllCompaniesInNexSystem();
    }
}
