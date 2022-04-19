#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab4;

namespace Lab4.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly DBUniWorkersContext _context;

        public DepartmentsController(DBUniWorkersContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index(int? id, string name)
        {
            if (id == null || name == null)
            {
                var departments = _context.Departments.Include(x => x.Faculty);
                return View(await departments.ToListAsync());
            }

            //Знаходження кафедр за факультетами
            ViewBag.FacultyId = id;
            ViewBag.FacultyName = name;
            var departmentsByFaculty = _context.Departments.Where(x => x.FacultyId == id).Include(x => x.Faculty);
            return View(await departmentsByFaculty.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            //return View(department);
            return RedirectToAction("Index", "People", new { id = department.Id, name = department.DepartmentName });
        }

        // GET: Departments/Create
        public IActionResult Create(int? facultyId)
        {
            if (facultyId == null)
            {
                ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "FacultyName");
            }
            else
            {
                ViewBag.FacultyId = facultyId;
                ViewBag.FacultyName = _context.Faculties.Where(f => f.Id == facultyId).FirstOrDefault().FacultyName;
            }
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int facultyId, [Bind("Id,DepartmentName")] Department department)
        {
            department.FacultyId = facultyId;
            department.Faculty = await _context.Faculties.FindAsync(department.FacultyId);
            ModelState.ClearValidationState(nameof(department.Faculty));
            TryValidateModel(department.Faculty, nameof(department.Faculty));
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Departments", new { id = facultyId, name = _context.Faculties.Where(c => c.Id == facultyId).FirstOrDefault().FacultyName });
            }
            //ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "FacultyName", department.FacultyId);
            //return View(department);
            return RedirectToAction("Index", "Departments", new { id = facultyId, name = _context.Faculties.Where(c => c.Id == facultyId).FirstOrDefault().FacultyName });
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id, int? facultyId)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments.FindAsync(id);
            department.Faculty = await _context.Faculties.FindAsync(department.FacultyId);
            ViewBag.FacultyId = department.FacultyId;
            if (facultyId != null)
            {
                ViewBag.FacultyName = _context.Faculties.Where(f => f.Id == department.FacultyId).FirstOrDefault().FacultyName;
                ViewBag.FacultyId = facultyId;
            }
            else
            {
                ViewBag.FacultyName = null;
                ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "FacultyName", facultyId);
            }

            if (department == null)
                return NotFound();

            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FacultyId,DepartmentName")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            department.Faculty = await _context.Faculties.FindAsync(department.FacultyId);
            ModelState.ClearValidationState(nameof(department.Faculty));
            TryValidateModel(department.Faculty, nameof(department.Faculty));
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Departments", new { id = department.FacultyId, name = _context.Faculties.Where(f => f.Id == department.FacultyId).FirstOrDefault().FacultyName });
            }
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "FacultyName", department.FacultyId);
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id, int? facultyId)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments
                .Include(d => d.Faculty)
                .Include(d => d.People)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (facultyId != null)
                ViewBag.FacultyName = _context.Faculties.Where(f => f.Id == department.FacultyId).FirstOrDefault().FacultyName;
            else
                ViewBag.FacultyName = null;

            if (department == null)
                return NotFound();

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Departments", new { id = department.FacultyId, name = _context.Faculties.Where(f => f.Id == department.FacultyId).FirstOrDefault().FacultyName });
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
