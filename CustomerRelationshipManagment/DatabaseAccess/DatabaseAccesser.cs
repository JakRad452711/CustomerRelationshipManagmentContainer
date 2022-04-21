using CustomerRelationshipManagment.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomerRelationshipManagment.DatabaseAccess
{
    public class DatabaseAccesser
    {
        private DatabaseContext databaseContext;
        public enum ModelType
        {
            User,
            Company,
            TradeNote,
            ContactPerson,
            Role,
            Industry
        }

        public DatabaseAccesser(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        /// <summary>
        /// Created specifically for this CRM project.
        /// Returns a list from a table using a DatabaseAccess instance.
        /// Returned entry type is dependant on value passed in 'queriedModelType' argument.
        /// 'queriedModelType' values are avaialable in DatabaseAccesser.ModelType field.
        /// </summary>
        /// <param name="queriedModelType"></param>
        /// <param name="ifIsDeleted"></param>
        /// <param name="numberOfSkippedObjects"></param>
        /// <param name="maxNumberOfReturnedObjects"></param>
        /// <returns>
        /// A List of type Object casted from a List of type:
        /// {UserModel or CompanyModel or TradeNoteModel or ContactPersonModel or RoleModel or IndustryModel}
        /// </returns>
        public List<Object> GetFromDatabase(ModelType queriedModelType, bool ifIsDeleted, int numberOfSkippedObjects, int maxNumberOfReturnedObjects)
        {
            List<Object> outputList = new List<object>();

            switch (queriedModelType)
            {
                case ModelType.User:
                    {
                        databaseContext.Users
                            .Where(user => user.IsDeleted == ifIsDeleted)
                            .OrderBy(user => user.Id)
                            .Skip(numberOfSkippedObjects)
                            .Take(maxNumberOfReturnedObjects)
                            .ToList<UserModel>()
                            .ForEach(user => outputList.Add((Object) user));
                    } break;
                case ModelType.Company:
                    {
                        databaseContext.Companies
                            .Where(company => company.IsDeleted == ifIsDeleted)
                            .OrderBy(company => company.Id)
                            .Skip(numberOfSkippedObjects)
                            .Take(maxNumberOfReturnedObjects)
                            .ToList<CompanyModel>()
                            .ForEach(company => outputList.Add((Object) company));
                    } break;
                case ModelType.TradeNote:
                    {
                        databaseContext.TradeNotes
                            .Where(tradeNote => tradeNote.IsDeleted == ifIsDeleted)
                            .OrderBy(tradeNote => tradeNote.Id)
                            .Skip(numberOfSkippedObjects)
                            .Take(maxNumberOfReturnedObjects)
                            .ToList<TradeNoteModel>()
                            .ForEach(tradeNote => outputList.Add((Object) tradeNote));
                    } break;
                case ModelType.ContactPerson:
                    {
                        databaseContext.ContactPeople
                            .Where(contactPerson => contactPerson.IsDeleted == ifIsDeleted)
                            .OrderBy(contactPerson => contactPerson.Id)
                            .Skip(numberOfSkippedObjects)
                            .Take(maxNumberOfReturnedObjects)
                            .ToList<ContactPersonModel>()
                            .ForEach(contactPerson => outputList.Add((Object) contactPerson));
                    } break;
                case ModelType.Role:
                    {
                        databaseContext.Roles
                            .OrderBy(role => role.Id)
                            .Skip(numberOfSkippedObjects)
                            .Take(maxNumberOfReturnedObjects)
                            .ToList<RoleModel>()
                            .ForEach(role => outputList.Add((Object) role));
                    } break;
                case ModelType.Industry:
                    {
                        databaseContext.Industries
                            .OrderBy(industry => industry.Id)
                            .Skip(numberOfSkippedObjects)
                            .Take(maxNumberOfReturnedObjects)
                            .ToList<IndustryModel>()
                            .ForEach(industry => outputList.Add((Object) industry));
                    } break;
            }

            return outputList;
        }
    }
}
