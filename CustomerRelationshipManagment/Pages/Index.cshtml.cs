using CustomerRelationshipManagment.Controllers;
using CustomerRelationshipManagment.DatabaseAccess;
using CustomerRelationshipManagment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerRelationshipManagment.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DatabaseContext _db;

        public IndexModel(ILogger<IndexModel> logger, DatabaseContext db)
        {
            _logger = logger;
            _db = db;
        }

        public void OnGet()
        {

        }
    }
}
