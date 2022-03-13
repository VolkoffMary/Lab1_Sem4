using System;
using System.Collections.Generic;

namespace Lab4
{
    public partial class Faculty
    {
        public Faculty()
        {
            Departments = new HashSet<Department>();
        }

        public int Id { get; set; }
        public string FacultyName { get; set; } = null!;

        public virtual ICollection<Department> Departments { get; set; }
    }
}
