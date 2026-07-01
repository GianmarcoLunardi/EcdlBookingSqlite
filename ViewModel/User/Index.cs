using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcdlBooking.ViewModel.User
{
    public class Index
    {
        public List<EcdlBooking.Models.Exam> ListaEsami { get; set; }
        public List<SelectListItem> ListModuli { get; set; }
        
    }
}
