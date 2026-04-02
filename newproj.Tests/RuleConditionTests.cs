using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using newproj.RuleEngine;
using project.Controllers;

namespace newproj.Tests
{
    [TestClass]
    public class RuleConditionTests
    {
        [TestMethod]
        public void ForeignCitizenCondition_WithForeignCitizenTrue_ReturnsTrueForForeignCitizen()
        {
            var condition = new ForeignCitizenCondition(true);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.CitizenshipPropertyName, "USA")
            };
            var citizen = new Profile(null, "Test", properties);

            var result = condition.IsSatisfied(citizen);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ForeignCitizenCondition_WithForeignCitizenTrue_ReturnsFalseForRussianCitizen()
        {
            var condition = new ForeignCitizenCondition(true);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.CitizenshipPropertyName, "ад")
            };
            var citizen = new Profile(null, "Test", properties);

            var result = condition.IsSatisfied(citizen);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ForeignCitizenCondition_WithForeignCitizenFalse_ReturnsTrueForRussianCitizen()
        {
            var condition = new ForeignCitizenCondition(false);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.CitizenshipPropertyName, "ад")
            };
            var citizen = new Profile(null, "Test", properties);

            var result = condition.IsSatisfied(citizen);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ForeignCitizenCondition_WithNullCitizen_ReturnsFalse()
        {
            var condition = new ForeignCitizenCondition(true);

            var result = condition.IsSatisfied(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void StayDurationCondition_WithMinAndMaxBounds_EvaluatesCorrectly()
        {
            var condition = new StayDurationCondition(minExclusive: 10, maxInclusive: 90);
            var entryDate = DateTime.Now.AddDays(-30);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, entryDate.ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);
            citizen.DurationDays = 30;

            var result = condition.IsSatisfied(citizen);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void StayDurationCondition_WithMinBoundViolation_ReturnsFalse()
        {
            var condition = new StayDurationCondition(minExclusive: 30, maxInclusive: null);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, DateTime.Now.AddDays(-10).ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);
            citizen.DurationDays = 15;

            var result = condition.IsSatisfied(citizen);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void StayDurationCondition_WithMaxBoundViolation_ReturnsFalse()
        {
            var condition = new StayDurationCondition(minExclusive: null, maxInclusive: 30);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, DateTime.Now.AddDays(-60).ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);
            citizen.DurationDays = 50;

            var result = condition.IsSatisfied(citizen);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void StayDurationCondition_WithNullCitizen_ReturnsFalse()
        {
            var condition = new StayDurationCondition(10, 90);

            var result = condition.IsSatisfied(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ApplicationPresenceCondition_WithRequiredTrue_ReturnsTrueWhenApplicationExists()
        {
            var condition = new ApplicationPresenceCondition(required: true);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, DateTime.Now.AddDays(-10).ToString("o")),
                new ProfileProperty(Profile.ApplicationDatePropertyName, DateTime.Now.AddDays(-5).ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);

            var result = condition.IsSatisfied(citizen);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ApplicationPresenceCondition_WithRequiredTrue_ReturnsFalseWhenApplicationMissing()
        {
            var condition = new ApplicationPresenceCondition(required: true);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, DateTime.Now.AddDays(-10).ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);

            var result = condition.IsSatisfied(citizen);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ApplicationPresenceCondition_WithRequiredFalse_ReturnsTrueWhenNoApplication()
        {
            var condition = new ApplicationPresenceCondition(required: false);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, DateTime.Now.AddDays(-10).ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);

            var result = condition.IsSatisfied(citizen);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ProfilePropertyCondition_WithMatchingValue_ReturnsTrue()
        {
            var condition = new ProfilePropertyCondition(Profile.PurposePropertyName, "Work");
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.PurposePropertyName, "Work")
            };
            var citizen = new Profile(null, "Test", properties);

            var result = condition.IsSatisfied(citizen);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ProfilePropertyCondition_WithNonMatchingValue_ReturnsFalse()
        {
            var condition = new ProfilePropertyCondition(Profile.PurposePropertyName, "Work");
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.PurposePropertyName, "Study")
            };
            var citizen = new Profile(null, "Test", properties);

            var result = condition.IsSatisfied(citizen);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ProfilePropertyCondition_CaseSensitivity_ReturnsTrueIgnoringCase()
        {
            var condition = new ProfilePropertyCondition(Profile.PurposePropertyName, "work");
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.PurposePropertyName, "Work")
            };
            var citizen = new Profile(null, "Test", properties);

            var result = condition.IsSatisfied(citizen);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ProfilePropertyInSetCondition_WithMatchingValue_ReturnsTrue()
        {
            var condition = new ProfilePropertyInSetCondition(Profile.PurposePropertyName, new[] { "Work", "Study" });
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.PurposePropertyName, "Work")
            };
            var citizen = new Profile(null, "Test", properties);

            var result = condition.IsSatisfied(citizen);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ProfilePropertyInSetCondition_WithNonMatchingValue_ReturnsFalse()
        {
            var condition = new ProfilePropertyInSetCondition(Profile.PurposePropertyName, new[] { "Work", "Study" });
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.PurposePropertyName, "Tourism")
            };
            var citizen = new Profile(null, "Test", properties);

            var result = condition.IsSatisfied(citizen);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ProfilePropertyInSetCondition_IgnoresCaseInComparison()
        {
            var condition = new ProfilePropertyInSetCondition(Profile.PurposePropertyName, new[] { "work", "study" });
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.PurposePropertyName, "Work")
            };
            var citizen = new Profile(null, "Test", properties);

            var result = condition.IsSatisfied(citizen);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CompositeRuleCondition_WithMultipleConditions_AllMustBeSatisfied()
        {
            var composite = new CompositeRuleCondition();
            composite.Add(new ForeignCitizenCondition(true));
            composite.Add(new ApplicationPresenceCondition(true));

            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.CitizenshipPropertyName, "USA"),
                new ProfileProperty(Profile.EntryDatePropertyName, DateTime.Now.AddDays(-10).ToString("o")),
                new ProfileProperty(Profile.ApplicationDatePropertyName, DateTime.Now.AddDays(-5).ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);

            var result = composite.IsSatisfied(citizen);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CompositeRuleCondition_WithOneFailingCondition_ReturnsFalse()
        {
            var composite = new CompositeRuleCondition();
            composite.Add(new ForeignCitizenCondition(true));
            composite.Add(new ApplicationPresenceCondition(true));

            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.CitizenshipPropertyName, "USA"),
                new ProfileProperty(Profile.EntryDatePropertyName, DateTime.Now.AddDays(-10).ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);

            var result = composite.IsSatisfied(citizen);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CompositeRuleCondition_WithNullConditionAdded_IgnoresNull()
        {
            var composite = new CompositeRuleCondition();
            composite.Add(new ForeignCitizenCondition(true));
            composite.Add(null);

            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.CitizenshipPropertyName, "USA")
            };
            var citizen = new Profile(null, "Test", properties);

            var result = composite.IsSatisfied(citizen);

            Assert.IsTrue(result);
        }
    }
}
