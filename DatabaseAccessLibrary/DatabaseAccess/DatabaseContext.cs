using DatabaseAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseAccessLibrary.DatabaseAccess
{
    public class DatabaseContext: DbContext
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
