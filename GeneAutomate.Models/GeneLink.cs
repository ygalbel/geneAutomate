using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneAutomate.Models
{
    public class FileParsingResult
    {
        public List<GeneLink> GeneLinks { get; set; }
    }


    public class GeneLink
    {
        public string From { get; set; }

        public string To { get; set; }

        public bool IsPositive { get; set; }

        public bool IsOptional { get; set; }
    }
}
