using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using project.Controllers;

namespace newproj.Tests
{
    [TestClass]
    public class RuleTests
    {
        [TestMethod]
        public void Rule_Constructor_InitializesTargetDocumentsCollection()
        {
            var rule = new Rule();

            Assert.IsNotNull(rule.TargetDocuments);
            Assert.AreEqual(0, rule.TargetDocuments.Count);
        }

        [TestMethod]
        public void IsApplicable_WithNullCondition_ReturnsTrue()
        {
            var rule = new Rule { Condition = null };
            var citizen = new Profile(null, "Test", new List<ProfileProperty>());

            var result = rule.IsApplicable(citizen);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsApplicable_WithSatisfiedCondition_ReturnsTrue()
        {
            var condition = new newproj.RuleEngine.ForeignCitizenCondition(true);
            var rule = new Rule { Condition = condition };
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.CitizenshipPropertyName, "USA")
            };
            var citizen = new Profile(null, "Test", properties);

            var result = rule.IsApplicable(citizen);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsApplicable_WithUnsatisfiedCondition_ReturnsFalse()
        {
            var condition = new newproj.RuleEngine.ForeignCitizenCondition(true);
            var rule = new Rule { Condition = condition };
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.CitizenshipPropertyName, "РФ")
            };
            var citizen = new Profile(null, "Test", properties);

            var result = rule.IsApplicable(citizen);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Apply_WithBasicRule_GeneratesCorrectMessage()
        {
            var rule = new Rule
            {
                Name = "Get Passport",
                Order = 1,
                TargetDocuments = new List<TargetDocument>
                {
                    new TargetDocument("Passport")
                }
            };
            var citizen = new Profile(null, "Test", new List<ProfileProperty>());

            var result = rule.Apply(citizen);

            Assert.IsTrue(result.Contains("Get Passport"));
            Assert.IsTrue(result.Contains("Passport"));
        }

        [TestMethod]
        public void Apply_WithGuide_IncludesGuideDescription()
        {
            var rule = new Rule
            {
                Name = "Test Rule",
                Order = 1,
                Guide = new Guide
                {
                    Description = "Apply at the embassy"
                }
            };
            var citizen = new Profile(null, "Test", new List<ProfileProperty>());

            var result = rule.Apply(citizen);

            Assert.IsTrue(result.Contains("Apply at the embassy"));
        }

        [TestMethod]
        public void Apply_WithOrganizations_IncludesOrganizationInfo()
        {
            var rule = new Rule
            {
                Name = "Test Rule",
                Order = 1,
                Guide = new Guide
                {
                    Organizations = new List<Organization>
                    {
                        new Organization { Name = "Embassy", Address = "Main Street" }
                    }
                }
            };
            var citizen = new Profile(null, "Test", new List<ProfileProperty>());

            var result = rule.Apply(citizen);

            Assert.IsTrue(result.Contains("Embassy"));
            Assert.IsTrue(result.Contains("Main Street"));
        }

        [TestMethod]
        public void Apply_WithDeadline_IncludesDeadlineInfo()
        {
            var trigger = new newproj.RuleEngine.RuleDeadlineTrigger
            {
                Anchor = newproj.RuleEngine.DeadlineAnchor.EntryDate,
                Days = 30,
                Description = "30 days from entry"
            };
            var rule = new Rule
            {
                Name = "Test Rule",
                Order = 1,
                Trigger = trigger
            };
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, DateTime.Now.AddDays(-10).ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);

            var result = rule.Apply(citizen);

            Assert.IsTrue(result.Contains("30 days from entry"));
            Assert.IsTrue(result.Contains("Крайний срок"));
        }

        [TestMethod]
        public void Apply_WithPassedDeadline_IncludesPassedStatus()
        {
            var trigger = new newproj.RuleEngine.RuleDeadlineTrigger
            {
                Anchor = newproj.RuleEngine.DeadlineAnchor.EntryDate,
                Days = -5,
                Description = "Deadline passed"
            };
            var rule = new Rule
            {
                Name = "Test Rule",
                Order = 1,
                Trigger = trigger
            };
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, DateTime.Now.AddDays(-10).ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);

            var result = rule.Apply(citizen);

            Assert.IsTrue(result.Contains("срок пропущен"));
        }

        [TestMethod]
        public void Apply_WithMultipleDocuments_ListsAllDocuments()
        {
            var rule = new Rule
            {
                Name = "Get Documents",
                Order = 1,
                TargetDocuments = new List<TargetDocument>
                {
                    new TargetDocument("Passport"),
                    new TargetDocument("Visa"),
                    new TargetDocument("Work Permit")
                }
            };
            var citizen = new Profile(null, "Test", new List<ProfileProperty>());

            var result = rule.Apply(citizen);

            Assert.IsTrue(result.Contains("Passport"));
            Assert.IsTrue(result.Contains("Visa"));
            Assert.IsTrue(result.Contains("Work Permit"));
        }
    }
}
