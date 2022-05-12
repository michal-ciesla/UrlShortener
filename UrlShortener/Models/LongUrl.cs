using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models
{ 
    public class LongUrl
    {
        [Required]
        [StringLength(2048)]
        public string Value { get; set;  }

        public bool IsValid() => Uri.TryCreate(Value, UriKind.Absolute, out Uri _);
    }
}
