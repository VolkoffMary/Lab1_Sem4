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
    public class PaychecksController : Controller
    {
        private readonly DBUniWorkersContext _context;

        public PaychecksController(DBUniWorkersContext context)
        {
            _context = context;
        }

        // GET: Paychecks
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null)
                return RedirectToAction("Faculties", "Index");
            //Знаходження працівників за кафедрами
            ViewBag.PersonId = id;
            ViewBag.PersonName = name;
            var paychecksByPerson = _context.Paychecks.Where(p => p.PersonId == id).Include(p => p.Person);
            return View(await paychecksByPerson.ToListAsync());
        }

        // GET: Paychecks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paycheck = await _context.Paychecks
                .Include(p => p.Person)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paycheck == null)
            {
                return NotFound();
            }

            return View(paycheck);
        }

        // GET: Paychecks/Create
        public IActionResult Create(int personId)
        {
            //ViewData["PersonId"] = new SelectList(_context.People, "Id", "PersonName");
            ViewBag.PersonId = personId;
            ViewBag.PersonName = _context.People.Where(p => p.Id == personId).FirstOrDefault().PersonName;
            return View();
        }

        // POST: Paychecks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int personId, [Bind("Id,MonthDate,Salary")] Paycheck paycheck)
        {
            paycheck.PersonId = personId;
            if (ModelState.IsValid)
            {
                _context.Add(paycheck);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Paychecks", new { id = personId, name = _context.People.Where(p => p.Id == personId).FirstOrDefault().PersonName });
            }
            //ViewData["PersonId"] = new SelectList(_context.People, "Id", "PersonName", paycheck.PersonId);
            //return View(paycheck);
            return RedirectToAction("Index", "Paychecks", new { id = personId, name = _context.People.Where(p => p.Id == personId).FirstOrDefault().PersonName });
        }

        // GET: Paychecks/Edit/5
        public async Task<IActionResult> Edit(int? id, int personId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paycheck = await _context.Paychecks.FindAsync(id);

            ViewBag.PersonId = personId;
            ViewBag.PersonName = _context.People.Where(p => p.Id == personId).FirstOrDefault().PersonName;

            if (paycheck == null)
            {
                return NotFound();
            }
            ViewData["PersonId"] = new SelectList(_context.People, "Id", "PersonName", paycheck.PersonId);
            return View(paycheck);
        }

        // POST: Paychecks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PersonId,MonthDate,Salary")] Paycheck paycheck)
        {
            if (id != paycheck.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paycheck);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaycheckExists(paycheck.Id))
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
            ViewData["PersonId"] = new SelectList(_context.People, "Id", "PersonName", paycheck.PersonId);
            return View(paycheck);
        }

        // GET: Paychecks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paycheck = await _context.Paychecks
                .Include(p => p.Person)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paycheck == null)
            {
                return NotFound();
            }

            return View(paycheck);
        }

        // POST: Paychecks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paycheck = await _context.Paychecks.FindAsync(id);
            _context.Paychecks.Remove(paycheck);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaycheckExists(int id)
        {
            return _context.Paychecks.Any(e => e.Id == id);
        }
    }
}
