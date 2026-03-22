using project.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Collections
{
    public class PurposeCollection
    {
        private List<Purpose> _purposes;

        public PurposeCollection()
        {
            _purposes = new List<Purpose>
            {
                new Purpose("Трудовая деятельность"),
                new Purpose("Иная")
            };
        }

        public List<Purpose> GetListPurposes() => _purposes;

    }
}