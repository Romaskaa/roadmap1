using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using newproj.Builders;
using newproj.DTO;
using newproj.RuleEngine;
using project.Controllers;

namespace newproj.Tests
{
    [TestClass]
    public class RuleBuilderTests
    {
        private RuleBuilder _builder;

        [TestInitialize]
        public void Setup()
        {
            _builder = new RuleBuilder();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Build_WithNullJsonRule_ThrowsArgumentNullException()
        {
            _builder.Build(null);
        }

        [TestMethod]
        public void Build_WithValidJsonRule_ReturnsRuleWithCorrectProperties()
        {
            var jsonRule = new JsonRule
            {
                Name = "Test Rule",
                Order = 1
            };

            var result = _builder.Build(jsonRule);

            Assert.IsNotNull(result);
            Assert.AreEqual("Test Rule", result.Name);
            Assert.AreEqual(1, result.Order);
        }

        [TestMethod]
        public void Build_WithNullGuide_ReturnsRuleWithEmptyGuide()
        {
            var jsonRule = new JsonRule { Name = "Test", Order = 1, Guide = null };

            var result = _builder.Build(jsonRule);

            Assert.IsNotNull(result.Guide);
            Assert.IsNull(result.Guide.Description);
        }

        [TestMethod]
        public void Build_WithGuideAndOrganizations_CreatesCorrectGuide()
        {
            var jsonRule = new JsonRule
            {
                Name = "Test",
                Order = 1,
                Guide = new JsonGuide
                {
                    Description = "Test description",
                    Rejection = "Test rejection",
                    Organizations = new List<JsonOrganization>
                    {
                        new JsonOrganization { Name = "Org1", Address = "Address1" },
                        new JsonOrganization { Name = "Org2", Address = "Address2" }
                    }
                }
            };

            var result = _builder.Build(jsonRule);

            Assert.AreEqual("Test description", result.Guide.Description);
            Assert.AreEqual("Test rejection", result.Guide.Rejection);
            Assert.AreEqual(2, result.Guide.Organizations.Count);
            Assert.AreEqual("Org1", result.Guide.Organizations[0].Name);
            Assert.AreEqual("Address1", result.Guide.Organizations[0].Address);
        }

        [TestMethod]
        public void Build_WithNullTargetDocuments_ReturnsEmptyTargetDocumentsList()
        {
            var jsonRule = new JsonRule { Name = "Test", Order = 1, TargetDocuments = null };

            var result = _builder.Build(jsonRule);

            Assert.IsNotNull(result.TargetDocuments);
            Assert.AreEqual(0, result.TargetDocuments.Count);
        }

        [TestMethod]
        public void Build_WithTargetDocuments_CreatesCorrectDocuments()
        {
            var jsonRule = new JsonRule
            {
                Name = "Test",
                Order = 1,
                TargetDocuments = new List<string> { "Passport", "Visa", "Permit" }
            };

            var result = _builder.Build(jsonRule);

            Assert.AreEqual(3, result.TargetDocuments.Count);
            Assert.AreEqual("Passport", result.TargetDocuments[0].Name);
            Assert.AreEqual("Visa", result.TargetDocuments[1].Name);
            Assert.AreEqual("Permit", result.TargetDocuments[2].Name);
        }

        [TestMethod]
        public void Build_WithNullTrigger_ReturnsRuleWithEmptyTrigger()
        {
            var jsonRule = new JsonRule { Name = "Test", Order = 1, Trigger = null };

            var result = _builder.Build(jsonRule);

            Assert.IsNotNull(result.Trigger);
            Assert.AreEqual(DeadlineAnchor.EntryDate, result.Trigger.Anchor);
        }

        [TestMethod]
        public void Build_WithTrigger_CreatesCorrectTrigger()
        {
            var jsonRule = new JsonRule
            {
                Name = "Test",
                Order = 1,
                Trigger = new JsonDeadlineTrigger
                {
                    Anchor = "application",
                    Days = 30,
                    Description = "30 days from application"
                }
            };

            var result = _builder.Build(jsonRule);

            Assert.AreEqual(DeadlineAnchor.ApplicationDate, result.Trigger.Anchor);
            Assert.AreEqual(30, result.Trigger.Days);
            Assert.AreEqual("30 days from application", result.Trigger.Description);
        }

        [TestMethod]
        public void Build_WithAnchorApplication_ParsesCorrectly()
        {
            var jsonRule = new JsonRule
            {
                Name = "Test",
                Order = 1,
                Trigger = new JsonDeadlineTrigger { Anchor = "application" }
            };

            var result = _builder.Build(jsonRule);

            Assert.AreEqual(DeadlineAnchor.ApplicationDate, result.Trigger.Anchor);
        }

        [TestMethod]
        public void Build_WithAnchorEntryOrApplication_ParsesCorrectly()
        {
            var jsonRule = new JsonRule
            {
                Name = "Test",
                Order = 1,
                Trigger = new JsonDeadlineTrigger { Anchor = "entry_or_application" }
            };

            var result = _builder.Build(jsonRule);

            Assert.AreEqual(DeadlineAnchor.EntryOrApplicationDate, result.Trigger.Anchor);
        }

        [TestMethod]
        public void Build_WithAnchorEntry_ParsesCorrectly()
        {
            var jsonRule = new JsonRule
            {
                Name = "Test",
                Order = 1,
                Trigger = new JsonDeadlineTrigger { Anchor = "entry" }
            };

            var result = _builder.Build(jsonRule);

            Assert.AreEqual(DeadlineAnchor.EntryDate, result.Trigger.Anchor);
        }

        [TestMethod]
        public void Build_WithUnknownAnchor_DefaultsToEntryDate()
        {
            var jsonRule = new JsonRule
            {
                Name = "Test",
                Order = 1,
                Trigger = new JsonDeadlineTrigger { Anchor = "unknown" }
            };

            var result = _builder.Build(jsonRule);

            Assert.AreEqual(DeadlineAnchor.EntryDate, result.Trigger.Anchor);
        }

        [TestMethod]
        public void Build_WithForeignCitizenCondition_CreatesCorrectCondition()
        {
            var jsonRule = new JsonRule
            {
                Name = "Test",
                Order = 1,
                Conditions = new JsonRuleCondition { IsForeignCitizen = true }
            };

            var result = _builder.Build(jsonRule);

            Assert.IsNotNull(result.Condition);
            Assert.IsInstanceOfType(result.Condition, typeof(CompositeRuleCondition));
        }

        [TestMethod]
        public void Build_WithNullConditions_CreatesEmptyCompositeCondition()
        {
            var jsonRule = new JsonRule { Name = "Test", Order = 1, Conditions = null };

            var result = _builder.Build(jsonRule);

            Assert.IsNotNull(result.Condition);
            var citizen = new Profile(null, "Test", new List<ProfileProperty>());
            Assert.IsTrue(result.Condition.IsSatisfied(citizen));
        }

        [TestMethod]
        public void Build_WithMultipleConditions_CreatesCompositeCondition()
        {
            var jsonRule = new JsonRule
            {
                Name = "Test",
                Order = 1,
                Conditions = new JsonRuleCondition
                {
                    IsForeignCitizen = true,
                    RequiresApplication = true
                }
            };

            var result = _builder.Build(jsonRule);

            Assert.IsNotNull(result.Condition);
            Assert.IsInstanceOfType(result.Condition, typeof(CompositeRuleCondition));
        }
    }
}
