using project.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newproj.RuleEngine
{
    public interface IRuleCondition
    {
        bool IsSatisfied(ForeignCitizen citizen);
    }
}
