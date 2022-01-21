using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADM_RT_CORE_LIB.Models
{
    public class EmailData
    {
        [Required]
        public string AppName { get; set; }
        public List<string> Copies { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public List<string> Receivers { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }

    public class SerializableEmailData
    {
        public string AppName { get; set; }
        public List<string> Copies { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public List<string> Receivers { get; set; }
        public List<FileData> Attachments { get; set; }
    }

    public class FileData
    {
        public byte[] Content { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
