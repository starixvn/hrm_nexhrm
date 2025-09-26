using System.Collections.Generic;
using Fanex.NexHR.Models;
using Fanex.NexHR.Services.Interfaces;

namespace Fanex.NexHR.Services
{
    public class CompanyService : ICompanyService
    {
        public List<CompanyModel> GetAllCompaniesInNexSystem()
        {
            List<CompanyModel> companies =
            [
                new() { Id = 1, Name = "Nexcel" },
                new() { Id = 2, Name = "Lumiplus" },
                new() { Id = 34, Name = "NexVision" }
            ];

            return companies;
        }
    }
}
