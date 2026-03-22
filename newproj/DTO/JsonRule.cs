using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newproj.DTO
{
    public class JsonRule
    {
        public string Title { get; set; }
        public string Action { get; set; }
        public int DurationDays { get; set; }
        public JsonRuleCondition Conditions { get; set; }
    }

    public class JsonRuleCondition
    {
        public string Purpose { get; set; }
        public List<string> Citizenships { get; set; }
    }
}
