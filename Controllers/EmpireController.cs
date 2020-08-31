using DragonsClaw.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DragonsClaw.Controllers
{
    public class EmpireController : Controller
    {
        // GET: Empire
        [Authorize]
        public ActionResult Index()
        {
            EmpireViewModel model = DataLayer.GetEmpire(User.Identity.GetUserName());

            if (model.EmpireId == -1)
            {
                return RedirectToAction("List");
            }

            return PartialView("_EmpireView", model);
        }

        [Authorize]
        public ActionResult List()
        {
            EmpireListViewModel model = DataLayer.GetEmpireList();

            return PartialView("_EmpireListView", model);
        }

        [Authorize]
        public ActionResult Create()
        {
            return PartialView("_CreateEmpireView");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(CreateEmpireModel model)
        {
            PlayerViewModel player = DataLayer.GetPlayer(User.Identity.GetUserName());

            if (!ModelState.IsValid)
            {
                model.Message = "Please enter a name and check the confirmation check box.";
                return PartialView("_CreateEmpireView", model);
            }

            if (string.IsNullOrEmpty(model.EmpireName))
            {
                model.Message = "You must enter a name for the empire.";
                return PartialView("_CreateEmpireView", model);
            }

            if (model.Create == false)
            {
                model.Message = "You must accept the terms for creating an empire.";
                return PartialView("_CreateEmpireView", model);
            }

            if (player.Credits < 1000000)
            {
                model.Message = "You do not have the 1,000,000cr.";
                return PartialView("_CreateEmpireView", model);
            }

            bool result = DataLayer.CreateEmpire(model, player);

            if (result)
            {
                return PartialView("_CreateEmpireSuccessView");
            }

            model.Message = "There was an error creating the empire. Try again later.";

            return PartialView("_CreateEmpireView", model);
        }
    }
}