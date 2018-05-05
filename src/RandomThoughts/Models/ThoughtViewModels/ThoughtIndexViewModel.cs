using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomThoughts.Models.ThoughtViewModels
{
    public class ThoughtIndexViewModel : ThoughtBaseViewModel
    {
        public int Id { get; set; }

        public string CreateAtHumanized { get; set; }

        public string ModifiedAtHumanized { get; set; }

        public int Likes { get; set; }

        public int Views { get; set; }

        public int NumberComments { get; set; }

        public string CreatedBy { get; set; }
    }
}
