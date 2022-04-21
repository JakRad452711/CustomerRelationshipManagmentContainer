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
    public class IndustryController : Controller
    {
        private static int onPageOfExistingEntries = 1;
        private static int numberOfPagesOfExistingEntries;
        private static readonly bool ifTrueShowExistingOtherwiseShowDeleted = true;
        private DatabaseContext databaseContext;

        public IndustryController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
            numberOfPagesOfExistingEntries =
                (int)(Math.Ceiling((float)databaseContext.Industries.Count() / (float)NUMBER_OF_ELEMENTS_PER_PAGE));
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
            DatabaseAccesser.ModelType queriedType = DatabaseAccesser.ModelType.Industry;

            IList<IndustryModel> industries = new List<IndustryModel>();
            List<Object> query = null;

            if(ifTrueShowExistingOtherwiseShowDeleted)
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
                .ForEach(industryObj => industries.Add((IndustryModel)industryObj));

            return View(industries);
        }

        [Authorize(Roles = "3")]
        public ActionResult Create()
        {
            IndustryModel newIndustry = new IndustryModel();
            return View(newIndustry);
        }

        [HttpPost]
        [Authorize(Roles = "3")]
        public ActionResult Create(IndustryModel newUser)
        {
            try
            {
                databaseContext.Add<IndustryModel>(newUser);
                databaseContext.SaveChanges();

                return View(SUCCESS_PATH);
            }
            catch
            {
                return View(FAILURE_PATH);
            }
        }

        [Authorize(Roles = "3")]
        public ActionResult Edit(int id)
        {
            IndustryModel queriedIndustry = databaseContext.Industries.Find(id);

            return View(queriedIndustry);
        }

        [HttpPost]
        [Authorize(Roles = "3")]
        public ActionResult Edit(IndustryModel editedIndustry)
        {
            try
            {
                databaseContext
                    .Industries
                    .Find(editedIndustry.Id)
                    .Name = editedIndustry.Name;

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
