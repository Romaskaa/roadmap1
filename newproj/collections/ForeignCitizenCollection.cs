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
        private List<Profile> _citizens;
        private string FilePath = "citizens.txt";

        private ForeignCitizenCollection()
        {
            _citizens = new List<Profile>();
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
            var newCitizen = new Profile(login, name, entryDate, applicationDate);
            _citizens.Add(newCitizen);

            SaveToFile();

            return login;
        }

        public Profile GetCitizen(string login)
        {
            return _citizens.FirstOrDefault(c => c.Login == login);
        }

        public void ChoosePurposeAndCitizenship(string login, string selectedPurpose, string selectedCitizenship)
        {
            var citizen = GetCitizen(login);
            if (citizen != null)
            {
                citizen.SetPurpose(selectedPurpose);
                citizen.SetCitizenship(selectedCitizenship);
                SaveToFile();
            }
        }

        public void SetProfileProperty(string login, string propertyName, string value)
        {
            var citizen = GetCitizen(login);
            if (citizen == null)
                return;

            citizen.SetProperty(propertyName, value);
            SaveToFile();
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
                    string purposeStr = c.GetPurpose() ?? "";
                    string citizenStr = c.GetCitizenship() ?? "";
                    string extraProperties = SerializeAdditionalProperties(c);

                    string line = $"{c.Login}|{c.Name}|{entryDateStr}|{appDateStr}|{purposeStr}|{citizenStr}|{extraProperties}";
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
                    var c = new Profile
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
                        c.SetPurpose(parts[4]);

                    if (!string.IsNullOrEmpty(parts[5]))
                        c.SetCitizenship(parts[5]);

                    if (parts.Length >= 7 && !string.IsNullOrEmpty(parts[6]))
                        DeserializeAdditionalProperties(c, parts[6]);

                    _citizens.Add(c);
                }
            }
        }

        private string SerializeAdditionalProperties(Profile citizen)
        {
            var additionalProperties = citizen.Properties
                .Where(property => property != null)
                .Where(property =>
                    property.Name != Profile.EntryDatePropertyName &&
                    property.Name != Profile.ApplicationDatePropertyName &&
                    property.Name != Profile.PurposePropertyName &&
                    property.Name != Profile.CitizenshipPropertyName)
                .Select(property => string.Format(
                    "{0}={1}",
                    Escape(property.Name),
                    Escape(property.Value)))
                .ToList();

            return string.Join(";", additionalProperties);
        }

        private void DeserializeAdditionalProperties(Profile citizen, string serializedProperties)
        {
            foreach (var item in serializedProperties.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = item.Split(new[] { '=' }, 2);
                if (parts.Length != 2)
                    continue;

                citizen.SetProperty(Unescape(parts[0]), Unescape(parts[1]));
            }
        }

        private string Escape(string value)
        {
            return (value ?? string.Empty)
                .Replace("%", "%25")
                .Replace("|", "%7C")
                .Replace(";", "%3B")
                .Replace("=", "%3D");
        }

        private string Unescape(string value)
        {
            return (value ?? string.Empty)
                .Replace("%3D", "=")
                .Replace("%3B", ";")
                .Replace("%7C", "|")
                .Replace("%25", "%");
        }
    }
}
