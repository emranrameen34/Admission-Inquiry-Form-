using AdmissionInquiryRecord.Data;
using AdmissionInquiryRecord.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System;

namespace AdmissionInquiryRecord.Controllers
{
    public class CounsellingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CounsellingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= LOGIN =================

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var counsellor = _context.Counsellors
                .FirstOrDefault(c => c.CounsellorEmail == email && c.CounsellorPassword == password);

            if (counsellor != null)
            {
                HttpContext.Session.SetInt32("CounsellorId", counsellor.CounsellorId);
                HttpContext.Session.SetString("CounsellorName", counsellor.CounsellorName);
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid email or password!";
            return View();
        }

        // ================= DASHBOARD =================

        public IActionResult Dashboard()
        {
            var counsellorId = HttpContext.Session.GetInt32("CounsellorId");
            if (counsellorId == null) return RedirectToAction("Login");

            ViewBag.CounsellorName = HttpContext.Session.GetString("CounsellorName");

            // Only students assigned to this counsellor
            ViewBag.AssignedStudents = _context.Persons
                .Where(p => p.CounsellorId == counsellorId)
                .OrderByDescending(p => p.PersonId)
                .ToList();

            return View();
        }

        // ================= ASSIGN TO ME =================

        public IActionResult Assign(int personId)
        {
            var counsellorId = HttpContext.Session.GetInt32("CounsellorId");
            if (counsellorId == null) return RedirectToAction("Login");

            var person = _context.Persons.FirstOrDefault(p => p.PersonId == personId);
            if (person == null) return NotFound();

            if (person.CounsellorId == null || person.CounsellorId == 0)
            {
                person.CounsellorId = counsellorId.Value;
                _context.SaveChanges();

                TempData["Success"] = "Student successfully assigned to you.";
                return RedirectToAction("Dashboard");
            }

            if (person.CounsellorId == counsellorId)
            {
                TempData["Error"] = "This student is already assigned to you.";
                return RedirectToAction("Dashboard");
            }

            TempData["Error"] = "This student has already been assigned to another counsellor.";
            return RedirectToAction("Dashboard");
        }

        // ================= COUNSELLING FORM (GET) =================
        public IActionResult Create(int personId)
        {
            var counsellorId = HttpContext.Session.GetInt32("CounsellorId");
            if (counsellorId == null) return RedirectToAction("Login");

            var person = _context.Persons.FirstOrDefault(p => p.PersonId == personId);
            if (person == null) return NotFound();

            // <<<==== Safety check: ensure student is assigned to this counsellor
            if (person.CounsellorId != counsellorId)
            {
                TempData["Error"] = "This student is not assigned to you yet!";
                return RedirectToAction("UnassignedStudents");
            }

            // Create counselling model
            var model = new CounsellingRecord
            {
                PersonId = person.PersonId,
                CounsellorId = counsellorId.Value,
                VisitDate = DateTime.Now
            };

            // Three buttons for navigation
            ViewBag.NewStudentsButton = Url.Action("UnassignedStudents");
            ViewBag.CounselledButton = Url.Action("AlreadyCounselled");
            ViewBag.AssignedButton = Url.Action("AssignedStudents");

            ViewBag.PersonName = person.Name;
            ViewBag.CounsellorName = HttpContext.Session.GetString("CounsellorName");
            ViewBag.ContactMethods = new[] { "Call", "WhatsApp", "Email" };
            ViewBag.StatusOptions = new[] { "Converted", "Considering", "Not Interested", "No Response" };

            // ================= STUDENT HISTORY =================

            var history = _context.CounsellingRecords
                .Where(c => c.PersonId == personId)
                .OrderByDescending(c => c.VisitDate)
                .ToList();

            ViewBag.StudentHistory = history;
            ViewBag.TotalVisits = history.Count;
            ViewBag.LastVisit = history.FirstOrDefault();

            return View(model);
        }

        // ================= COUNSELLING FORM (POST) =================

        [HttpPost]
        public IActionResult Create(CounsellingRecord model)
        {
            var counsellorId = HttpContext.Session.GetInt32("CounsellorId");
            if (counsellorId == null) return RedirectToAction("Login");

            var person = _context.Persons.FirstOrDefault(p => p.PersonId == model.PersonId);
            if (person == null) return NotFound();

            if (person.CounsellorId != counsellorId)
            {
                TempData["Error"] = "You are not allowed to counsel this student.";
                return RedirectToAction("Dashboard");
            }

            if (ModelState.IsValid)
            {
                // Check if student already has counselling record
                var existingRecord = _context.CounsellingRecords
                    .Where(c => c.PersonId == model.PersonId)
                    .OrderByDescending(c => c.CounsellingId)
                    .FirstOrDefault();

                // Add new record regardless, handle repeated student logic
                model.CounsellorId = counsellorId.Value;
                model.VisitDate = DateTime.Now;

                _context.CounsellingRecords.Add(model);
                _context.SaveChanges();

                // Generate counselling code
                string sessionCode = person.SessionId == 1 ? "FL" : "SP";
                string datePart = model.VisitDate.ToString("ddMMyy");
                string counsellingCode = $"{sessionCode}-{model.CounsellingId}-{datePart}";

                TempData["CounsellingCode"] = counsellingCode;
                TempData["PersonName"] = person.Name;
                TempData["EntryDate"] = model.VisitDate.ToString("dd-MM-yyyy HH:mm");

                return RedirectToAction("Success");
            }

            ViewBag.PersonName = person.Name;
            ViewBag.CounsellorName = HttpContext.Session.GetString("CounsellorName");
            ViewBag.ContactMethods = new[] { "Call", "WhatsApp", "Email" };
            ViewBag.StatusOptions = new[] { "Converted", "Considering", "Not Interested", "No Response" };

            return View(model);
        }

        // ================= SUCCESS PAGE =================

        public IActionResult Success()
        {
            return View();
        }

        // ================= LOGOUT =================

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ================= NEW ACTIONS =================

        // Show assigned students
        public IActionResult AssignedStudents()
        {
            var counsellorId = HttpContext.Session.GetInt32("CounsellorId");
            if (counsellorId == null) return RedirectToAction("Login");

            var students = _context.Persons
                .Where(p => p.CounsellorId == counsellorId)
                .OrderByDescending(p => p.PersonId)
                .ToList();

            return View(students);
        }

        // Show unassigned students
        public IActionResult UnassignedStudents()
        {
            var students = _context.Persons
                .Where(p => p.CounsellorId == 0 || p.CounsellorId == null)
                .OrderByDescending(p => p.PersonId)
                .ToList();

            return View(students);
        }

        // Show already counselled students
        public IActionResult AlreadyCounselled()
        {
            var counsellorId = HttpContext.Session.GetInt32("CounsellorId");
            if (counsellorId == null) return RedirectToAction("Login");

            var records = _context.CounsellingRecords
                .Include(c => c.Person)
                .Where(c => c.CounsellorId == counsellorId)
                .OrderByDescending(c => c.CounsellingId)
                .ToList();

            return View(records);
        }
    }
}