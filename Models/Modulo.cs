using System.ComponentModel.DataAnnotations;
namespace EcdlBooking.Models
{
    // Rappresntazione informatica di una entità Modulo
    // Un modulo rappresenta un singolo esame di ecdl 
    public class Modulo 
    {
        // Definzione della chiave primaria
        [Key]  
        // Ogni attributo dell entità è rappresento con un attributo di una classe
        // Se ne specifica til tipo, è possibile impostare anche delle restrinzioni
        public Guid Id { get; set; }
        public string Nome { get; set; }
        // anticipo:
        // Prenotazioni è una relazione fra le entità Modulo e SchedulerEcdl
        // Un modulo può essere prenotato più volte (List<T>)
        // Descritto in moddo matematico
        // Prenotazione: Modulo -> Prenotazioni
        /*
         * E' una relazione fra le due tabelle Modulo e SchedulerEcdl
         * Serve poi per la stampa delle prenotazioni di un utente
         */
        //public SchedulerEcdl ModuliPrenotazioni { get; set; }
    }
}
