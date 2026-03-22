using System.Collections.Generic;

namespace project.Controllers
{
    public class Guide
    {
        public string Description { get; set; }
        public string Rejection { get; set; }
        public List<MedicalOrganization> Organizations { get; set; }

        public Guide()
        {
            Organizations = new List<MedicalOrganization>();
        }
    }
}
