using CustomerRelationshipManagment.DatabaseAccess;
using CustomerRelationshipManagment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static CustomerRelationshipManagment.PredefinedValues.Values;
using static CustomerRelationshipManagment.PredefinedValues.ViewPaths;
using static CustomerRelationshipManagment.PredefinedValues.ViewTextContents;

namespace CustomerRelationshipManagment.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private static int onPageOfExistingEntries = 1;
        private static int onPageOfDeletedEntries = 1;
        private static int numberOfPagesOfExistingEntries;
        private static int numberOfPagesOfDeletedEntries;
        private static bool ifTrueShowExistingOtherwiseShowDeleted = true;
        private DatabaseContext databaseContext;

        public UserController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
            numberOfPagesOfExistingEntries = 
                (int)(Math.Ceiling((float)databaseContext.Users.Where(user => user.IsDeleted == false).Count() / (float)NUMBER_OF_ELEMENTS_PER_PAGE));
            numberOfPagesOfDeletedEntries =
                (int)(Math.Ceiling((float)databaseContext.Users.Where(user => user.IsDeleted == true).Count() / (float)NUMBER_OF_ELEMENTS_PER_PAGE));
        }

        private void SetUpViewData()
        {
            ViewData["EditActionText"] = EDIT_ACTION_TEXT;
            ViewData["DeleteActionText"] = DELETE_ACTION_TEXT;
            ViewData["RecreateActionText"] = RECREATE_ACTION_TEXT;
            ViewData["CreateActionText"] = CREATE_ACTION_TEXT;
            ViewData["GoBackActionText"] = GO_HOME_TEXT;
            ViewData["NextPageText"] = NEXT_PAGE_TEXT;
            ViewData["PreviousPageText"] = PREVIOUS_PAGE_TEXT;
            ViewData["HomePath"] = HOME_PATH;
            ViewData["NumberOfIndexesToDisplay"] = NUMBER_OF_INDEXES_TO_DISPLAY;
        }

        public ActionResult Index()
        {
            SetUpViewData();

            DatabaseAccesser databaseAccesser = new DatabaseAccesser(databaseContext);
            DatabaseAccesser.ModelType typeUser = DatabaseAccesser.ModelType.User;

            IList<UserModel> users = new List<UserModel>();
            List<UserModel> auxiliaryUsersList = new List<UserModel>();
            List<Object> query = null;
            
            if (ifTrueShowExistingOtherwiseShowDeleted)
            {
                ViewData["RecreateActionText"] = "";
                ViewData["CurrentPage"] = onPageOfExistingEntries;
                ViewData["NumberOfPages"] = numberOfPagesOfExistingEntries;
                ViewData["DisplayOption"] = "Show deleted";


                query = databaseAccesser
                    .GetFromDatabase(
                            typeUser,
                            false,
                            NUMBER_OF_ELEMENTS_PER_PAGE * (onPageOfExistingEntries - 1),
                            NUMBER_OF_ELEMENTS_PER_PAGE
                        );
            }
            else
            {
                ViewData["DeleteActionText"] = "";
                ViewData["CurrentPage"] = onPageOfDeletedEntries;
                ViewData["NumberOfPages"] = numberOfPagesOfDeletedEntries;
                ViewData["DisplayOption"] = "Show existing";

                query = databaseAccesser
                    .GetFromDatabase(
                            typeUser,
                            true,
                            NUMBER_OF_ELEMENTS_PER_PAGE * (onPageOfDeletedEntries - 1),
                            NUMBER_OF_ELEMENTS_PER_PAGE
                        );
            }

            query
                .ForEach(userObj => users.Add((UserModel) userObj));

            foreach(UserModel user in users)
                user.Role = databaseContext.Roles.Find(user.RoleId).RoleName;

            return View(users);
        }

        public ActionResult Create()
        {
            UserModel newUser = new UserModel();
            return View(newUser);
        }

        [HttpPost]
        public ActionResult Create(UserModel newUser)
        {
            try
            {
                if (databaseContext.Users.Where(user => user.Login == newUser.Login).Count() != 0)
                    return View(FAILURE_PATH);

                newUser.PasswordMd5 =
                    BitConverter
                        .ToString(
                            new MD5CryptoServiceProvider()
                            .ComputeHash(new UTF8Encoding().GetBytes(newUser.PasswordMd5))
                        );

                newUser.RoleId = 1;

                databaseContext.Add<UserModel>(newUser);
                databaseContext.SaveChanges();

                return View(SUCCESS_PATH);
            }
            catch
            {
                return View(FAILURE_PATH);
            }
        }

        [Authorize(Roles = "2, 3")]
        public ActionResult Edit(int id)
        {
            UserModel queriedUser = databaseContext.Users.Find(id);
            
            return View(queriedUser);
        }

        [HttpPost]
        [Authorize(Roles = "2, 3")]
        public ActionResult Edit(UserModel editedUser)
        {
            if(editedUser.PasswordMd5 == null)
                return View(FAILURE_PATH);

            try
            {
                if (databaseContext.Roles.Where(role => role.Id == editedUser.RoleId).Count() == 0)
                    return View(FAILURE_PATH);

                editedUser.PasswordMd5 =
                    BitConverter
                    .ToString(
                        new MD5CryptoServiceProvider()
                        .ComputeHash(new UTF8Encoding().GetBytes(editedUser.PasswordMd5))
                    );

                databaseContext
                    .Users
                    .Find(editedUser.Id)
                    .Name = editedUser.Name;

                databaseContext
                    .Users
                    .Find(editedUser.Id)
                    .Surname = editedUser.Surname;

                databaseContext
                    .Users
                    .Find(editedUser.Id)
                    .DateOfBirth = editedUser.DateOfBirth;

                if(User.FindFirst(claim => claim.Type == System.Security.Claims.ClaimTypes.Role)?.Value == "3")
                {
                    databaseContext
                        .Users
                        .Find(editedUser.Id)
                        .Login = editedUser.Login;

                    databaseContext
                        .Users
                        .Find(editedUser.Id)
                        .PasswordMd5 = editedUser.PasswordMd5;
                }

                if (
                        User.FindFirst(claim => claim.Type == System.Security.Claims.ClaimTypes.Role)?.Value == "3" &&
                        databaseContext.Roles.Find(editedUser.RoleId) != null
                   )
                        databaseContext
                            .Users
                            .Find(editedUser.Id)
                            .RoleId = editedUser.RoleId;

                databaseContext.SaveChanges();

                return View(SUCCESS_PATH);
            }
            catch
            {
                return View(FAILURE_PATH);
            }
        }

        [Authorize(Roles = "3")]
        public ActionResult Delete(int id)
        {
            UserModel queriedUser = databaseContext.Users.Find(id);
            
            return View(queriedUser);
        }

        [HttpPost]
        [Authorize(Roles = "3")]
        public ActionResult Delete(UserModel deletedUser)
        {
            try
            {
                databaseContext
                    .Users
                    .Find(deletedUser.Id)
                    .IsDeleted = true;
                databaseContext.SaveChanges();

                return View(SUCCESS_PATH);
            }
            catch
            {
                return View(FAILURE_PATH);
            }
        }

        [Authorize(Roles = "3")]
        public ActionResult Recreate(int id)
        {
            UserModel queriedUser = databaseContext.Users.Find(id);

            return View(queriedUser);
        }

        [HttpPost]
        [Authorize(Roles = "3")]
        public ActionResult Recreate(UserModel recreatedCompany)
        {
            try
            {
                databaseContext
                    .Users
                    .Find(recreatedCompany.Id)
                    .IsDeleted = false;
                databaseContext.SaveChanges();

                return View(SUCCESS_PATH);
            }
            catch
            {
                return View(FAILURE_PATH);
            }
        }

        [Authorize(Roles = "2, 3")]
        public ActionResult SwitchShowOption()
        {
            ifTrueShowExistingOtherwiseShowDeleted = !ifTrueShowExistingOtherwiseShowDeleted;
            return RedirectToAction("Index");
        }

        public ActionResult ChangePage(int id)
        {
            if(ifTrueShowExistingOtherwiseShowDeleted && id > 0 && id <= numberOfPagesOfExistingEntries)
            {
                onPageOfExistingEntries = id;
            }

            if(!ifTrueShowExistingOtherwiseShowDeleted && id > 0 && id <= numberOfPagesOfExistingEntries)
            {
                onPageOfDeletedEntries = id;
            }

            return RedirectToAction("Index");
        }
        
        public ActionResult NextPage()
        {
            if(ifTrueShowExistingOtherwiseShowDeleted && onPageOfExistingEntries < numberOfPagesOfExistingEntries)
            {
                onPageOfExistingEntries ++;
            }

            if (!ifTrueShowExistingOtherwiseShowDeleted && onPageOfDeletedEntries < numberOfPagesOfDeletedEntries)
            {
                onPageOfDeletedEntries++;
            }

            return RedirectToAction("Index");
        }

        public ActionResult PreviousPage()
        {
            if (ifTrueShowExistingOtherwiseShowDeleted && onPageOfExistingEntries > 1)
            {
                onPageOfExistingEntries --;
            }

            if (!ifTrueShowExistingOtherwiseShowDeleted && onPageOfDeletedEntries > 1)
            {
                onPageOfDeletedEntries--;
            }

            return RedirectToAction("Index");
        }
    }
}
