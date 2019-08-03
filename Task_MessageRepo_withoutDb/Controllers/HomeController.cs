using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_MessageRepo_withoutDb.Models;

namespace Task_MessageRepo_withoutDb.Controllers
{
    public class HomeController : Controller
    {
        internal static List<Message> jsonMessages = GetDataFromJson();
        Message message = new Message();
        private static List<Message> GetDataFromJson()
        {
            if (jsonMessages == null)
            {
                jsonMessages = new List<Message>();
            }
            using (StreamReader file = System.IO.File.OpenText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\MessagesDatabase.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                var usersList_Deserialize = (List<Message>)serializer.Deserialize(file, typeof(List<Message>));
                return usersList_Deserialize;
            }
        }

        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Customers = AccountController.applicationUsers;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(ApplicationUser user)
        {
            string outputUsers = "";
            string outputMessages = "";
            using (StreamReader file = System.IO.File.OpenText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\UsersDatabase.json"))
            {
                using (StreamReader file_messages = System.IO.File.OpenText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\MessagesDatabase.json"))
                {
                    foreach (var _user in AccountController.applicationUsers)
                    {
                        if (_user.Email == User.Identity.Name)
                        {
                            Message lastMessage = jsonMessages.LastOrDefault();
                            if (lastMessage == null)
                            {
                                message.Id = 1;
                            }
                            else
                            {
                                int index = lastMessage.Id;
                                message.Id = ++index;
                            }

                            _user.LastMessage = user.LastMessage;
                            message.DateTime = DateTime.Now;
                            message.ApplicationUserId = _user.Id;
                            message.UserName = User.Identity.Name;
                            message.Mess = _user.LastMessage;

                            message.User = _user;
                            jsonMessages.Add(message);
                        }
                    }
                    outputUsers = JsonConvert.SerializeObject(AccountController.applicationUsers, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                }
                outputMessages = JsonConvert.SerializeObject(jsonMessages, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            System.IO.File.WriteAllText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\UsersDatabase.json", outputUsers);
            System.IO.File.WriteAllText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\MessagesDatabase.json", outputMessages);

            ViewBag.Customers = AccountController.applicationUsers;
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult ViewMyMessages(string id, string sortOrder)
        {
            ViewBag.IdSortParam = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";
            var AllUserMessages = jsonMessages.FindAll(i => i.ApplicationUserId == id);
            var messages = from s in AllUserMessages
                           select s;
            switch (sortOrder)
            {
                case "id_desc":
                    messages = messages.OrderByDescending(s => s.Id);
                    break;
                case "Date":
                    messages = messages.OrderBy(s => s.DateTime);
                    break;
                case "date_desc":
                    messages = messages.OrderByDescending(s => s.DateTime);
                    break;
                default:
                    messages = messages.OrderBy(s => s.Id);
                    break;
            }
            return View(messages.ToList());
        }

        [Authorize]
        [HttpGet]
        public ActionResult ViewAllMessages(string sortOrder)
        {
            ViewBag.IdSortParam = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";
            var messages = from mess in jsonMessages
                           select mess;
            switch (sortOrder)
            {
                case "id_desc":
                    messages = messages.OrderByDescending(s => s.Id);
                    break;
                case "Date":
                    messages = messages.OrderBy(s => s.DateTime);
                    break;
                case "date_desc":
                    messages = messages.OrderByDescending(s => s.DateTime);
                    break;
                default:
                    messages = messages.OrderBy(s => s.Id);
                    break;
            }
            return View(messages.ToList());
        }

        [Authorize]
        public RedirectToRouteResult DeleteMessage(int id)
        {
            string outputMessages = "";
            using (StreamReader file = System.IO.File.OpenText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\MessagesDatabase.json"))
            {
                Message messageForDel = jsonMessages.Find(i => i.Id == id);
                jsonMessages?.Remove(messageForDel);

                outputMessages = JsonConvert.SerializeObject(jsonMessages, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            System.IO.File.WriteAllText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\MessagesDatabase.json", outputMessages);       
            return RedirectToAction("ViewAllMessages");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "My contacts.";
            return View();
        }
    }
}