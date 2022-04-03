using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Людина")]
        public string PersonName { get; set; } = null!;

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Кафедра")]
        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<Paycheck> Paychecks { get; set; }
        public virtual ICollection<PeoplePosition> PeoplePositions { get; set; }
    }
}
