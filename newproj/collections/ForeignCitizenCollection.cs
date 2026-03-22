using project.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Collections
{
    public class ForeignCitizenCollection
    {
        private static ForeignCitizenCollection _instance;
        private List<ForeignCitizen> _citizens;
        private string FilePath = "citizens.txt";

        private ForeignCitizenCollection()
        {
            _citizens = new List<ForeignCitizen>();
            LoadFromFile();
        }

        public static ForeignCitizenCollection Instance
        {
            get
            {
                if (_instance == null) _instance = new ForeignCitizenCollection();
                return _instance;
            }
        }

        public string EnterCitizen(string name, DateTime entryDate, DateTime? applicationDate)
        {
            int nextId = 1;
            if (_citizens.Any())
            {
                int maxId = _citizens.Select(c => int.TryParse(c.Login, out int id) ? id : 0).Max();
                nextId = maxId + 1;
            }

            string login = nextId.ToString();
            var newCitizen = new ForeignCitizen(login, name, entryDate, applicationDate);
            _citizens.Add(newCitizen);

            SaveToFile();

            return login;
        }

        public ForeignCitizen GetCitizen(string login)
        {
            return _citizens.FirstOrDefault(c => c.Login == login);
        }

        public void ChoosePurposeAndCitizenship(string login, Purpose selectedPurpose, Citizenship selectedCitizenship)
        {
            var citizen = GetCitizen(login);
            if (citizen != null)
            {
                citizen.Purpose = selectedPurpose;
                citizen.Citizenship = selectedCitizenship;
                SaveToFile();
            }
        }

        private void SaveToFile()
        {
            using (StreamWriter sw = new StreamWriter(FilePath, false, Encoding.UTF8))
            {
                foreach (var c in _citizens)
                {
                    string entryDateStr = c.GetEntryDate().ToString("dd.MM.yyyy");
                    DateTime? applicationDate = c.GetApplicationDate();
                    string appDateStr = applicationDate.HasValue ? applicationDate.Value.ToString("dd.MM.yyyy") : "";
                    string purposeStr = c.Purpose != null ? c.Purpose.Name : "";
                    string citizenStr = c.Citizenship != null ? c.Citizenship.Name : "";

                    string line = $"{c.Login}|{c.Name}|{entryDateStr}|{appDateStr}|{purposeStr}|{citizenStr}";
                    sw.WriteLine(line);
                }
            }
        }

        private void LoadFromFile()
        {
            if (!File.Exists(FilePath)) return;

            string[] lines = File.ReadAllLines(FilePath, Encoding.UTF8);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split('|');
                if (parts.Length >= 6)
                {
                    var c = new ForeignCitizen
                    {
                        Login = parts[0],
                        Name = parts[1]
                    };

                    c.SetEntryDate(DateTime.Parse(parts[2]));

                    if (!string.IsNullOrEmpty(parts[3]))
                        c.SetApplicationDate(DateTime.Parse(parts[3]));
                    else
                        c.SetApplicationDate(null);

                    if (!string.IsNullOrEmpty(parts[4]))
                        c.Purpose = new Purpose(parts[4]);

                    if (!string.IsNullOrEmpty(parts[5]))
                        c.Citizenship = new Citizenship(parts[5]);

                    _citizens.Add(c);
                }
            }
        }
    }
}
