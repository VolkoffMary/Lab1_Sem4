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
    public class PeopleController : Controller
    {
        private readonly DBUniWorkersContext _context;

        public PeopleController(DBUniWorkersContext context)
        {
            _context = context;
        }

        // GET: People
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null)
                return RedirectToAction("Faculties", "Index");
            //Знаходження працівників за кафедрами
            ViewBag.DepartmentId = id;
            ViewBag.DepartmentName = name;
            var peopleByDepartment = _context.People.Where(x => x.DepartmentId == id).Include(x => x.Department);
            return View(await peopleByDepartment.ToListAsync());
        }

        // GET: People/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.People
                .Include(p => p.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
            //return RedirectToAction("Index", "Paychecks", new { id = person.Id, name = person.PersonName });
        }

        // GET: People/Create
        public IActionResult Create(int departmentId)
        {
            //ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "DepartmentName");
            ViewBag.DepartmentId = departmentId;
            ViewBag.DepartmentName = _context.Departments.Where(d => d.Id == departmentId).FirstOrDefault().DepartmentName;
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int departmentId, [Bind("Id,PersonName")] Person person)
        {
            person.DepartmentId = departmentId;
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "People", new { id = departmentId, name = _context.Departments.Where(c => c.Id == departmentId).FirstOrDefault().DepartmentName });
            }
            //ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "DepartmentName", person.DepartmentId);
            //return View(person);
            return RedirectToAction("Index", "People", new { id = departmentId, name = _context.Departments.Where(c => c.Id == departmentId).FirstOrDefault().DepartmentName });
        }

        // GET: People/Edit/5
        public async Task<IActionResult> Edit(int? id, int departmentId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.People.FindAsync(id);
            person.DepartmentId = departmentId;

            ViewBag.DepartmentId = person.DepartmentId;
            ViewBag.DepartmentName = _context.Departments.Where(d => d.Id == person.DepartmentId).FirstOrDefault().DepartmentName;

            if (person == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "DepartmentName", person.DepartmentId);
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DepartmentId,PersonName")] Person person)
        {
            if (id != person.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.Id))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "DepartmentName", person.DepartmentId);
            return View(person);
        }

        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.People
                .Include(p => p.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.People.FindAsync(id);
            _context.People.Remove(person);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return _context.People.Any(e => e.Id == id);
        }
    }
}
