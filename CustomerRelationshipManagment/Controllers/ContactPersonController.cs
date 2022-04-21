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
    public class ContactPersonController : Controller
    {
        private static int onPageOfExistingEntries = 1;
        private static int onPageOfDeletedEntries = 1;
        private static int numberOfPagesOfExistingEntries;
        private static int numberOfPagesOfDeletedEntries;
        private static bool ifTrueShowExistingOtherwiseShowDeleted = true;
        private static bool ifTrueThenIsSearchingOtherwiseNot = false;
        private static string searchSurname;
        private DatabaseContext databaseContext;

        public ContactPersonController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
            numberOfPagesOfExistingEntries =
                (int)(Math.Ceiling((float)databaseContext.ContactPeople.Where(contactPerson => contactPerson.IsDeleted == false).Count() / (float)NUMBER_OF_ELEMENTS_PER_PAGE));
            numberOfPagesOfDeletedEntries =
                (int)(Math.Ceiling((float)databaseContext.ContactPeople.Where(contactPerson => contactPerson.IsDeleted == true).Count() / (float)NUMBER_OF_ELEMENTS_PER_PAGE));
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
            ifTrueThenIsSearchingOtherwiseNot = false;

            SetUpViewData();

            DatabaseAccesser databaseAccesser = new DatabaseAccesser(databaseContext);
            DatabaseAccesser.ModelType queriedType = DatabaseAccesser.ModelType.ContactPerson;

            IList<ContactPersonModel> contactPersons = new List<ContactPersonModel>();
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
                .ForEach(contactPersonObj => contactPersons.Add((ContactPersonModel)contactPersonObj));

            foreach (ContactPersonModel contactPerson in contactPersons)
            {
                contactPerson.AssociatedCompany = databaseContext.Companies.Find(contactPerson.AssociatedCompanyId).Name;
                contactPerson.Inviter = databaseContext.Users.Find(contactPerson.InviterId).Login;
            }

            return View(contactPersons);
        }

        public ActionResult Search(string surnameLike)
        {
            ifTrueThenIsSearchingOtherwiseNot = true;

            SetUpViewData();

            IList<ContactPersonModel> contactPeople = new List<ContactPersonModel>();

            if (surnameLike == null)
                surnameLike = "";

            searchSurname = surnameLike;

            if (ifTrueShowExistingOtherwiseShowDeleted)
            {
                int numberOfPages = (int)Math.Ceiling
                    (
                        (float)
                        databaseContext.ContactPeople
                            .Where(
                                contactPerson =>
                                contactPerson.IsDeleted == false &&
                                contactPerson.Surname.Contains(surnameLike)
                            ).Count() / (float)NUMBER_OF_ELEMENTS_PER_PAGE
                    );

                if (numberOfPages != 0 && numberOfPages < onPageOfExistingEntries)
                    onPageOfExistingEntries = numberOfPages;

                ViewData["RecreateActionText"] = "";
                ViewData["CurrentPage"] = onPageOfExistingEntries;
                ViewData["DisplayOption"] = "Show deleted";
                ViewData["NumberOfPages"] = numberOfPages;


                var query = databaseContext.ContactPeople
                    .Where(
                        contactPerson =>
                        contactPerson.IsDeleted == false &&
                        contactPerson.Surname.Contains(surnameLike)
                    ).Skip(NUMBER_OF_ELEMENTS_PER_PAGE * (onPageOfExistingEntries - 1))
                    .Take(NUMBER_OF_ELEMENTS_PER_PAGE);

                contactPeople = query.ToList();
            }
            else
            {
                int numberOfPages = (int)Math.Ceiling
                    (
                        (float)databaseContext.ContactPeople
                            .Where(
                                contactPerson =>
                                contactPerson.IsDeleted == true &&
                                contactPerson.Surname.Contains(surnameLike)
                            ).Count() / (float)NUMBER_OF_ELEMENTS_PER_PAGE
                    );

                if (numberOfPages != 0 && numberOfPages < onPageOfDeletedEntries)
                    onPageOfDeletedEntries = numberOfPages;

                ViewData["DeleteActionText"] = "";
                ViewData["CurrentPage"] = onPageOfDeletedEntries;
                ViewData["DisplayOption"] = "Show existing";
                ViewData["NumberOfPages"] = numberOfPages;

                var query = databaseContext.ContactPeople
                    .Where(
                        contactPerson =>
                        contactPerson.IsDeleted == true &&
                        contactPerson.Surname.Contains(surnameLike)
                    ).Skip(NUMBER_OF_ELEMENTS_PER_PAGE * (onPageOfExistingEntries - 1))
                    .Take(NUMBER_OF_ELEMENTS_PER_PAGE);

                contactPeople = query.ToList();
            }

            foreach (ContactPersonModel contactPerson in contactPeople)
            {
                contactPerson.AssociatedCompany = databaseContext.Companies.Find(contactPerson.AssociatedCompanyId).Name;
                contactPerson.Inviter = databaseContext.Users.Find(contactPerson.InviterId).Login;
            }

            return View(contactPeople);
        }

        public ActionResult Create()
        {
            ContactPersonModel newContactPerson = new ContactPersonModel();
            return View(newContactPerson);
        }

        [HttpPost]
        public ActionResult Create(ContactPersonModel newContactPerson)
        {
            try
            {
                if (databaseContext.Companies.Where(company => company.Id == newContactPerson.AssociatedCompanyId).Count() == 0)
                    return View(FAILURE_PATH);

                string creatorLogin = (User.FindFirst(claim => claim.Type == "Login")).Value;
                int inviterId = databaseContext
                        .Users
                        .Where(user => user.Login == creatorLogin)
                        .First()
                        .Id;

                newContactPerson.InviterId = inviterId;

                databaseContext.Add<ContactPersonModel>(newContactPerson);
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
            ContactPersonModel queriedContactPerson = databaseContext.ContactPeople.Find(id);

            return View(queriedContactPerson);
        }

        [HttpPost]
        [Authorize(Roles = "2, 3")]
        public ActionResult Edit(ContactPersonModel editedContactPerson)
        {
            try
            {
                if (databaseContext.Companies.Where(company => company.Id == editedContactPerson.AssociatedCompanyId).Count() == 0)
                    return View(FAILURE_PATH);

                databaseContext
                    .ContactPeople
                    .Find(editedContactPerson.Id)
                    .AssociatedCompanyId = editedContactPerson.AssociatedCompanyId;

                databaseContext
                    .ContactPeople
                    .Find(editedContactPerson.Id)
                    .EmailAddress = editedContactPerson.EmailAddress;

                databaseContext
                    .ContactPeople
                    .Find(editedContactPerson.Id)
                    .Name = editedContactPerson.Name;

                databaseContext
                    .ContactPeople
                    .Find(editedContactPerson.Id)
                    .Surname = editedContactPerson.Surname;

                databaseContext
                    .ContactPeople
                    .Find(editedContactPerson.Id)
                    .PhoneNumber = editedContactPerson.PhoneNumber;

                databaseContext
                    .ContactPeople
                    .Find(editedContactPerson.Id)
                    .Position = editedContactPerson.Position;

                databaseContext.SaveChanges();

                return View(SUCCESS_PATH);
            }
            catch
            {
                return View(FAILURE_PATH);
            }
        }

        public ActionResult Delete(int id)
        {
            ContactPersonModel queriedContactPerson = databaseContext.ContactPeople.Find(id);

            return View(queriedContactPerson);
        }

        [HttpPost]
        public ActionResult Delete(ContactPersonModel deletedContactPerson)
        {
            try
            {
                databaseContext
                    .ContactPeople
                    .Find(deletedContactPerson.Id)
                    .IsDeleted = true;
                databaseContext.SaveChanges();

                return View(SUCCESS_PATH);
            }
            catch
            {
                return View(FAILURE_PATH);
            }
        }

        [Authorize(Roles = "2, 3")]
        public ActionResult Recreate(int id)
        {
            ContactPersonModel queriedContactPerson = databaseContext.ContactPeople.Find(id);

            return View(queriedContactPerson);
        }

        [HttpPost]
        [Authorize(Roles = "2, 3")]
        public ActionResult Recreate(ContactPersonModel recreatedContactPerson)
        {
            try
            {
                databaseContext
                    .ContactPeople
                    .Find(recreatedContactPerson.Id)
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

            if (!ifTrueThenIsSearchingOtherwiseNot)
                return RedirectToAction("Index");

            return RedirectToAction("Search",
                new
                {
                    surnameLike = searchSurname
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

            if (!ifTrueThenIsSearchingOtherwiseNot)
                return RedirectToAction("Index");

            return RedirectToAction("Search",
                new
                {
                    surnameLike = searchSurname
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

            if (!ifTrueThenIsSearchingOtherwiseNot)
                return RedirectToAction("Index");

            return RedirectToAction("Search",
                new
                {
                    surnameLike = searchSurname
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

            if (!ifTrueThenIsSearchingOtherwiseNot)
                return RedirectToAction("Index");

            return RedirectToAction("Search",
                new
                {
                    surnameLike = searchSurname
                });
        }
    }
}
