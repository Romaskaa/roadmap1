using System.Collections.Generic;

namespace project.Controllers
{
    public class Roadmap
    {
        public string Version { get; set; }

        public List<Rule> Rules { get; set; }

        public Roadmap()
        {
            Rules = new List<Rule>();
        }
    }
}