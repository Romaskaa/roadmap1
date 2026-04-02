using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using project.Controllers;

namespace newproj.Tests
{
    [TestClass]
    public class ProfileTests
    {
        [TestMethod]
        public void Profile_Constructor_InitializesPropertiesCorrectly()
        {
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, DateTime.Now.ToString("o"))
            };

            var profile = new Profile("user1", "John Doe", properties);

            Assert.AreEqual("user1", profile.Login);
            Assert.AreEqual("John Doe", profile.Name);
            Assert.AreEqual(1, profile.Properties.Count);
        }

        [TestMethod]
        public void Profile_Constructor_WithNullProperties_CreatesEmptyPropertyList()
        {
            var profile = new Profile("user1", "John Doe", null);

            Assert.IsNotNull(profile.Properties);
            Assert.AreEqual(0, profile.Properties.Count);
        }

        [TestMethod]
        public void IsForeignCitizen_WithNonRussianCitizenship_ReturnsTrue()
        {
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.CitizenshipPropertyName, "USA")
            };
            var profile = new Profile(null, "Test", properties);

            Assert.IsTrue(profile.IsForeignCitizen);
        }

        [TestMethod]
        public void IsForeignCitizen_WithRussianCitizenship_ReturnsFalse()
        {
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.CitizenshipPropertyName, "ад")
            };
            var profile = new Profile(null, "Test", properties);

            Assert.IsFalse(profile.IsForeignCitizen);
        }

        [TestMethod]
        public void IsForeignCitizen_WithNoCitizenship_ReturnsFalse()
        {
            var profile = new Profile(null, "Test", new List<ProfileProperty>());

            Assert.IsFalse(profile.IsForeignCitizen);
        }

        [TestMethod]
        public void GetPropertyValue_WithExistingProperty_ReturnsCorrectValue()
        {
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty("CustomProp", "CustomValue")
            };
            var profile = new Profile(null, "Test", properties);

            var value = profile.GetPropertyValue("CustomProp");

            Assert.AreEqual("CustomValue", value);
        }

        [TestMethod]
        public void GetPropertyValue_WithNonExistingProperty_ReturnsNull()
        {
            var profile = new Profile(null, "Test", new List<ProfileProperty>());

            var value = profile.GetPropertyValue("NonExistent");

            Assert.IsNull(value);
        }

        [TestMethod]
        public void GetEntryDate_WithValidDate_ReturnsCorrectDate()
        {
            var entryDate = DateTime.Now.AddDays(-10);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, entryDate.ToString("o"))
            };
            var profile = new Profile(null, "Test", properties);

            var result = profile.GetEntryDate();

            Assert.AreEqual(entryDate.Date, result.Date);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetEntryDate_WithoutEntryDate_ThrowsInvalidOperationException()
        {
            var profile = new Profile(null, "Test", new List<ProfileProperty>());

            profile.GetEntryDate();
        }

        [TestMethod]
        public void GetApplicationDate_WithValidDate_ReturnsCorrectDate()
        {
            var appDate = DateTime.Now.AddDays(-5);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.ApplicationDatePropertyName, appDate.ToString("o"))
            };
            var profile = new Profile(null, "Test", properties);

            var result = profile.GetApplicationDate();

            Assert.IsNotNull(result);
            Assert.AreEqual(appDate.Date, result.Value.Date);
        }

        [TestMethod]
        public void GetApplicationDate_WithoutApplicationDate_ReturnsNull()
        {
            var profile = new Profile(null, "Test", new List<ProfileProperty>());

            var result = profile.GetApplicationDate();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void HasApplication_WithApplicationDate_ReturnsTrue()
        {
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.ApplicationDatePropertyName, DateTime.Now.ToString("o"))
            };
            var profile = new Profile(null, "Test", properties);

            Assert.IsTrue(profile.HasApplication());
        }

        [TestMethod]
        public void HasApplication_WithoutApplicationDate_ReturnsFalse()
        {
            var profile = new Profile(null, "Test", new List<ProfileProperty>());

            Assert.IsFalse(profile.HasApplication());
        }

        [TestMethod]
        public void GetPurpose_WithPurposeSet_ReturnsCorrectValue()
        {
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.PurposePropertyName, "Work")
            };
            var profile = new Profile(null, "Test", properties);

            var purpose = profile.GetPurpose();

            Assert.AreEqual("Work", purpose);
        }

        [TestMethod]
        public void SetPurpose_UpdatesPropertyCorrectly()
        {
            var profile = new Profile(null, "Test", new List<ProfileProperty>());

            profile.SetPurpose("Study");

            Assert.AreEqual("Study", profile.GetPurpose());
        }

        [TestMethod]
        public void GetCitizenship_WithCitizenshipSet_ReturnsCorrectValue()
        {
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.CitizenshipPropertyName, "USA")
            };
            var profile = new Profile(null, "Test", properties);

            var citizenship = profile.GetCitizenship();

            Assert.AreEqual("USA", citizenship);
        }

        [TestMethod]
        public void SetCitizenship_UpdatesPropertyCorrectly()
        {
            var profile = new Profile(null, "Test", new List<ProfileProperty>());

            profile.SetCitizenship("Canada");

            Assert.AreEqual("Canada", profile.GetCitizenship());
        }

        [TestMethod]
        public void SetEntryDate_UpdatesPropertyCorrectly()
        {
            var profile = new Profile(null, "Test", new List<ProfileProperty>());
            var entryDate = DateTime.Now;

            profile.SetEntryDate(entryDate);

            Assert.AreEqual(entryDate.Date, profile.GetEntryDate().Date);
        }

        [TestMethod]
        public void SetApplicationDate_WithValidDate_UpdatesPropertyCorrectly()
        {
            var profile = new Profile(null, "Test", new List<ProfileProperty>());
            var appDate = DateTime.Now;

            profile.SetApplicationDate(appDate);

            Assert.IsNotNull(profile.GetApplicationDate());
            Assert.AreEqual(appDate.Date, profile.GetApplicationDate().Value.Date);
        }

        [TestMethod]
        public void SetApplicationDate_WithNull_ClearsProperty()
        {
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.ApplicationDatePropertyName, DateTime.Now.ToString("o"))
            };
            var profile = new Profile(null, "Test", properties);

            profile.SetApplicationDate(null);

            Assert.IsFalse(profile.HasApplication());
        }

        [TestMethod]
        public void SetProperty_NewProperty_AddsToList()
        {
            var profile = new Profile(null, "Test", new List<ProfileProperty>());

            profile.SetProperty("TestProp", "TestValue");

            Assert.AreEqual("TestValue", profile.GetPropertyValue("TestProp"));
        }

        [TestMethod]
        public void SetProperty_ExistingProperty_UpdatesValue()
        {
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty("TestProp", "OldValue")
            };
            var profile = new Profile(null, "Test", properties);

            profile.SetProperty("TestProp", "NewValue");

            Assert.AreEqual("NewValue", profile.GetPropertyValue("TestProp"));
        }
    }
}
