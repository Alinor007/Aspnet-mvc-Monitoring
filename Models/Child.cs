using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Models
{
    public class Child
    {
        public string ChildId { get; set; }
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }

        public string BloodType { get; set; }

        public string Address { get; set; }

        [ForeignKey("ParentId")]
        public Parent Parent { get; set; }
        public string ParentId { get; set; } // Reference to parent document
        [Display(Name = "Parent Name")]
        public string ParentName { get; set; } // Add this property
    }
}
