using System.ComponentModel.DataAnnotations;

namespace Monitoring.Models
{
    public class HealthRec
    {
        public string RecordId { get; set; }

        public double Height { get; set; }
        public double Weight { get; set; }
        public double BMI { get; set; }
        public int Heart_Rate { get; set; }
        public string Blood_Pressure { get; set; }
        public double Temperature { get; set; }

        [Display(Name = "Date and Time")]
        public DateTime Date_Time { get; set; }
        [Display(Name = "Doctor Name")]
        public Doctor doctor { get; set; }
        [Display(Name = "Doctor Name")]
        public string DocId { get; set; } // Reference to Doctor document
        [Display(Name = "Doctor Name")]
        public string DocName { get; set; } // Add this property

        public Child child { get; set; }
        public string ChildId { get; set; } // Reference to Child document
        [Display(Name = "Child Name")]
        public string ChildName { get; set; } // Add this property



    }
}
