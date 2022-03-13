using System;
using System.Collections.Generic;

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
        public string DepartmentName { get; set; } = null!;

        public virtual Faculty Faculty { get; set; } = null!;
        public virtual ICollection<Person> People { get; set; }
    }
}
