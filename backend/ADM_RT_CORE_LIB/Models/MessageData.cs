using System.ComponentModel.DataAnnotations;

namespace ADM_RT_CORE_LIB.Models
{
    public class MessageData
    {
        [Required]
        public string AppName { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
