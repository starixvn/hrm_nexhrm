namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Fanex.NexHR.ViewModels.LateInEarlyOut;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Criteria.LateInEarlyOut;

    [Authorize]
    public class LateEarlyController : BaseController
    {
        private readonly ILateInEarlyOutService service;


        public LateEarlyController()
            : this(null)
        {
        }

        public LateEarlyController(ILateInEarlyOutService service)
        {
            this.service = service ?? new LateInEarlyOutService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public void Insert()
        {

        }

        public void Update()
        {

        }

        public void Delete()
        {

        }

        public ActionResult ListLateEarly(int empId)
        {
            return PartialView(GetLateEarlyByEmpId(empId));
        }

        private IList<LateInEarlyOutViewModel> GetLateEarlyByEmpId(int empId)
        {
            var criteria = new GetLateInEarlyOutByEmpIdCriteria() { EmpId = empId };
            IList<LateInEarlyOutViewModel> list = service.GetByEmpId(criteria).ToList();

            return list;
        }
    }
}
