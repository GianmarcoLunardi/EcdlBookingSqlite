using EcdlBooking.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcdlBooking.ViewModel.User.Studenti
{
    public class PrenotaEsameVM
    {
        [ValidateNever]
        public List<EcdlBooking.Models.Exam> ListaEsami { get; set; }
        [ValidateNever]
        public List<SelectListItem> ListModuli { get; set; }
        public string ModuloSelezionato { get; set; }
        public Guid IdEsame { get; set; }
        public String IdStudente { get; set; }
    }
}

