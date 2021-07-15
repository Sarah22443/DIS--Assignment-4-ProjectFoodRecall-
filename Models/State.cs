using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectFoodRecall.Models
{
    public class State
    {
        public int StateId { get; set; }

        public string State_Code { get; set; }

        [Required]
        public Recall_Item Recall { get; set; }

    }
}
