using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerRelationshipManagment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CustomerRelationshipManagment.Views.Main
{
    public class TestModel : PageModel
    {
        public List<UserModel> Users;

        public void OnGet()
        {
        }
    }
}
