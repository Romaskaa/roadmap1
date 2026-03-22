using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using project.Controllers;

namespace newproj.RuleEngine
{
    public class RuleEngine
    {
        public List<Rule> Evaluate(IEnumerable<Rule> rules, ForeignCitizen citizen)
        {
            return rules
                .Where(rule => rule != null && rule.IsApplicable(citizen))
                .OrderBy(rule => rule.Order)
                .ToList();
        }
    }
}
