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
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null)
                return RedirectToAction("Faculties", "Index");
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
        public IActionResult Create(int facultyId)
        {
            //ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "FacultyName");
            ViewBag.FacultyId = facultyId;
            ViewBag.FacultyName = _context.Faculties.Where(f => f.Id == facultyId).FirstOrDefault().FacultyName;
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
        public async Task<IActionResult> Edit(int? id, int facultyId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            department.FacultyId = facultyId;

            ViewBag.FacultyId = department.FacultyId;
            ViewBag.FacultyName = _context.Faculties.Where(f => f.Id == department.FacultyId).FirstOrDefault().FacultyName;

            if (department == null)
            {
                return NotFound();
            }
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "FacultyName", facultyId);
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int facultyId, [Bind("Id,DepartmentName")] Department department)
        {
            department.FacultyId = facultyId;
            if (id != department.Id)
            {
                return NotFound();
            }

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
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "FacultyName", department.FacultyId);
            return View(department);
            //return RedirectToAction("Index", "Departments", new { id = department.FacultyId, name = _context.Faculties.Where(f => f.Id == department.FacultyId).FirstOrDefault().FacultyName });
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
