using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly DBUniWorkersContext _context;
        public ChartController(DBUniWorkersContext context)
        { 
            _context = context; 
        }

        [HttpGet("JsonDataPeopleByDepartment")]
        public JsonResult JsonDataPeopleByDepartment()
        {
            var departments = _context.Departments.ToList();
            List<object> peopleByDepartment = new List<object>();
            peopleByDepartment.Add(new[] { "Кафедра", "Кількість працівників" });
            foreach (var d in departments)
            {
                d.People = _context.People.Where(p => p.DepartmentId == d.Id).ToList();
                peopleByDepartment.Add(new object[] { d.DepartmentName, d.People.Count() });
            }
            return new JsonResult(peopleByDepartment);
        }

        [HttpGet("JsonDataSalaryPerPosition")]
        public JsonResult JsonDataSalaryPerPosition()
        {
            var positions = _context.Positions.ToList();
            List<object> salaryPerPosition = new List<object>();
            salaryPerPosition.Add(new[] { "Посада", "Зарплата" });
            foreach (var p in positions)
            {
                salaryPerPosition.Add(new object[] { p.PositionName, p.Salary });
            }
            return new JsonResult(salaryPerPosition);
        }
    }
}
