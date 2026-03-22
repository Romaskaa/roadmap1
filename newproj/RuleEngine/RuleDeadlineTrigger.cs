using System;
using project.Controllers;

namespace newproj.RuleEngine
{
    public enum DeadlineAnchor
    {
        EntryDate,
        ApplicationDate,
        EntryOrApplicationDate
    }

    public class RuleDeadlineTrigger
    {
        public DeadlineAnchor Anchor { get; set; }
        public int Days { get; set; }
        public string Description { get; set; }

        public DateTime? ResolveDeadline(ForeignCitizen citizen)
        {
            if (citizen == null)
                return null;

            DateTime baseDate;

            switch (Anchor)
            {
                case DeadlineAnchor.ApplicationDate:
                    if (!citizen.HasApplication())
                        return null;

                    baseDate = citizen.GetApplicationDate().Value;
                    break;
                case DeadlineAnchor.EntryOrApplicationDate:
                    baseDate = citizen.GetApplicationDate() ?? citizen.GetEntryDate();
                    break;
                default:
                    baseDate = citizen.GetEntryDate();
                    break;
            }

            return baseDate.AddDays(Days);
        }
    }
}
