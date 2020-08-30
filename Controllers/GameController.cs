using DragonsClaw.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DragonsClaw.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        [Authorize]
        public ActionResult Index()
        {
            if (!DataLayer.DoesPlayerExist(User.Identity.GetUserName()))
            {
                return RedirectToAction("Create");
            }

            return View(DataLayer.GetPlayer(User.Identity.GetUserName()));
        }

        [Authorize]
        public ActionResult Create()
        {
            if (DataLayer.DoesPlayerExist(User.Identity.GetUserName()))
            {
                return RedirectToAction("Index");
            }

            CreatePlayerModel model = new CreatePlayerModel();

            ExtractClassAndRace(model);
            return View(model);
        }

        private void ExtractClassAndRace(CreatePlayerModel model)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM Races";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<SelectListItem> items = new List<SelectListItem>
                        {
                            new SelectListItem() { Value = "-1", Text = "--------------------" }
                        };

                        while (reader.Read())
                        {
                            SelectListItem item = new SelectListItem
                            {
                                Text = reader.GetString(1),
                                Value = reader.GetInt32(0).ToString()
                            };

                            items.Add(item);
                        }

                        model.RaceList = new SelectList(items, "Value", "Text");

                        reader.Close();
                    }

                    command.CommandText = "SELECT * FROM Classes";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<SelectListItem> items = new List<SelectListItem>
                        {
                            new SelectListItem() { Value = "-1", Text = "--------------------" }
                        };

                        while (reader.Read())
                        {
                            SelectListItem item = new SelectListItem
                            {
                                Text = reader.GetString(1),
                                Value = reader.GetInt32(0).ToString()
                            };

                            items.Add(item);
                        }

                        model.ClassList = new SelectList(items, "Value", "Text");
                    }
                }
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(CreatePlayerModel model)
        {
            if (!ModelState.IsValid || model.Race == "-1" || model.Class == "-1" || model.Gender == "-1")
            {
                ExtractClassAndRace(model);
                return View(model);
            }

            DataLayer.CreatePlayer(model, User.Identity.GetUserName());

            return RedirectToAction("Index");
        }
    }
}