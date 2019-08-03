using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Task_MessageRepo_withoutDb.Models;

namespace Task_MessageRepo_withoutDb.Controllers
{
    public class AccountController : Controller
    {
        internal static List<ApplicationUser> applicationUsers = GetDataFromJson();

        private static List<ApplicationUser> GetDataFromJson()
        {
            if (applicationUsers == null)
            {
                applicationUsers = new List<ApplicationUser>();
            }
            using (StreamReader file = System.IO.File.OpenText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\UsersDatabase.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                var usersList_Deserialize = (List<ApplicationUser>)serializer.Deserialize(file, typeof(List<ApplicationUser>));
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

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            string outputUsers = "";
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    applicationUsers.Add(user);
                    using (StreamReader file = System.IO.File.OpenText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\UsersDatabase.json"))
                    {
                        outputUsers = JsonConvert.SerializeObject(applicationUsers, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    }
                    System.IO.File.WriteAllText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\UsersDatabase.json", outputUsers);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return View(model);
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser jsonUser = applicationUsers.Find(m => m.Email == model.Email);
                if (jsonUser == null)
                {
                    ModelState.AddModelError("", "Login or password is incorrect.");
                }
                else
                {
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(jsonUser,
                                            DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    if (string.IsNullOrEmpty(returnUrl))
                        return RedirectToAction("Index", "Home");
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            string outputUsers = "";
            string outputMessages = "";
            ApplicationUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    using (StreamReader file = System.IO.File.OpenText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\UsersDatabase.json"))
                    {
                        using (StreamReader file1 = System.IO.File.OpenText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\MessagesDatabase.json"))
                        {
                            var messages = HomeController.jsonMessages.FindAll(s => s.ApplicationUserId == user.Id);
                            foreach (var message in messages)
                            {
                                HomeController.jsonMessages.Remove(message);                              
                            }
                            outputMessages = JsonConvert.SerializeObject(HomeController.jsonMessages, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                        }

                        foreach (var userr in applicationUsers)
                        {
                            if (userr.Id == id)
                            {
                                applicationUsers.Remove(userr);
                                break;
                            }
                        }
                        outputUsers = JsonConvert.SerializeObject(applicationUsers, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    }
                    System.IO.File.WriteAllText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\UsersDatabase.json", outputUsers);
                    System.IO.File.WriteAllText(@"E:\STEP\myhomework2017\Task_MessageRepo_withoutDb\Task_MessageRepo_withoutDb\MessagesDatabase.json", outputMessages);
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login");
        }
    }
}