using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Monitoring.Models
{
    public class TaskModel
    {
        public int TaskID { get; set; }

        [Required(ErrorMessage = "Task name is required.")]
        public string TaskName { get; set; }

        public string? TaskDescription { get; set; }

        public bool IsCompleted { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public UserModel User { get; set; }
    }
}
