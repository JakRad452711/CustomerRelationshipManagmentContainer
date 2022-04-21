using CustomerRelationshipManagment.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerRelationshipManagment.DatabaseAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() { }
        public DatabaseContext(DbContextOptions options) : base(options) { }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<CompanyModel> Companies { get; set; }
        public DbSet<TradeNoteModel> TradeNotes { get; set; }
        public DbSet<ContactPersonModel> ContactPeople { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<IndustryModel> Industries { get; set; }
    }
}
