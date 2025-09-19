using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.NexHR.Models;

namespace Fanex.NexHR.Services.Interfaces
{
    public interface ICompanyService
    {
         List<CompanyModel> GetAllCompaniesInNexSystem();
    }
}
