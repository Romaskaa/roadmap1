using project.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.collections
{
    public class RoadmapCollection
    {
        private ForeignCitizenCollection _citizens;
        private RuleCollection _rules;

        public RoadmapCollection(ForeignCitizenCollection citizens, RuleCollection rules)
        {
            _citizens = citizens;
            _rules = rules;
        }

        public string GetMessage(string login)
        {
            var citizen = _citizens.GetCitizen(login);

            if (citizen == null)
                return "Пользователь не найден.";

            return _rules.GetMessage(citizen);
        }
    }
}