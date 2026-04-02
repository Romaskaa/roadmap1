using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using newproj.Infrastructure;
using newproj.DTO;
using System.IO;
using System.Text;

namespace newproj.Tests
{
    [TestClass]
    public class JsonRuleLoaderTests
    {
        private JsonRuleLoader _loader;
        private string _testFilePath;

        [TestInitialize]
        public void Setup()
        {
            _loader = new JsonRuleLoader();
            _testFilePath = Path.Combine(Path.GetTempPath(), "test_rules.json");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Load_WithNonExistentFile_ThrowsFileNotFoundException()
        {
            var nonExistentPath = Path.Combine(Path.GetTempPath(), "non_existent_rules.json");

            _loader.Load(nonExistentPath);
        }

        [TestMethod]
        public void Load_WithValidJsonFile_ReturnsRulesList()
        {
            var json = @"[
                {
                    ""Name"": ""Test Rule"",
                    ""Order"": 1,
                    ""TargetDocuments"": [""Passport""],
                    ""Guide"": null,
                    ""Trigger"": null,
                    ""Conditions"": null
                }
            ]";

            File.WriteAllText(_testFilePath, json, Encoding.UTF8);

            var result = _loader.Load(_testFilePath);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test Rule", result[0].Name);
        }

        [TestMethod]
        public void Load_WithEmptyJsonArray_ReturnsEmptyList()
        {
            var json = "[]";
            File.WriteAllText(_testFilePath, json, Encoding.UTF8);

            var result = _loader.Load(_testFilePath);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Load_WithComplexJsonRule_ParsesCorrectly()
        {
            var json = @"[
                {
                    ""Name"": ""Apply for Visa"",
                    ""Order"": 1,
                    ""TargetDocuments"": [""Visa Application"", ""Passport Copy""],
                    ""Guide"": {
                        ""Description"": ""Fill in the application form"",
                        ""Rejection"": ""Application may be rejected"",
                        ""Organizations"": [
                            {
                                ""Name"": ""Visa Center"",
                                ""Address"": ""Main Street 123""
                            }
                        ]
                    },
                    ""Trigger"": {
                        ""Anchor"": ""application"",
                        ""Days"": 30,
                        ""Description"": ""30 days from application""
                    },
                    ""Conditions"": {
                        ""IsForeignCitizen"": true,
                        ""RequiresApplication"": true
                    }
                }
            ]";

            File.WriteAllText(_testFilePath, json, Encoding.UTF8);

            var result = _loader.Load(_testFilePath);

            Assert.AreEqual(1, result.Count);
            var rule = result[0];
            Assert.AreEqual("Apply for Visa", rule.Name);
            Assert.AreEqual(1, rule.Order);
            Assert.AreEqual(2, rule.TargetDocuments.Count);
            Assert.IsNotNull(rule.Guide);
            Assert.AreEqual("Fill in the application form", rule.Guide.Description);
            Assert.AreEqual(1, rule.Guide.Organizations.Count);
            Assert.IsNotNull(rule.Trigger);
            Assert.AreEqual("application", rule.Trigger.Anchor);
        }

        [TestMethod]
        public void Load_WithMultipleRules_ParsesAll()
        {
            var json = @"[
                {
                    ""Name"": ""Rule 1"",
                    ""Order"": 1,
                    ""TargetDocuments"": null,
                    ""Guide"": null,
                    ""Trigger"": null,
                    ""Conditions"": null
                },
                {
                    ""Name"": ""Rule 2"",
                    ""Order"": 2,
                    ""TargetDocuments"": null,
                    ""Guide"": null,
                    ""Trigger"": null,
                    ""Conditions"": null
                }
            ]";

            File.WriteAllText(_testFilePath, json, Encoding.UTF8);

            var result = _loader.Load(_testFilePath);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Rule 1", result[0].Name);
            Assert.AreEqual("Rule 2", result[1].Name);
        }

        [TestMethod]
        public void Load_WithUtf8Content_HandlesCorrectly()
        {
            var json = @"[
                {
                    ""Name"": ""Получить визу"",
                    ""Order"": 1,
                    ""TargetDocuments"": [""Заявление на визу""],
                    ""Guide"": null,
                    ""Trigger"": null,
                    ""Conditions"": null
                }
            ]";

            File.WriteAllText(_testFilePath, json, Encoding.UTF8);

            var result = _loader.Load(_testFilePath);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Получить визу", result[0].Name);
        }
    }
}
