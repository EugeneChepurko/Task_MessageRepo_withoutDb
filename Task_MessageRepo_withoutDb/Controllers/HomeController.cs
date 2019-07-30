using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Task_MessageRepo_withoutDb.Models;

namespace Task_MessageRepo_withoutDb.Controllers
{
    public class HomeController : Controller
    {
        internal static List<Message> jsonMessages = GetDataFromJson();

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

        ApplicationContext db = new ApplicationContext();
        Message message = new Message();

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
            //IEnumerable<ApplicationUser> users = db.Users;
            //ViewBag.Customers = users;

            ViewBag.Customers = AccountController.applicationUsers; // for json storage
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Index(ApplicationUser user)
        {
            string outputUsers = "";
            string outputMessages = "";
            // ???????????????? IEnumerable<ApplicationUser> users = db.Users /*UserManager.Users*/;

            ApplicationUser foundUser = await UserManager.FindByEmailAsync(User.Identity.Name);
            foundUser.LastMessage = user.LastMessage;
            message.DateTime = DateTime.Now;
            message.UserName = User.Identity.Name;
            if (foundUser.UserMessages == null)
            {
                message.ApplicationUserId = foundUser.Id;
                message.Mess = foundUser.LastMessage;

                foundUser.UserMessages = new List<Message>();
                foundUser.UserMessages.Add(message);
            }
            else
            {
                message.ApplicationUserId = foundUser.Id;
                message.Mess = foundUser.LastMessage;
                foundUser.UserMessages.Add(message);
            }

            IdentityResult result = await UserManager.UpdateAsync(foundUser);
            if (result.Succeeded)
            {
                // FOR adding to JSON WITHOUT db
                using (StreamReader file = System.IO.File.OpenText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\UsersDatabase.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    using (StreamReader file_messages = System.IO.File.OpenText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\MessagesDatabase.json"))
                    {
                        foreach (var _user in AccountController.applicationUsers)
                        {
                            if (_user.Email == User.Identity.Name)
                            {
                                _user.LastMessage = user.LastMessage;
                                message.DateTime = DateTime.Now;
                                message.ApplicationUserId = _user.Id;
                                message.UserName = User.Identity.Name;
                                message.Mess = _user.LastMessage;
                                jsonMessages.Add(message);
                                if (_user.UserMessages == null)
                                {
                                    _user.UserMessages = new List<Message>();
                                    _user.UserMessages.Add(message);
                                }
                                else
                                    _user.UserMessages.Add(message);
                            }
                        }
                        outputUsers = JsonConvert.SerializeObject(AccountController.applicationUsers, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    }
                    outputMessages = JsonConvert.SerializeObject(jsonMessages, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                }
                System.IO.File.WriteAllText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\UsersDatabase.json", outputUsers);
                System.IO.File.WriteAllText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\MessagesDatabase.json", outputMessages);
                // END adding to JSON WITHOUT db
                await db.SaveChangesAsync();
            }
            ViewBag.Customers = AccountController.applicationUsers;  // for json storage
            ViewBag.list = foundUser.UserMessages;
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult ViewMyMessages(string id, string sortOrder)
        {
            ApplicationUser jsonuser = AccountController.applicationUsers.FirstOrDefault(i => i.Id == id);

            ViewBag.IdSortParam = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";
            var messages = from s in jsonuser.UserMessages
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
            // !!! remove message with using Json !!!
            string outputMessages = "";
            string outputUsers = "";
            using (StreamReader file = System.IO.File.OpenText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\MessagesDatabase.json"))
            {
                foreach (var messItem in jsonMessages)
                {
                    foreach (var item in messItem.User.UserMessages)
                    {
                        if (item.Id == id)
                        {
                            messItem.User.UserMessages.Remove(item);
                            break;
                        }
                    }
                }
                Message messageForDel = jsonMessages.Find(i => i.Id == id);
                jsonMessages?.Remove(messageForDel);

                outputMessages = JsonConvert.SerializeObject(jsonMessages, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            System.IO.File.WriteAllText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\MessagesDatabase.json", outputMessages);

            using (StreamReader file = System.IO.File.OpenText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\UsersDatabase.json"))
            {
                foreach (var user in AccountController.applicationUsers)
                {
                    if (user.UserMessages.Contains(user.UserMessages.FirstOrDefault(i => i.Id == id)))
                    {
                        Message messToDel = user.UserMessages.FirstOrDefault(i => i.Id == id);
                        user?.UserMessages?.Remove(messToDel);
                        break;
                    }
                }
                outputUsers = JsonConvert.SerializeObject(AccountController.applicationUsers, Formatting.Indented);
            }
            System.IO.File.WriteAllText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\UsersDatabase.json", outputUsers);
            // !!! END remove message with using Json !!!    
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