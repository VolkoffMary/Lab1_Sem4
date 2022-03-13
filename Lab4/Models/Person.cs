using System;
using System.Collections.Generic;

namespace Lab4
{
    public partial class Person
    {
        public Person()
        {
            Paychecks = new HashSet<Paycheck>();
            PeoplePositions = new HashSet<PeoplePosition>();
        }

        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string PersonName { get; set; } = null!;

        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<Paycheck> Paychecks { get; set; }
        public virtual ICollection<PeoplePosition> PeoplePositions { get; set; }
    }
}
