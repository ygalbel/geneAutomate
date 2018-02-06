using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneAutomate.Models
{
    public class GeneLink
    {
        public string From { get; set; }

        public string To { get; set; }

        public bool IsPositive { get; set; }

        public bool IsKnockout { get; set; }

        public bool IsOverExpression { get; set; }

        public bool IsOptional { get; set; }
    }
}
