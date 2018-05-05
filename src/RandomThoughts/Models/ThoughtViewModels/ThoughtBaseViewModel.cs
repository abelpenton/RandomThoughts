using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RandomThoughts.Domain.Enums;

namespace RandomThoughts.Models.ThoughtViewModels
{
    public class ThoughtBaseViewModel
    {
        [Required]
        [MaxLength(30)]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public ThinkerMood Mood { get; set; }
    }
}
