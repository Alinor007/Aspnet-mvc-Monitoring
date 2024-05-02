using System.ComponentModel.DataAnnotations;

namespace Monitoring.Models
{
    public class Parent
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ParentId { get; set; }
        [Display(Name = "Parent Name")]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<string> ChildId { get; set; } // List of Child IDs
    }
}

