using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Zadanie_5.Models
{
    public class Import
    {
        [Required]
        [Display(Name = "Plik do importu")]
        public IFormFile File { get; set; }

        
        [Display(Name = "Źródło danych")]
        public string DataSource { get; set; }
        [ValidateNever]

        public List<SelectListItem> DataSources { get; set; }
    }
}
