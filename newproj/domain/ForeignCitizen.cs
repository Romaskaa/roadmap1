using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Controllers
{
    public class ForeignCitizen
    {
        public const string EntryDatePropertyName = "ДатаВъезда";
        public const string ApplicationDatePropertyName = "ДатаЗаявления";

        public string Name { get; set; }
        public string Login { get; set; }
        public Purpose Purpose { get; set; }
        public Citizenship Citizenship { get; set; }
        public int? DurationDays { get; set; }
        public string RejectionReason { get; set; }
        public List<ProfileProperty> Properties { get; set; }

        public ForeignCitizen(string login, string name, DateTime entryDate, DateTime? applicationDate)
        {
            Login = login;
            Name = name;
            Properties = new List<ProfileProperty>();
            SetEntryDate(entryDate);
            SetApplicationDate(applicationDate);
        }

        public bool IsForeignCitizen
        {
            get { return Citizenship != null && Citizenship.Name != "РФ"; }
        }

        public string GetPropertyValue(string propertyName)
        {
            if (Properties == null)
                return null;

            var property = Properties.FirstOrDefault(p => p.Name == propertyName);
            return property != null ? property.Value : null;
        }

        public DateTime GetEntryDate()
        {
            var value = GetPropertyValue(EntryDatePropertyName);

            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException("В профиле отсутствует дата въезда.");

            return DateTime.Parse(value);
        }

        public DateTime? GetApplicationDate()
        {
            var value = GetPropertyValue(ApplicationDatePropertyName);

            if (string.IsNullOrWhiteSpace(value))
                return null;

            return DateTime.Parse(value);
        }

        public bool HasApplication()
        {
            return GetApplicationDate().HasValue;
        }

        public void SetEntryDate(DateTime entryDate)
        {
            SetProperty(EntryDatePropertyName, entryDate.ToString("o"));
        }

        public void SetApplicationDate(DateTime? applicationDate)
        {
            SetProperty(
                ApplicationDatePropertyName,
                applicationDate.HasValue ? applicationDate.Value.ToString("o") : null);
        }

        public void SetProperty(string propertyName, string value)
        {
            if (Properties == null)
                Properties = new List<ProfileProperty>();

            var property = Properties.FirstOrDefault(p => p.Name == propertyName);

            if (property == null)
            {
                property = new ProfileProperty(propertyName, value);
                Properties.Add(property);
                return;
            }

            property.Value = value;
        }

        public ForeignCitizen()
        {
            Properties = new List<ProfileProperty>();
        }
    }
}
