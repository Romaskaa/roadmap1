using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using newproj.RuleEngine;
using project.Controllers;

namespace newproj.Tests
{
    [TestClass]
    public class RuleDeadlineTriggerTests
    {
        private RuleDeadlineTrigger _trigger;

        [TestInitialize]
        public void Setup()
        {
            _trigger = new RuleDeadlineTrigger();
        }

        [TestMethod]
        public void ResolveDeadline_WithEntryDateAnchor_CalculatesCorrectDeadline()
        {
            var entryDate = DateTime.Now.AddDays(-10);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, entryDate.ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);

            _trigger.Anchor = DeadlineAnchor.EntryDate;
            _trigger.Days = 30;

            var deadline = _trigger.ResolveDeadline(citizen);

            Assert.IsNotNull(deadline);
            Assert.AreEqual(entryDate.AddDays(30).Date, deadline.Value.Date);
        }

        [TestMethod]
        public void ResolveDeadline_WithApplicationDateAnchor_CalculatesCorrectDeadline()
        {
            var entryDate = DateTime.Now.AddDays(-20);
            var applicationDate = DateTime.Now.AddDays(-10);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, entryDate.ToString("o")),
                new ProfileProperty(Profile.ApplicationDatePropertyName, applicationDate.ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);

            _trigger.Anchor = DeadlineAnchor.ApplicationDate;
            _trigger.Days = 30;

            var deadline = _trigger.ResolveDeadline(citizen);

            Assert.IsNotNull(deadline);
            Assert.AreEqual(applicationDate.AddDays(30).Date, deadline.Value.Date);
        }

        [TestMethod]
        public void ResolveDeadline_WithApplicationDateAnchorButNoApplication_ReturnsNull()
        {
            var entryDate = DateTime.Now.AddDays(-10);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, entryDate.ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);

            _trigger.Anchor = DeadlineAnchor.ApplicationDate;
            _trigger.Days = 30;

            var deadline = _trigger.ResolveDeadline(citizen);

            Assert.IsNull(deadline);
        }

        [TestMethod]
        public void ResolveDeadline_WithEntryOrApplicationAnchorAndApplication_UsesApplicationDate()
        {
            var entryDate = DateTime.Now.AddDays(-20);
            var applicationDate = DateTime.Now.AddDays(-5);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, entryDate.ToString("o")),
                new ProfileProperty(Profile.ApplicationDatePropertyName, applicationDate.ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);

            _trigger.Anchor = DeadlineAnchor.EntryOrApplicationDate;
            _trigger.Days = 30;

            var deadline = _trigger.ResolveDeadline(citizen);

            Assert.IsNotNull(deadline);
            Assert.AreEqual(applicationDate.AddDays(30).Date, deadline.Value.Date);
        }

        [TestMethod]
        public void ResolveDeadline_WithEntryOrApplicationAnchorNoApplication_UsesEntryDate()
        {
            var entryDate = DateTime.Now.AddDays(-20);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, entryDate.ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);

            _trigger.Anchor = DeadlineAnchor.EntryOrApplicationDate;
            _trigger.Days = 30;

            var deadline = _trigger.ResolveDeadline(citizen);

            Assert.IsNotNull(deadline);
            Assert.AreEqual(entryDate.AddDays(30).Date, deadline.Value.Date);
        }

        [TestMethod]
        public void ResolveDeadline_WithZeroDays_ReturnsEntryDate()
        {
            var entryDate = DateTime.Now.AddDays(-10);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, entryDate.ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);

            _trigger.Anchor = DeadlineAnchor.EntryDate;
            _trigger.Days = 0;

            var deadline = _trigger.ResolveDeadline(citizen);

            Assert.IsNotNull(deadline);
            Assert.AreEqual(entryDate.Date, deadline.Value.Date);
        }

        [TestMethod]
        public void ResolveDeadline_WithNegativeDays_ReturnsDateInThePast()
        {
            var entryDate = DateTime.Now.AddDays(-10);
            var properties = new List<ProfileProperty>
            {
                new ProfileProperty(Profile.EntryDatePropertyName, entryDate.ToString("o"))
            };
            var citizen = new Profile(null, "Test", properties);

            _trigger.Anchor = DeadlineAnchor.EntryDate;
            _trigger.Days = -5;

            var deadline = _trigger.ResolveDeadline(citizen);

            Assert.IsNotNull(deadline);
            Assert.AreEqual(entryDate.AddDays(-5).Date, deadline.Value.Date);
        }
    }
}
