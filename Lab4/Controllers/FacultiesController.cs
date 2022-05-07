#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab4;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO.Compression;


namespace Lab4.Controllers
{
    public class FacultiesController : Controller
    {
        private readonly ILogger<Faculty> _logger;
        private readonly DBUniWorkersContext _context;

        public FacultiesController(DBUniWorkersContext context)
        {
            _context = context;
        }

        // GET: Faculties
        public async Task<IActionResult> Index()
        {
            return View(await _context.Faculties.ToListAsync());
        }

        // GET: Faculties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (faculty == null)
            {
                return NotFound();
            }

            //return View(faculty);
            return RedirectToAction("Index", "Departments", new { id = faculty.Id, name = faculty.FacultyName });
        }

        // GET: Faculties/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Faculties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FacultyName")] Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                _context.Add(faculty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(faculty);
        }

        // GET: Faculties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null)
            {
                return NotFound();
            }
            return View(faculty);
        }

        // POST: Faculties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FacultyName")] Faculty faculty)
        {
            if (id != faculty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faculty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyExists(faculty.Id))
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
            return View(faculty);
        }

        // GET: Faculties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var faculty = await _context.Faculties
                .Include(b => b.Departments)
                .FirstOrDefaultAsync(m => m.Id == id);


            if (faculty == null)
                return NotFound();

            return View(faculty);
        }

        // POST: Faculties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var faculty = await _context.Faculties.FindAsync(id);
            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacultyExists(int id)
        {
            return _context.Faculties.Any(e => e.Id == id);
        }
        //  <===========================================================>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcel(IFormFile dlFileExcel)
        {
            if (ModelState.IsValid)
            {
                if (dlFileExcel != null)
                {
                    using (var stream = new FileStream(dlFileExcel.FileName, FileMode.Create))
                    {
                        await dlFileExcel.CopyToAsync(stream);
                        using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
                        {
                            //перегляд ycix листів (в даному випадку категорій)
                            foreach (IXLWorksheet worksheet in workBook.Worksheets)
                            {
                                //worksheet.Name - назва категорії. Пробусмо знайти в БД, якщо відсутня, то створюсмо нову
                                string facName = worksheet.Cell("B1").Value.ToString();
                                Faculty newfac;
                                var f = (from fac in _context.Faculties
                                         where fac.FacultyName.Contains(facName)
                                         select fac).ToList();
                                if (f.Count > 0)
                                    newfac = f[0];
                                else
                                {
                                    newfac = new Faculty();
                                    newfac.FacultyName = facName;
                                    //додати в контекст
                                    _context.Faculties.Add(newfac);
                                }

                                //у разі наявності - знайти, у разі відсутності - додати
                                foreach (IXLRow row in worksheet.RowsUsed().Skip(2))
                                {
                                    try
                                    {   
                                        IXLCell firstCell = row.FirstCellUsed();
                                        string depName = firstCell.Value.ToString();
                                        Department newdep;
                                        var d = (from dep in _context.Departments
                                                 where dep.DepartmentName.Contains(depName)
                                                 select dep).ToList();
                                        if (d.Count > 0)
                                            newdep = d[0];
                                        else
                                        {
                                            newdep = new Department();
                                            newdep.DepartmentName = depName;
                                            newdep.Faculty = newfac;
                                            _context.Departments.Add(newdep);
                                        }

                                        //у разі наявності - знайти, у разі відсутності - додати
                                        foreach (IXLCell cell in row.CellsUsed().Skip(1))
                                        {
                                            string cellValue = cell.Value.ToString();
                                            if (cellValue.Length > 0)
                                            {
                                                Person person;
                                                var p = (from per in _context.People
                                                         where per.PersonName.Contains(cellValue)
                                                         select per).ToList();
                                                if (p.Count > 0)
                                                    person = p[0];
                                                else
                                                { 
                                                    person = new Person();
                                                    person.PersonName = cellValue;
                                                    person.Department = newdep;
                                                    //додати в контекст
                                                    _context.Add(person);
                                                }
                                            }
                                        }    
                                    }
                                    catch (Exception)
                                    {
                                        _logger.LogError("Failed to add departments or people");
                                    }
                                }
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        //  <===========================================================>

        public ActionResult ExportExcel()
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var faculties = _context.Faculties.Include("Departments").ToList();
                //тут, для прикладу ми пишемо yci книжки з БД, в своїх просктах ТАК НЕ РОБИТИ (писати лише вибрані)
                foreach (var f in faculties)
                {
                    string facNameFull = f.FacultyName;
                    string facNameShort = (facNameFull.Length > 31) ? facNameFull.Substring(0, 31) : facNameFull;

                    var worksheet = workbook.Worksheets.Add(facNameShort);
                    worksheet.Cell("A1").Value = "Повна назва факультету:";
                    worksheet.Cell("B1").Value = facNameFull;
                    worksheet.Cell("A2").Value = "Кафедра";
                    worksheet.Cell("B2").Value = "Працівник";
                    worksheet.Row(1).Style.Font.Bold = true;
                    worksheet.Row(2).Style.Font.Bold = true;

                    var departments = f.Departments.ToList();

                    //нумерація рядків/стовпчиків починасться з індекса 1 (не 0)
                    int maxColumn = 2;
                    for (int i=0; i<departments.Count;i++)
                    {
                        worksheet.Cell(i + 3, 1).Value = departments[i].DepartmentName;
                        var people = _context.People.Where(p => p.DepartmentId == departments[i].Id).ToList();

                        int j = 2;
                        foreach (var p in people)
                        {
                            worksheet.Cell(i + 3, j).Value = p.PersonName;
                            if (maxColumn < j)
                            {
                                maxColumn = j;
                                worksheet.Cell(2, maxColumn).Value = "Працівник";
                            }
                            j++;
                        }
                    }
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"FacDepPer_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }

        //  <===========================================================>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportWord(IFormFile dlFileWord)
        {
            if (ModelState.IsValid)
            {
                if (dlFileWord != null)
                {
                    using (var stream = new FileStream(dlFileWord.FileName, FileMode.Create))
                    {
                        await dlFileWord.CopyToAsync(stream);

                        string xml = string.Empty;
                        stream.Position = 0;
                        using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, false))
                        {
                            Body docBody = doc.MainDocumentPart.Document.Body; // тело документа (размеченный текст без стилей)

                            Faculty lastFaculty = null;
                            Department lastDepartment = null;
                            foreach (var el in docBody.ChildElements)
                            {
                                if (el.GetFirstChild<ParagraphProperties>() != null)
                                {
                                    if (el.GetFirstChild<ParagraphProperties>().GetFirstChild<NumberingProperties>() == null)
                                    {
                                        continue;
                                    }
                                    int level = el.GetFirstChild<ParagraphProperties>().GetFirstChild<NumberingProperties>().GetFirstChild<NumberingLevelReference>().Val;
                                    string name = el.GetFirstChild<Run>().InnerText;

                                    switch (level)
                                    {
                                        case 0:
                                            Faculty newfac;
                                            var f = (from fac in _context.Faculties
                                                     where fac.FacultyName.Contains(name)
                                                     select fac).ToList();
                                            if (f.Count > 0)
                                                newfac = f[0];
                                            else
                                            {
                                                newfac = new Faculty();
                                                newfac.FacultyName = name;
                                                _context.Faculties.Add(newfac);
                                            }
                                            lastFaculty = newfac;
                                            break;
                                        case 1:
                                            Department newdep;
                                            var d = (from dep in _context.Departments
                                                     where dep.DepartmentName.Contains(name)
                                                     select dep).ToList();
                                            if (d.Count > 0)
                                                newdep = d[0];
                                            else
                                            {
                                                newdep = new Department();
                                                newdep.DepartmentName = name;
                                                newdep.Faculty = lastFaculty;
                                                _context.Departments.Add(newdep);
                                            }
                                            lastDepartment = newdep;
                                            break;
                                        case 2:
                                            Person person;
                                            var p = (from per in _context.People
                                                     where per.PersonName.Contains(name)
                                                     select per).ToList();
                                            if (p.Count > 0)
                                                person = p[0];
                                            else
                                            {
                                                person = new Person();
                                                person.PersonName = name;
                                                person.Department = lastDepartment;
                                                _context.Add(person);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));       
        }

        //  <===========================================================>

        public ActionResult ExportWord()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (WordprocessingDocument package = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
                {
                    var faculties = _context.Faculties.ToList();

                    MainDocumentPart mainPart = package.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    var body = new Body();
                    
                    
                    NumberingDefinitionsPart numberingPart =
                        mainPart.AddNewPart<NumberingDefinitionsPart>("someUniqueIdHere");

                    Numbering element =
                        new Numbering(
                            new AbstractNum(
                                new Level(
                                    new NumberingFormat() { Val = NumberFormatValues.Bullet },
                                    new LevelText() { Val = "•" }
                                )
                                { LevelIndex = 0 },
                                new Level(
                                    new NumberingFormat() { Val = NumberFormatValues.Bullet },
                                    new LevelText() { Val = "       ○" }
                                )
                                { LevelIndex = 1 },
                                new Level(
                                    new NumberingFormat() { Val = NumberFormatValues.Bullet },
                                    new LevelText() { Val = "               ○" }
                                )
                                { LevelIndex = 2 }
                            )
                            { AbstractNumberId = 1 },
                            new NumberingInstance(
                              new AbstractNumId() { Val = 1 }
                            )
                            { NumberID = 1 }
                        );
                    element.Save(numberingPart);

                    foreach (var faculty in faculties)
                    {
                        
                        Run runFacultyName = new Run(); 
                        Paragraph facultyName = new Paragraph();
                        facultyName.ParagraphProperties = 
                            new ParagraphProperties(
                                new NumberingProperties(
                                    new NumberingLevelReference() { Val = 0 },
                                    new NumberingId() { Val = 1 }
                                    )
                                );

                        var dep = _context.Departments.Where(a => a.FacultyId == faculty.Id).ToList();

                        RunProperties runHeaderProperties = runFacultyName.AppendChild(new RunProperties(new Bold()));
                        RunProperties runProperties = runFacultyName.AppendChild(new RunProperties(new Italic()));

                        runFacultyName.AppendChild(new Text($"{faculty.FacultyName}") { Space = SpaceProcessingModeValues.Default });
                        facultyName.Append(runFacultyName);
                        body.Append(facultyName);

                        if (dep.Count() > 0)
                        {
                            foreach (var d in dep)
                            {
                                var people = _context.People.Where(p => p.DepartmentId == d.Id).ToList();

                                Run runDepartmentName = new Run();
                                Paragraph departmentName = new Paragraph();
                                departmentName.ParagraphProperties =
                                    new ParagraphProperties(
                                        new NumberingProperties(
                                            new NumberingLevelReference() { Val = 1 },
                                            new NumberingId() { Val = 1 }
                                            )
                                        );

                                runDepartmentName.AppendChild(new Text($"{d.DepartmentName}") { Space = SpaceProcessingModeValues.Default });
                                departmentName.Append(runDepartmentName);
                                body.Append(departmentName);

                                if (people.Count() > 0)
                                {
                                    foreach (var person in people)
                                    {
                                        Run runPersonName = new Run();
                                        Paragraph personName = new Paragraph();
                                        personName.ParagraphProperties =
                                            new ParagraphProperties(
                                                new NumberingProperties(
                                                    new NumberingLevelReference() { Val = 2 },
                                                    new NumberingId() { Val = 1 }
                                                    )
                                                );

                                        runPersonName.AppendChild(new Text($"{person.PersonName}") { Space = SpaceProcessingModeValues.Default });
                                        personName.Append(runPersonName);
                                        body.Append(personName);
                                    }
                                }
                            }
                        }
                    }

                    mainPart.Document.Append(body);
                    package.Close();
                }

                

                ms.Flush();
                return new FileContentResult(ms.ToArray(), "application/vnd.ms-word")
                {
                    //змініть назву файла відповідно до тематики Вашого проєкту
                    FileDownloadName = $"FacDepPer_{DateTime.UtcNow.ToShortDateString()}.docx"
                };
            }
        }

        //  <===========================================================>
    }
}
