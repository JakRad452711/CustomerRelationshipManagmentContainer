using CustomerRelationshipManagment.DatabaseAccess;
using CustomerRelationshipManagment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using static CustomerRelationshipManagment.PredefinedValues.Values;
using static CustomerRelationshipManagment.PredefinedValues.ViewPaths;
using static CustomerRelationshipManagment.PredefinedValues.ViewTextContents;

namespace CustomerRelationshipManagment.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private static int onPageOfExistingEntries = 1;
        private static int numberOfPagesOfExistingEntries;
        private static readonly bool ifTrueShowExistingOtherwiseShowDeleted = true;
        private DatabaseContext databaseContext;

        public RoleController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
            numberOfPagesOfExistingEntries =
                (int)(Math.Ceiling((float)databaseContext.Roles.Count() / (float)NUMBER_OF_ELEMENTS_PER_PAGE));
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
            DatabaseAccesser.ModelType queriedType = DatabaseAccesser.ModelType.Role;

            IList<RoleModel> roles = new List<RoleModel>();
            List<Object> query = null;

            if (ifTrueShowExistingOtherwiseShowDeleted)
            {
                ViewData["RecreateActionText"] = "";
                ViewData["CurrentPage"] = onPageOfExistingEntries;
                ViewData["NumberOfPages"] = numberOfPagesOfExistingEntries;
                ViewData["DisplayOption"] = "";


                query = databaseAccesser
                    .GetFromDatabase(
                            queriedType,
                            false,
                            NUMBER_OF_ELEMENTS_PER_PAGE * (onPageOfExistingEntries - 1),
                            NUMBER_OF_ELEMENTS_PER_PAGE
                        );
            }

            query
                .ForEach(roleObj => roles.Add((RoleModel)roleObj));

            return View(roles);
        }

        [Authorize(Roles = "3")]
        public ActionResult Edit(int id)
        {
            RoleModel queriedRole = databaseContext.Roles.Find(id);

            return View(queriedRole);
        }

        [HttpPost]
        [Authorize(Roles = "3")]
        public ActionResult Edit(RoleModel editedRole)
        {
            try
            {
                databaseContext
                    .Roles
                    .Find(editedRole.Id)
                    .RoleName = editedRole.RoleName;

                databaseContext.SaveChanges();

                return View(SUCCESS_PATH);
            }
            catch
            {
                return View(FAILURE_PATH);
            }
        }

        public ActionResult ChangePage(int id)
        {
            if (ifTrueShowExistingOtherwiseShowDeleted && id > 0 && id <= numberOfPagesOfExistingEntries)
            {
                onPageOfExistingEntries = id;
            }

            return RedirectToAction("Index");
        }

        public ActionResult NextPage()
        {
            if (ifTrueShowExistingOtherwiseShowDeleted && onPageOfExistingEntries < numberOfPagesOfExistingEntries)
            {
                onPageOfExistingEntries++;
            }

            return RedirectToAction("Index");
        }

        public ActionResult PreviousPage()
        {
            if (ifTrueShowExistingOtherwiseShowDeleted && onPageOfExistingEntries > 1)
            {
                onPageOfExistingEntries--;
            }

            return RedirectToAction("Index");
        }
    }
}
