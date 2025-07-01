using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zadanie_5.Data;
using Zadanie_5.Models;

namespace Zadanie_5.Controllers
{
    public class TestController : Controller
    {
        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Klient> klienci = await _context.Klienci.ToListAsync();
            return View(klienci);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Klient klient)
        {
            if (ModelState.IsValid)
            {
                _context.Klienci.Add(klient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(klient);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var klient = await _context.Klienci.FindAsync(id);
            if (klient == null)
            {
                return NotFound();
            }
            return View(klient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Klient klient)
        {
            if (id != klient.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(klient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KlientExists(klient.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(klient);
        }

        private bool KlientExists(int id)
        {
            return _context.Klienci.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var klient = await _context.Klienci.FirstOrDefaultAsync(m => m.Id == id);
            if (klient == null)
                return NotFound();

            return View(klient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var klient = await _context.Klienci.FindAsync(id);
            if (klient != null)
            {
                _context.Klienci.Remove(klient);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Import()
        {
            var model = new Import
            {
                DataSources = new List<SelectListItem>
            {
                new SelectListItem { Value = "CSV", Text = "CSV" },
                new SelectListItem { Value = "XLSX", Text = "XLSX" }
            }
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(Import model)
        {
            if (!ModelState.IsValid)
            {
                model.DataSources = new List<SelectListItem>
            {
                new SelectListItem { Value = "CSV", Text = "CSV" },
                new SelectListItem { Value = "XLSX", Text = "XLSX" }
            };
                return View(model);
            }

            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("File", "Proszę wybrać plik do importu.");
                model.DataSources = new List<SelectListItem>
            {
                new SelectListItem { Value = "CSV", Text = "CSV" },
                new SelectListItem { Value = "XLSX", Text = "XLSX" }
            };
                return View(model);
            }

            if (model.DataSource == "CSV")
            {
                using (var stream = model.File.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var parts = line.Split(',');
                        if (parts.Length < 3)
                        {

                            continue;
                        }

                        string name = parts[0].Trim();
                        string surname = parts[1].Trim();
                        string pesel = parts[2].Trim();

                        var (birthYear, gender) = ParsePesel(pesel);

                        var klient = new Klient
                        {
                            Name = name,
                            Surname = surname,
                            PESEL = pesel,
                            BirthYear = birthYear,
                            Gender = gender
                        };

                        if(klient.Name == "Name")
                        {
                            continue;
                        }    

                        _context.Klienci.Add(klient);
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Import zakończony pomyślnie.";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Obsługiwany jest tylko format CSV.");
                model.DataSources = new List<SelectListItem>
            {
                new SelectListItem { Value = "CSV", Text = "CSV" },
                new SelectListItem { Value = "XLSX", Text = "XLSX" }
            };
                return View(model);
            }
        }

        private (int birthYear, int gender) ParsePesel(string pesel)
        {
            if (pesel.Length != 11)
                return (0, 0);

            int year = int.Parse(pesel.Substring(0, 2));
            int month = int.Parse(pesel.Substring(2, 2));
            int day = int.Parse(pesel.Substring(4, 2));

            int century = 1900;
            if (month > 80)
            {
                century = 1800;
                month -= 80;
            }
            else if (month > 60)
            {
                century = 2200;
                month -= 60;
            }
            else if (month > 40)
            {
                century = 2100;
                month -= 40;
            }
            else if (month > 20)
            {
                century = 2000;
                month -= 20;
            }

            int fullYear = century + year;

            int genderDigit = int.Parse(pesel.Substring(9, 1));
            int gender = (genderDigit % 2 == 0) ? 1 : 0;

            return (fullYear, gender);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export(string exportFormat)
        {
            var klienci = await _context.Klienci.ToListAsync();

            if (string.IsNullOrEmpty(exportFormat))
            {
                TempData["ErrorMessage"] = "Proszę wybrać format eksportu.";
                return RedirectToAction(nameof(Index));
            }

            if (exportFormat == "CSV")
            {
                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("Name,Surname,PESEL,BirthYear,Gender");

                foreach (var k in klienci)
                {
                    csvBuilder.AppendLine($"{k.Name},{k.Surname},{k.PESEL},{k.BirthYear},{k.Gender}");
                }

                var bytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
                return File(bytes, "text/csv", "klienci.csv");
            }
            else if (exportFormat == "XLSX")
            {
                // Placeholder – jeszcze nieobsługiwany format, wyświetlamy komunikat i wracamy do Index
                TempData["ErrorMessage"] = "Eksport do XLSX nie jest jeszcze dostępny.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Nieobsługiwany format eksportu.";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
