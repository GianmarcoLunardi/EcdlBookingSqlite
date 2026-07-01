using EcdlBooking.Data;
using EcdlBooking.Models;
using EcdlBooking.Services;
using EcdlBooking.Services.Interfaces;
using EcdlBooking.Services.Repository;
using EcdlBooking.ViewModel;
using EcdlBooking.ViewModel.User;
using EcdlBooking.ViewModel.User.Studenti; // contiene il modello di view per la prenotazione dell esame
using Humanizer.Localisation;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
//using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;



namespace EcdlBooking.Controllers
{
    [Area("User")]
    public class StudentiController : Controller
    {
        private readonly ILogger<StudentiController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db; 
        private readonly SignInManager<ApplicationUser> _signInManager;
        public StudentiController(
            ILogger<StudentiController> logger
            , UserManager<ApplicationUser> _userManager
            ,IUnitOfWork unitOfWork
           ,ApplicationDbContext db
            , SignInManager<ApplicationUser> signInManager
            )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _db = db;
            _signInManager = signInManager;
        }


        // visualizza solo il contenuto del calendario degli esami
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_unitOfWork.Esami.VisualizzaEsami());
            //List<Exam> x =  _unitOfWork.Esami.VisualizzaEsamiDaSostenere();
            //     return View(x);
           
        }




        public IActionResult VisualizzaEsami()
        {

            return View();
        }

        public IActionResult IndexProtetto()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> EsamiSostenuti()
        {
            //ClaimsPrincipal utenteLoggato =  User;
            // ApplicationUser UtenteLoggato = await _userManager.GetUserAsync(User);
            //  string userId = _userManager.GetUserId(User);

           string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ;
            var x = _db.SchedulerExams
                 .Where(e => e.IdStudente == id)
                 .Join(_db.Moduli, e => e.IdModulo, m => m.Id, (e, m) => new { NomeModulo = m.Nome ,
                                                                               DataPrenotazione = e.DataPrenotazione,
                                                                               voto = e.voto,
                                                                               IdExam = e.IdEsame
                 })

                 .Join(_db.Exams, e =>e.IdExam, ex => ex.id, (e, ex) => new { NomeModulo = e.NomeModulo,
                                                                             DataPrenotazione = e.DataPrenotazione,
                                                                             DataEsame = ex.Data,
                                                                             voto = e.voto,
                 })
                .Select(e => new EsamiSotemutiVM
                {
                    DataPrenotazione = e.DataPrenotazione,
                    DataEsame = e.DataEsame,
                    NomeModulo = e.NomeModulo,
                    Voto = e.voto
                })
                .ToList();
               
        
                ;
            //return View(UtenteLoggato);
            return View(x);
        }

        // Modulo della pagina per l inserimento di una prenotazione di una specifico modulo 
        //da parte di uno user loggato
        /// <summary>
        /// [Authorize]
        /// </summary>
        /// <returns></returns>
        /// 

        [Authorize]
        public async Task<IActionResult> PrenotaEsameAutenticato ()
        {
            // La consultazione della pagina è fatta solo se un utente è loggato
            // ricerca dell utent loggato
            
            string idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
          
            ApplicationUser utente = await _unitOfWork.Utente.GetAsync(idUser);

            //controllo dei valori trovati


               //
               //Exam exam = _db.Exams.Find(Id) ;



            // User_Studente_PrenotaEsame PE = new User_Studente_PrenotaEsame();

            //User_Studente_PrenotaEsame Prenotazione = new User_Studente_PrenotaEsame();

            /*
            PrenotaEsameVM Prenotazione = new PrenotaEsameVM(); Prenotazione.Id = idUser;

            //Id = Guid.NewGuid(),
            Prenotazione.DataPrenotazione = DateTime.Today;
            Prenotazione.Studente = utente;
            Prenotazione.IdStudente = Guid.Parse(utente.Id);
            Prenotazione.IdEsame = exam.id;
            Prenotazione.Exam = exam;
                Prenotazione.ListaModuli = _unitOfWork.Moduli.GetModuliListForDropDown();
            */

            PrenotaEsameVM record = new PrenotaEsameVM
            {
                IdStudente = idUser,
                ListaEsami = _unitOfWork.Esami.VisualizzaEsami(),
                ListModuli = _unitOfWork.Moduli.GetModuliListForDropDown(),
            };
            //User_Studente_PrenotaEsame PE = _unitOfWork.Esami.ViewModel(utente., exam);



            return View(record);
            //return View(Prenotazione);

        }

        [HttpPost]
        public async Task<IActionResult> PrenotaEsameAutenticato_Post(PrenotaEsameVM Prenotazione)
        {

                if (!ModelState.IsValid)
                {
                // Se i dati non sono validi, restituisci la vista con gli errori
                return Content("errore");//View(Prenotazione);
                }

                // Se arrivi qui, i dati sono puliti e pronti per il database
                SchedulerEcdl Prenota = new SchedulerEcdl
                {
                    DataPrenotazione = DateTime.Today,
                    IdStudente = Prenotazione.IdStudente,
                    IdEsame = Prenotazione.IdEsame,
                    IdModulo = Guid.Parse(Prenotazione.ModuloSelezionato),
                    voto = -1 // Imposta un valore negativo per indicare che l'esame non è stato ancora sostenuto
                };

            int risultato = await _unitOfWork.SchedulerEcdl.PrenotazioneAsync(Prenota);

            return Content("Esame Prenotato");
        
        }


        [HttpPost]
        public async Task<IActionResult> PrenotaEsame_Post(EcdlBooking.ViewModel.User.PrenotaEsame Prenotazione)
        {




           
            SchedulerEcdl Prenota = new SchedulerEcdl
            {
                DataPrenotazione= DateTime.Today,
                IdStudente= Prenotazione.IdStudente,
                IdEsame= Prenotazione.IdEsame,
                IdModulo= Prenotazione.IdModulo,
            };

            var risultato = await _unitOfWork.SchedulerEcdl.PrenotazioneAsync(Prenota);

            return NoContent();
        }

        
        /*
                 //Tabella Delle Prenotazioni Di tutti Gli Studenti
    public Guid Id { get; set; }
    public DateTime  { get; set; }

    //relazione con studente
    public Guid  { get; set; }
    public ApplicationUser Studente { get; set; }


    public  Guid  { get; set; }
    public Exam Exam { get; set; }

    public Guid IdModulo { get; set; }
    public Modulo Modulo { get; set; }

    public float voto { get; set; }// numero negativo 
        */
        //if (ModelState.IsValid)

        // Ritorna la view con gli errori
        // inizia dell inserimento di una prenotazione da parte di uno scolaro
        /*
                        SchedulerEcdl EsameSchedule = new SchedulerEcdl
                        {
                            Studente = Prenotazione.Studente,
                            DataPrenotazione = Prenotazione.DataPrenotazione,
                            Exam = Prenotazione.Exam,
                            Modulo = Prenotazione.Modulo,
                            voto = 0
                        };


                        _unitOfWork.SchedulerEcdl.add(EsameSchedule);
                        _unitOfWork.Save();

                        PrenotaEsame_stampa(Prenotazione.Id);
                        //return View(Prenotazione);
                    }
        */
        // Dati validi: procedi con la logica
        //return RedirectToAction("Errore Nell Iseriemento");



        // if(Modelstate.is)
        //  User_Studente_PrenotaEsame prova = Prenotazione;
        //return null;




        // return RedirectToAction("Index");


        /*

        [HttpGet("{id}")]
        public IActionResult PrenotaEsame_stampa
            (Guid id)
        {

            SchedulerEcdl prenotazione = _db.SchedulerExams
                .Filter(s => s.Id == Guid.Parse(id.ToString()))
                .Include(s => s.Studente)
                .Include(s => s.Exam)
                .FirstOrDefault()

                ;
            return View (prenotazione); 
        }

        
        */

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
