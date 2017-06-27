using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{
    public class UserHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static string GetDisplayNameFromId(string Id)
        {
            return db.Users.Find(Id).DisplayName;
        }


    }
}