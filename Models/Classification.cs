using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectFoodRecall.Models
{
    public class Classification
    {
        [Key]
        public int classificationId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string classification { get; set; }
        public string center_classification_date { get; set; }

        [Required]
        public Recall_Item recall { get; set; }
    } 
}
