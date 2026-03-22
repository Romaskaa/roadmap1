using project.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Collections
{
    public class CitizenshipCollection
    {
        private List<Citizenship> _citizenships;

        public CitizenshipCollection()
        {
            _citizenships = new List<Citizenship>
            {
                new Citizenship("РФ"),
                new Citizenship("Азербайджан"),
                new Citizenship("Армения"),
                new Citizenship("Киргизия"),
                new Citizenship("Молдова"),
                new Citizenship("Украина"),
                new Citizenship("Узбекистан"),
                new Citizenship("Таджикистан"),
                new Citizenship("Казахстан"),
                new Citizenship("Китай"),
                new Citizenship("Другое")
            };
        }

        public List<Citizenship> GetListCitizenships() => _citizenships;
    }
}
