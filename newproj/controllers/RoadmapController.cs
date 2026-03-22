using project.collections;
using project.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Collections
{
    public class RoadmapController
    {
        private readonly RoadmapCollection _roadmap;

        public RoadmapController()
        {
            var citizens = ForeignCitizenCollection.Instance;
            var rules = new RuleCollection();

            _roadmap = new RoadmapCollection(citizens, rules);
        }

        public string GetMessage(string login)
        {
            return _roadmap.GetMessage(login);
        }
    }
}