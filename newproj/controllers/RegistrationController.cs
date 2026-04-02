using project.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Collections
{
    public class RegistrationController
    {
        private readonly ForeignCitizenCollection _citizens;
        private readonly List<string> _purposes;
        private readonly List<string> _citizenships;

        public RegistrationController()
        {
            _citizens = ForeignCitizenCollection.Instance;
            _purposes = new List<string>
            {
                "Трудовая деятельность",
                "Иная"
            };
            _citizenships = new List<string>
            {
                "РФ",
                "Азербайджан",
                "Армения",
                "Киргизия",
                "Молдова",
                "Украина",
                "Узбекистан",
                "Таджикистан",
                "Казахстан",
                "Китай",
                "Другое"
            };
        }

        public string EnterCitizen(Profile citizen)
        {
            return _citizens.EnterCitizen(citizen);
        }

        public List<string> GetListPurposes()
        {
            return _purposes;
        }

        public List<string> GetListCitizenships()
        {
            return _citizenships;
        }

        public void ChoosePurposeAndCitizenship(string login, string selectedPurpose, string selectedCitizenship)
        {
            _citizens.ChoosePurposeAndCitizenship(login, selectedPurpose, selectedCitizenship);
        }

        public void SetProfileProperty(string login, string propertyName, string value)
        {
            _citizens.SetProfileProperty(login, propertyName, value);
        }
    }
}
