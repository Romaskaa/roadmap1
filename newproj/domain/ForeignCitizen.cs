using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Controllers
{
    public class ForeignCitizen
    {
        public string Name { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public string Login { get; set; }
        public Purpose Purpose { get; set; }
        public Citizenship Citizenship { get; set; }

        public ForeignCitizen(string login, string name, DateTime entryDate, DateTime? applicationDate)
        {
            Login = login;
            Name = name;
            EntryDate = entryDate;
            ApplicationDate = applicationDate;
        }

        public ForeignCitizen() { }
    }
}