using System.ComponentModel.DataAnnotations;

namespace GPT.Models
{
    public class Question
    {
        public string context { get; set; }

        [Required(ErrorMessage ="Please enter a valid message")]
        public string  message { get; set; }

        public string response { get; set; }
    }

}
