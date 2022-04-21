using CustomerRelationshipManagment.DatabaseAccess;
using CustomerRelationshipManagment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CustomerRelationshipManagment.PredefinedValues.Values;
using static CustomerRelationshipManagment.PredefinedValues.ViewPaths;
using static CustomerRelationshipManagment.PredefinedValues.ViewTextContents;

namespace CustomerRelationshipManagment.Controllers
{
    public class TradeNoteController : Controller
    {
        private static int onPageOfExistingEntries = 1;
        private static int onPageOfDeletedEntries = 1;
        private static int numberOfPagesOfExistingEntries;
        private static int numberOfPagesOfDeletedEntries;
        private static bool ifTrueShowExistingOtherwiseShowDeleted = true;
        private DatabaseContext databaseContext;

        public TradeNoteController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
            numberOfPagesOfExistingEntries =
                (int)(Math.Ceiling((float)databaseContext.TradeNotes.Where(tradeNote => tradeNote.IsDeleted == false).Count() / (float)NUMBER_OF_ELEMENTS_PER_PAGE));
            numberOfPagesOfDeletedEntries =
                (int)(Math.Ceiling((float)databaseContext.TradeNotes.Where(tradeNote => tradeNote.IsDeleted == true).Count() / (float)NUMBER_OF_ELEMENTS_PER_PAGE));
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
            DatabaseAccesser.ModelType queriedType = DatabaseAccesser.ModelType.TradeNote;

            IList<TradeNoteModel> tradeNotes = new List<TradeNoteModel>();
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
                .ForEach(tradeNoteObj => tradeNotes.Add((TradeNoteModel)tradeNoteObj));

            foreach(TradeNoteModel tradeNote in tradeNotes)
            {
                tradeNote.AssociatedCompany = databaseContext.Companies.Find(tradeNote.AssociatedCompanyId).Name;
                tradeNote.Creator = databaseContext.Users.Find(tradeNote.CreatorId).Login;
            }

            return View(tradeNotes);
        }

        public ActionResult Create()
        {
            TradeNoteModel newTradeNote = new TradeNoteModel();
            return View(newTradeNote);
        }

        [HttpPost]
        public ActionResult Create(TradeNoteModel newTradeNote)
        {
            try
            {
                if (newTradeNote.Contents.Contains("\n"))
                    return View(FAILURE_PATH);

                if (databaseContext.Companies.Where(company => company.Id == newTradeNote.AssociatedCompanyId).Count() == 0)
                    return View(FAILURE_PATH);

                string creatorLogin = (User.FindFirst(claim => claim.Type == "Login")).Value;
                int creatorId = databaseContext
                        .Users
                        .Where(user => user.Login == creatorLogin)
                        .First()
                        .Id;

                newTradeNote.CreatorId = creatorId;

                databaseContext.Add<TradeNoteModel>(newTradeNote);
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
            TradeNoteModel queriedTradeNote = databaseContext.TradeNotes.Find(id);

            return View(queriedTradeNote);
        }

        [HttpPost]
        [Authorize(Roles = "2, 3")]
        public ActionResult Edit(TradeNoteModel editedTradeNote)
        {
            try
            {
                if (editedTradeNote.Contents.Contains("\n"))
                    return View(FAILURE_PATH);

                if (databaseContext.Companies.Where(company => company.Id == editedTradeNote.AssociatedCompanyId).Count() == 0)
                    return View(FAILURE_PATH);

                databaseContext
                    .TradeNotes
                    .Find(editedTradeNote.Id)
                    .AssociatedCompanyId = editedTradeNote.AssociatedCompanyId;

                databaseContext
                    .TradeNotes
                    .Find(editedTradeNote.Id)
                    .Contents = editedTradeNote.Contents;

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
            TradeNoteModel queriedTradeNote = databaseContext.TradeNotes.Find(id);

            return View(queriedTradeNote);
        }

        [HttpPost]
        public ActionResult Delete(TradeNoteModel deletedTradeNote)
        {
            try
            {
                databaseContext
                    .TradeNotes
                    .Find(deletedTradeNote.Id)
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
            TradeNoteModel queriedTradeNote = databaseContext.TradeNotes.Find(id);

            return View(queriedTradeNote);
        }

        [HttpPost]
        [Authorize(Roles = "2, 3")]
        public ActionResult Recreate(TradeNoteModel recreatedTradeNote)
        {
            try
            {
                databaseContext
                    .TradeNotes
                    .Find(recreatedTradeNote.Id)
                    .IsDeleted = false;
                databaseContext.SaveChanges();

                return View(SUCCESS_PATH);
            }
            catch
            {
                return View(FAILURE_PATH);
            }
        }

        public ActionResult Details(int id)
        {
            TradeNoteModel tradeNote = databaseContext.TradeNotes.Find(id);

            return View(tradeNote);
        }

        [Authorize(Roles = "2, 3")]
        public ActionResult SwitchShowOption()
        {
            ifTrueShowExistingOtherwiseShowDeleted = !ifTrueShowExistingOtherwiseShowDeleted;
            return RedirectToAction("Index");
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

            return RedirectToAction("Index");
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

            return RedirectToAction("Index");
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

            return RedirectToAction("Index");
        }
    }
}