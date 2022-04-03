﻿#nullable disable
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
    public class PeoplePositionsController : Controller
    {
        private readonly DBUniWorkersContext _context;

        public PeoplePositionsController(DBUniWorkersContext context)
        {
            _context = context;
        }

        // GET: PeoplePositions
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null)
                return RedirectToAction("Faculties", "Index");
            //Знаходження працівників за кафедрами
            ViewBag.PersonId = id;
            ViewBag.PersonName = name;
            var personPositionsByPerson = _context.PeoplePositions.Where(p => p.PersonId == id).Include(p => p.Person).Include(p => p.Position);
            return View(await personPositionsByPerson.ToListAsync());
        }

        // GET: PeoplePositions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var peoplePosition = await _context.PeoplePositions
                .Include(p => p.Person)
                .Include(p => p.Position)
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (peoplePosition == null)
            {
                return NotFound();
            }

            return View(peoplePosition);
        }

        // GET: PeoplePositions/Create
        public IActionResult Create(int personId)
        {
            //ViewData["PersonId"] = new SelectList(_context.People, "Id", "PersonName");
            //ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Id");
            ViewBag.PersonId = personId;
            ViewBag.PersonName = _context.People.Where(p => p.Id == personId).FirstOrDefault().PersonName;
            return View();
        }

        // POST: PeoplePositions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int personId, [Bind("PositionId,Start,Finish")] PeoplePosition peoplePosition)
        {
            peoplePosition.PersonId = personId;
            if (ModelState.IsValid)
            {
                _context.Add(peoplePosition);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "PeoplePositions", new { id = personId, name = _context.People.Where(p => p.Id == personId).FirstOrDefault().PersonName });
            }
            //ViewData["PersonId"] = new SelectList(_context.People, "Id", "PersonName", peoplePosition.PersonId);
            //ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Id", peoplePosition.PositionId);
            //return View(peoplePosition);
            return RedirectToAction("Index", "PeoplePositions", new { id = personId, name = _context.People.Where(p => p.Id == personId).FirstOrDefault().PersonName });
        }

        // GET: PeoplePositions/Edit/5
        public async Task<IActionResult> Edit(int personId, int positionId)
        {
            /*if (id == null)
            {
                return NotFound();
            }*/

            var peoplePosition = await _context.PeoplePositions.FindAsync(personId, positionId);
            if (peoplePosition == null)
            {
                return NotFound();
            }
            //ViewData["PersonId"] = new SelectList(_context.People, "Id", "PersonName", peoplePosition.PersonId);
            //ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Id", peoplePosition.PositionId);
            ViewBag.PersonId = personId;
            ViewBag.PositionId = positionId;
            ViewBag.PersonName = _context.People.Where(p => p.Id == personId).FirstOrDefault().PersonName;
            return View(peoplePosition);
        }

        // POST: PeoplePositions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonId,PositionId,Start,Finish")] PeoplePosition peoplePosition)
        {
            if (id != peoplePosition.PersonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(peoplePosition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PeoplePositionExists(peoplePosition.PersonId))
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
            ViewData["PersonId"] = new SelectList(_context.People, "Id", "PersonName", peoplePosition.PersonId);
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Id", peoplePosition.PositionId);
            return View(peoplePosition);
        }

        // GET: PeoplePositions/Delete/5
        public async Task<IActionResult> Delete(int personId, int positionId)
        {
            /*if (personId == null || positionId == null)
            {
                return NotFound();
            }*/

            var peoplePosition = await _context.PeoplePositions
                .Include(p => p.Person)
                .Include(p => p.Position)
                .FirstOrDefaultAsync(m => m.PersonId == personId && m.PositionId == positionId);
            if (peoplePosition == null)
            {
                return NotFound();
            }

            return View(peoplePosition);
        }

        // POST: PeoplePositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var peoplePosition = await _context.PeoplePositions.FindAsync(id);
            _context.PeoplePositions.Remove(peoplePosition);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PeoplePositionExists(int id)
        {
            return _context.PeoplePositions.Any(e => e.PersonId == id);
        }
    }
}
