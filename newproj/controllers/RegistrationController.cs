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
        private readonly PurposeCollection _purposeCollection;
        private readonly CitizenshipCollection _citizenshipCollection;

        public RegistrationController()
        {
            _citizens = ForeignCitizenCollection.Instance;
            _purposeCollection = new PurposeCollection();
            _citizenshipCollection = new CitizenshipCollection();
        }

        public string EnterCitizen(string name, DateTime entryDate, DateTime? applicationDate)
        {
            return _citizens.EnterCitizen(name, entryDate, applicationDate);
        }

        public List<Purpose> GetListPurposes()
        {
            return _purposeCollection.GetListPurposes();
        }

        public List<Citizenship> GetListCitizenships()
        {
            return _citizenshipCollection.GetListCitizenships();
        }

        public void ChoosePurposeAndCitizenship(string login, Purpose selectedPurpose, Citizenship selectedCitizenship)
        {
            _citizens.ChoosePurposeAndCitizenship(login, selectedPurpose, selectedCitizenship);
        }
    }
}