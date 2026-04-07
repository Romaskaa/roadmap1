using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using newproj.DTO;
using newproj.RuleEngine;
using project.Controllers;

namespace newproj.Builders
{
    public class RuleBuilder
    {
        public Roadmap BuildRoadmap(JsonRoadmap jsonRoadmap)
        {
            if (jsonRoadmap == null)
                throw new ArgumentNullException("jsonRoadmap");

            return new Roadmap
            {
                Version = string.IsNullOrWhiteSpace(jsonRoadmap.Version) ? "1.0" : jsonRoadmap.Version,
                Rules = (jsonRoadmap.Rules ?? new List<JsonRule>())
                    .Select(Build)
                    .OrderBy(rule => rule.Order)
                    .ToList()
            };
        }

        public Rule Build(JsonRule jsonRule)
        {
            if (jsonRule == null)
                throw new ArgumentNullException("jsonRule");

            return new Rule
            {
                Name = jsonRule.Name,
                Order = jsonRule.Order,
                Guide = BuildGuide(jsonRule.Guide),
                TargetDocuments = BuildTargetDocuments(jsonRule.TargetDocuments),
                Trigger = BuildTrigger(jsonRule.Trigger),
                Condition = BuildCondition(jsonRule.Conditions)
            };
        }

        private Guide BuildGuide(JsonGuide guide)
        {
            var result = new Guide();

            if (guide == null)
                return result;

            result.Description = guide.Description;
            result.Rejection = guide.Rejection;

            return result;
        }

        private List<TargetDocument> BuildTargetDocuments(List<JsonTargetDocument> targetDocuments)
        {
            if (targetDocuments == null)
                return new List<TargetDocument>();

            return targetDocuments
                .Select(document => new TargetDocument
                {
                    Name = document.Name,
                    Organizations = (document.Organizations ?? new List<JsonOrganization>())
                        .Select(o => new Organization
                        {
                            Name = o.Name,
                            Address = o.Address
                        })
                        .ToList()
                })
                .ToList();
        }

        private RuleDeadlineTrigger BuildTrigger(JsonDeadlineTrigger trigger)
        {
            if (trigger == null)
                return new RuleDeadlineTrigger();

            return new RuleDeadlineTrigger
            {
                Anchor = ParseAnchor(trigger.Anchor),
                Days = trigger.Days,
                Description = trigger.Description
            };
        }

        private DeadlineAnchor ParseAnchor(string anchor)
        {
            if (string.Equals(anchor, "application", StringComparison.OrdinalIgnoreCase))
                return DeadlineAnchor.ApplicationDate;

            if (string.Equals(anchor, "entry_or_application", StringComparison.OrdinalIgnoreCase))
                return DeadlineAnchor.EntryOrApplicationDate;

            return DeadlineAnchor.EntryDate;
        }

        private IRuleCondition BuildCondition(JsonRuleCondition conditions)
        {
            var builder = new CompositeRuleCondition();

            if (conditions == null)
                return builder;

            if (conditions.IsForeignCitizen.HasValue)
                builder.Add(new ForeignCitizenCondition(conditions.IsForeignCitizen.Value));

            if (conditions.MinStayDaysExclusive.HasValue || conditions.MaxStayDaysInclusive.HasValue)
                builder.Add(new StayDurationCondition(conditions.MinStayDaysExclusive, conditions.MaxStayDaysInclusive));

            if (conditions.RequiresApplication.HasValue)
                builder.Add(new ApplicationPresenceCondition(conditions.RequiresApplication.Value));

            if (conditions.ProfileProperties != null)
            {
                foreach (var propertyCondition in conditions.ProfileProperties)
                {
                    if (propertyCondition.Values != null && propertyCondition.Values.Any())
                    {
                        builder.Add(new ProfilePropertyInSetCondition(propertyCondition.Name, propertyCondition.Values));
                        continue;
                    }

                    builder.Add(new ProfilePropertyCondition(propertyCondition.Name, propertyCondition.Value));
                }
            }

            return builder;
        }
    }
}
