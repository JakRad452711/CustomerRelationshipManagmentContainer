using CustomerRelationshipManagment.Authentication;
using CustomerRelationshipManagment.DatabaseAccess;
using CustomerRelationshipManagment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static CustomerRelationshipManagment.PredefinedValues.ViewPaths;

namespace CustomerRelationshipManagment.Controllers
{
    [Authorize]
    public class MainController : Controller
    {
        DatabaseContext databaseContext = null;

        public MainController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            if (Authenticator.OpenLogInTab(this) != null)
                return Authenticator.OpenLogInTab(this);

            return View();
        }

        [AllowAnonymous]
        public ActionResult LogIn()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(UserModel registeringUser)
        {
            try
            {
                Console.WriteLine(":(");
                if (databaseContext.Users.Where(user => user.Login == registeringUser.Login).Count() != 0)
                    return View(FAILURE_PATH);
                Console.WriteLine(":)");
                registeringUser.PasswordMd5 =
                    BitConverter
                        .ToString(
                            new MD5CryptoServiceProvider()
                            .ComputeHash(new UTF8Encoding().GetBytes(registeringUser.PasswordMd5))
                        );

                registeringUser.RoleId = databaseContext
                    .Roles
                    .Where(role => role.RoleName == "normal user")
                    .First()
                    .Id;

                databaseContext.Add<UserModel>(registeringUser);
                databaseContext.SaveChanges();

                if (!Authenticator.LogIn(this, databaseContext, registeringUser.Id, registeringUser.PasswordMd5))
                    return Redirect("/Main/LogIn");

                return View(SUCCESS_PATH);
            }
            catch
            {
                return View(FAILURE_PATH);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogIn(UserModel loggingInUser)
        {
            UserModel theUser = null;
            
            if (databaseContext.Users.Where(user => user.Login == loggingInUser.Login).Count() == 0) 
                return View(FAILURE_PATH);

            theUser = databaseContext.Users.Where(user => user.Login == loggingInUser.Login).First();
            
            if (!Authenticator.LogIn(this, databaseContext, theUser.Id, loggingInUser.PasswordMd5))
                return View(FAILURE_PATH);

            return Redirect("/");
        }

        public ActionResult LogOut()
        {
            return Authenticator.LogOut(this);
        }
    }
}
