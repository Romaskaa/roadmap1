using System.Collections.Generic;

namespace project.Controllers
{
    public class TargetDocument
    {
        public string Name { get; set; }
        public List<Organization> Organizations { get; set; }

        public TargetDocument()
        {
            Organizations = new List<Organization>();
        }

        public TargetDocument(string name)
        {
            Name = name;
            Organizations = new List<Organization>();
        }
    }
}