using CustomerRelationshipManagment.DatabaseAccess;
using CustomerRelationshipManagment.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace CustomerRelationshipManagment.Authentication
{
    public static class Authenticator
    {
        public static ActionResult OpenLogInTab(Controller controller)
        {
            if (!controller.User.Identity.IsAuthenticated)
                return controller.RedirectToAction("LogIn", "Main");

            return null;
        }

        /// <summary>
        /// A method that checks if user entered good credentials.
        /// </summary>
        /// <param name="databaseContext"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns>
        /// On success: authentication token string. 
        /// On failure: (null)
        /// </returns>
        [HttpPost]
        public static bool LogIn(Controller controller, DatabaseContext databaseContext, int userId, string password)
        {
            UserModel authenticatingUser = new UserModel();

            if ((authenticatingUser = databaseContext.Users.Find(userId)) != null)
            {
                string PasswordMd5 =
                    BitConverter
                    .ToString(
                        new MD5CryptoServiceProvider()
                        .ComputeHash(new UTF8Encoding().GetBytes(password))
                    );

                if (authenticatingUser.PasswordMd5 == PasswordMd5)
                {
                    var identity = new ClaimsIdentity(
                        GetUserClaims(authenticatingUser),
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);
                    var login = controller.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return true;
                }
            }

            return false;
        }

        public static ActionResult LogOut(Controller controller)
        {
            controller.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return controller.RedirectToAction("LogIn", "Main");
        }

        private static Claim[] GetUserClaims(UserModel user)
        {
            Claim[] userClaims = new Claim[]
                {
                        new Claim("Login", user.Login),
                        new Claim(ClaimTypes.Name, user.Name + " " + user.Surname),
                        new Claim(ClaimTypes.Role, user.RoleId.ToString())
                };

            return userClaims;
        }
    }
}
