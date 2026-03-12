using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AdmissionInquiryRecord.Data;
using AdmissionInquiryRecord.Models;
using System.Linq;
using System.Collections.Generic;

namespace AdmissionInquiryRecord.Controllers
{
    public class PersonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PersonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========================= CREATE FORM (GET) =========================
        public IActionResult Create()
        {
            var model = new Person
            {
                EntryDate = System.DateTime.Now
            };

            LoadDropdowns();
            LoadLearnSources();
            LoadCounsellors(); // new
            return View(model);
        }

        // ========================= CREATE FORM (POST) =========================
        [HttpPost]
        public IActionResult Create(Person model, List<int> SelectedSources)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                LoadLearnSources();
                LoadCounsellors();
                return View(model);
            }

            var existingPerson = _context.Persons.FirstOrDefault(p => p.CNIC == model.CNIC);

            if (existingPerson != null)
            {
                TempData["PersonId"] = existingPerson.PersonId;
                TempData["PersonName"] = existingPerson.Name;
                TempData["UserExists"] = true;

                if (model.CounsellorId != null)
                    existingPerson.CounsellorId = model.CounsellorId;

                _context.SaveChanges();
                return RedirectToAction("Success");
            }
            else
            {
                model.EntryDate = System.DateTime.Now;

                var lastVisit = _context.Persons.OrderByDescending(p => p.PersonId).FirstOrDefault();
                model.VisitNo = lastVisit == null ? 1 : lastVisit.VisitNo + 1;

                if (SelectedSources != null && SelectedSources.Count > 0)
                    model.HowDidCandidate = string.Join(",", SelectedSources);

                _context.Persons.Add(model);
                _context.SaveChanges();

                TempData["PersonId"] = model.PersonId;
                TempData["PersonName"] = model.Name;
                TempData["UserExists"] = false;

                return RedirectToAction("Success");
            }
        }

        public IActionResult AssignToCounsellor(int personId)
        {
            var counsellorId = HttpContext.Session.GetInt32("CounsellorId");
            if (counsellorId == null) return RedirectToAction("Login");

            var person = _context.Persons.FirstOrDefault(p => p.PersonId == personId);
            if (person == null) return NotFound();

            if (person.CounsellorId == null || person.CounsellorId == 0)
            {
                person.CounsellorId = counsellorId.Value;
                _context.SaveChanges();
            }

            return RedirectToAction("UnassignedStudents");
        }

        // ========================= DROPDOWNS =========================
        private void LoadDropdowns()
        {
            ViewBag.Cities = new SelectList(_context.Cities, "CityId", "CityName");
            ViewBag.Districts = new SelectList(_context.Districts, "DistrictId", "DistrictName");
            ViewBag.Tehsils = new SelectList(_context.Tehsils, "TehsilId", "TehsilName");
            ViewBag.Programs = new SelectList(_context.Programs, "ProgramId", "ProgramName");
            ViewBag.Sessions = new SelectList(_context.Sessions, "SessionId", "SessionName");
        }

        private void LoadLearnSources()
        {
            ViewBag.LearnSources = _context.LearnSources.ToList();
        }

        private void LoadCounsellors()
        {
            ViewBag.Counsellors = new SelectList(_context.Counsellors.OrderBy(c => c.CounsellorName), "CounsellorId", "CounsellorName");
        }

        public IActionResult Success()
        {
            return View();
        }

        [HttpGet] 
        public JsonResult GetPersonByCNIC(string cnic)
        { 
            var person = _context.Persons
                .Where(p => p.CNIC == cnic)
                .Select(p => new 
                { 
                    p.PersonId,
                    p.Name, 
                    p.CellNo,
                    p.WhatsAppNo,
                    p.CityId, p.TehsilId,
                    p.DistrictId,
                    p.ProgramInterestedId,
                    p.AlternativeProgramId, 
                    p.SessionId,
                    p.ReferredBy,
                    p.NotReferred,
                    p.HowDidCandidate,
                    p.CounsellorId
                })
                .FirstOrDefault();

            return Json(person); 
        }
    }
}