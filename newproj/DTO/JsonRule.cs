using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newproj.DTO
{
    public class JsonRule
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public List<string> TargetDocuments { get; set; }
        public JsonGuide Guide { get; set; }
        public JsonDeadlineTrigger Trigger { get; set; }
        public JsonRuleCondition Conditions { get; set; }
    }

    public class JsonGuide
    {
        public string Description { get; set; }
        public string Rejection { get; set; }
        public List<JsonOrganization> Organizations { get; set; }
    }

    public class JsonOrganization
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class JsonDeadlineTrigger
    {
        public string Anchor { get; set; }
        public int Days { get; set; }
        public string Description { get; set; }
    }

    public class JsonRuleCondition
    {
        public bool? IsForeignCitizen { get; set; }
        public int? MinStayDaysExclusive { get; set; }
        public int? MaxStayDaysInclusive { get; set; }
        public bool? RequiresApplication { get; set; }
        public List<JsonProfilePropertyCondition> ProfileProperties { get; set; }
    }

    public class JsonProfilePropertyCondition
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public List<string> Values { get; set; }
    }
}
