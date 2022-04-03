using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab4
{
    public partial class Department
    {
        public Department()
        {
            People = new HashSet<Person>();
        }

        public int Id { get; set; }
        public int FacultyId { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Кафедра")]
        public string DepartmentName { get; set; } = null!;

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Факультет")]
        public virtual Faculty Faculty { get; set; } = null!;
        public virtual ICollection<Person> People { get; set; }
    }
}
