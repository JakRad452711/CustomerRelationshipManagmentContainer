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
    public class CompanyController : Controller
    {
        private static int onPageOfExistingEntries = 1;
        private static int onPageOfDeletedEntries = 1;
        private static int numberOfPagesOfExistingEntries;
        private static int numberOfPagesOfDeletedEntries;
        private static bool ifTrueShowExistingOtherwiseShowDeleted = true;
        private static bool ifTrueThenIsFilteringOtherwiseNot = false;
        private static int?[] lastFilterValuesIndustryId = new int?[2];
        private static DateTime?[] lastFilterValuesWhenAdded = new DateTime?[2];
        private DatabaseContext databaseContext;

        public CompanyController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
            numberOfPagesOfExistingEntries =
                (int)(Math.Ceiling((float)databaseContext.Companies.Where(company => company.IsDeleted == false).Count() / (float)NUMBER_OF_ELEMENTS_PER_PAGE));
            numberOfPagesOfDeletedEntries =
                (int)(Math.Ceiling((float)databaseContext.Companies.Where(company => company.IsDeleted == true).Count() / (float)NUMBER_OF_ELEMENTS_PER_PAGE));
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
            ifTrueThenIsFilteringOtherwiseNot = false;
            SetUpViewData();

            DatabaseAccesser databaseAccesser = new DatabaseAccesser(databaseContext);
            DatabaseAccesser.ModelType queriedType = DatabaseAccesser.ModelType.Company;

            IList<CompanyModel> companies = new List<CompanyModel>();
            List<Object> query = null;

            if (ifTrueShowExistingOtherwiseShowDeleted)
            {
                ViewData["RecreateActionText"] = "";
                ViewData["CurrentPage"] = onPageOfExistingEntries;
                ViewData["NumberOfPages"] = numberOfPagesOfExistingEntries;
                ViewData["DisplayOption"] = "Show deleted";


                query = databaseAccesser
                    .GetFromDatabase(
                            queriedType,
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
                            queriedType,
                            true,
                            NUMBER_OF_ELEMENTS_PER_PAGE * (onPageOfDeletedEntries - 1),
                            NUMBER_OF_ELEMENTS_PER_PAGE
                        );
            }

            query
                .ForEach(companyObj => companies.Add((CompanyModel) companyObj));

            foreach(CompanyModel company in companies)
            {
                company.Creator = databaseContext.Users.Find(company.CreatorId).Login;
                company.Industry = databaseContext.Industries.Find(company.IndustryId).Name;
            }

            return View(companies);
        }

        public ActionResult Filter(int? industryIdFrom, int? industryIdTo, DateTime? whenAddedFrom, DateTime? whenAddedTo)
        {
            ifTrueThenIsFilteringOtherwiseNot = true;

            SetUpViewData();

            IList<CompanyModel> companies = new List<CompanyModel>();

            if (industryIdFrom == null)
                industryIdFrom = 1;

            if (industryIdTo == null)
                industryIdTo = databaseContext.Industries.Max(industry => industry.Id);

            if (whenAddedTo == null)
                whenAddedTo = DateTime.Now;

            if (whenAddedFrom == null)
                whenAddedFrom = new DateTime(1, 1, 1);

            lastFilterValuesIndustryId[0] = industryIdFrom;
            lastFilterValuesIndustryId[1] = industryIdTo;
            lastFilterValuesWhenAdded[0] = whenAddedFrom;
            lastFilterValuesWhenAdded[1] = whenAddedTo;

            if (ifTrueShowExistingOtherwiseShowDeleted)
            {
                int numberOfPages = (int) Math.Ceiling
                    (
                        (float) databaseContext.Companies
                            .Where(
                                company =>
                                company.IndustryId >= industryIdFrom &&
                                company.IndustryId <= industryIdTo &&
                                company.WhenAdded >= whenAddedFrom &&
                                company.WhenAdded <= whenAddedTo &&
                                company.IsDeleted == false
                            ).Count() / (float) NUMBER_OF_ELEMENTS_PER_PAGE
                    );

                if (numberOfPages != 0 && numberOfPages < onPageOfExistingEntries)
                    onPageOfExistingEntries = numberOfPages;

                ViewData["RecreateActionText"] = "";
                ViewData["CurrentPage"] = onPageOfExistingEntries;
                ViewData["DisplayOption"] = "Show deleted";
                ViewData["NumberOfPages"] = numberOfPages;


                var query = databaseContext.Companies
                    .Where(
                        company =>
                        company.IndustryId >= industryIdFrom &&
                        company.IndustryId <= industryIdTo &&
                        company.WhenAdded >= whenAddedFrom &&
                        company.WhenAdded <= whenAddedTo &&
                        company.IsDeleted == false
                    ).OrderBy(company => company.Id)
                    .Skip(NUMBER_OF_ELEMENTS_PER_PAGE * (onPageOfExistingEntries - 1))
                    .Take(NUMBER_OF_ELEMENTS_PER_PAGE);

                companies = query.ToList();
            }
            else
            {
                int numberOfPages = (int)Math.Ceiling
                    (
                        (float)databaseContext.Companies
                            .Where(
                                company =>
                                company.IndustryId >= industryIdFrom &&
                                company.IndustryId <= industryIdTo &&
                                company.WhenAdded >= whenAddedFrom &&
                                company.WhenAdded <= whenAddedTo &&
                                company.IsDeleted == true
                            ).Count() / (float)NUMBER_OF_ELEMENTS_PER_PAGE
                    );

                if (numberOfPages != 0 && numberOfPages < onPageOfDeletedEntries)
                    onPageOfDeletedEntries = numberOfPages;

                ViewData["DeleteActionText"] = "";
                ViewData["CurrentPage"] = onPageOfDeletedEntries;
                ViewData["DisplayOption"] = "Show existing";
                ViewData["NumberOfPages"] = numberOfPages;

                var query = databaseContext.Companies
                    .Where(
                        company =>
                        company.IndustryId >= industryIdFrom &&
                        company.IndustryId <= industryIdTo &&
                        company.WhenAdded >= whenAddedFrom &&
                        company.WhenAdded <= whenAddedTo &&
                        company.IsDeleted == true
                    ).OrderBy(company => company.Id)
                    .Skip(NUMBER_OF_ELEMENTS_PER_PAGE * (onPageOfExistingEntries - 1))
                    .Take(NUMBER_OF_ELEMENTS_PER_PAGE);

                companies = query.ToList();
            }

            foreach (CompanyModel company in companies)
            {
                company.Creator = databaseContext.Users.Find(company.CreatorId).Login;
                company.Industry = databaseContext.Industries.Find(company.IndustryId).Name;
            }

            return View(companies);
        }

        public ActionResult Create()
        {
            CompanyModel newCompany = new CompanyModel();
            return View(newCompany);
        }

        [HttpPost]
        public ActionResult Create(CompanyModel newCompany)
        {
            try
            {
                if (databaseContext.Industries.Where(industry => industry.Id == newCompany.IndustryId).Count() == 0)
                    return View(FAILURE_PATH);

                string creatorLogin = (User.FindFirst(claim => claim.Type == "Login")).Value;
                int creatorId = databaseContext
                        .Users
                        .Where(user => user.Login == creatorLogin)
                        .First()
                        .Id;

                newCompany.CreatorId = creatorId;
                newCompany.WhenAdded = DateTime.Now;

                databaseContext.Add<CompanyModel>(newCompany);
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
            CompanyModel queriedCompany = databaseContext.Companies.Find(id);

            return View(queriedCompany);
        }

        [HttpPost]
        [Authorize(Roles = "3")]
        public ActionResult Edit(CompanyModel editedCompany)
        {
            try
            {
                if (databaseContext.Industries.Where(industry => industry.Id == editedCompany.IndustryId).Count() == 0)
                    return View(FAILURE_PATH);

                databaseContext
                    .Companies
                    .Find(editedCompany.Id)
                    .Name = editedCompany.Name;

                databaseContext
                    .Companies
                    .Find(editedCompany.Id)
                    .Nip = editedCompany.Nip;

                databaseContext
                    .Companies
                    .Find(editedCompany.Id)
                    .IndustryId = editedCompany.IndustryId;

                databaseContext
                    .Companies
                    .Find(editedCompany.Id)
                    .Address = editedCompany.Address;

                databaseContext
                    .Companies
                    .Find(editedCompany.Id)
                    .City = editedCompany.City;

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
            CompanyModel queriedCompany = databaseContext.Companies.Find(id);

            return View(queriedCompany);
        }

        [HttpPost]
        [Authorize(Roles = "3")]
        public ActionResult Delete(CompanyModel deletedCompany)
        {
            try
            {
                databaseContext
                    .Companies
                    .Find(deletedCompany.Id)
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
            CompanyModel queriedCompany = databaseContext.Companies.Find(id);

            return View(queriedCompany);
        }

        [HttpPost]
        [Authorize(Roles = "3")]
        public ActionResult Recreate(CompanyModel recreatedCompany)
        {
            try
            {
                databaseContext
                    .Companies
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

            if (!ifTrueThenIsFilteringOtherwiseNot)
                return RedirectToAction("Index");
            
            return RedirectToAction("Filter", 
                new { 
                    industryIdFrom = lastFilterValuesIndustryId[0], 
                    whenAddedFrom = lastFilterValuesWhenAdded[0],
                    industryIdTo = lastFilterValuesIndustryId[1],
                    whenAddedTo = lastFilterValuesWhenAdded[1] 
                });
        }

        public ActionResult ChangePage(int id)
        {
            if (ifTrueShowExistingOtherwiseShowDeleted && id > 0 && id <= numberOfPagesOfExistingEntries)
            {
                onPageOfExistingEntries = id;
            }

            if (!ifTrueShowExistingOtherwiseShowDeleted && id > 0 && id <= numberOfPagesOfExistingEntries)
            {
                onPageOfDeletedEntries = id;
            }

            if (!ifTrueThenIsFilteringOtherwiseNot)
                return RedirectToAction("Index");

            return RedirectToAction("Filter",
                new
                {
                    industryIdFrom = lastFilterValuesIndustryId[0],
                    whenAddedFrom = lastFilterValuesWhenAdded[0],
                    industryIdTo = lastFilterValuesIndustryId[1],
                    whenAddedTo = lastFilterValuesWhenAdded[1]
                });
        }

        public ActionResult NextPage()
        {
            if (ifTrueShowExistingOtherwiseShowDeleted && onPageOfExistingEntries < numberOfPagesOfExistingEntries)
            {
                onPageOfExistingEntries++;
            }

            if (!ifTrueShowExistingOtherwiseShowDeleted && onPageOfDeletedEntries < numberOfPagesOfDeletedEntries)
            {
                onPageOfDeletedEntries++;
            }

            if(!ifTrueThenIsFilteringOtherwiseNot)
                return RedirectToAction("Index");

            return RedirectToAction("Filter",
                new
                {
                    industryIdFrom = lastFilterValuesIndustryId[0],
                    whenAddedFrom = lastFilterValuesWhenAdded[0],
                    industryIdTo = lastFilterValuesIndustryId[1],
                    whenAddedTo = lastFilterValuesWhenAdded[1]
                });
        }

        public ActionResult PreviousPage()
        {
            if (ifTrueShowExistingOtherwiseShowDeleted && onPageOfExistingEntries > 1)
            {
                onPageOfExistingEntries--;
            }

            if (!ifTrueShowExistingOtherwiseShowDeleted && onPageOfDeletedEntries > 1)
            {
                onPageOfDeletedEntries--;
            }

            if (!ifTrueThenIsFilteringOtherwiseNot)
                return RedirectToAction("Index");

            return RedirectToAction("Filter",
                new
                {
                    industryIdFrom = lastFilterValuesIndustryId[0],
                    whenAddedFrom = lastFilterValuesWhenAdded[0],
                    industryIdTo = lastFilterValuesIndustryId[1],
                    whenAddedTo = lastFilterValuesWhenAdded[1]
                });
        }
    }
}
