using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab4
{
    public partial class Position
    {
        public Position()
        {
            PeoplePositions = new HashSet<PeoplePosition>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Назва посади")]
        public string PositionName { get; set; } = null!;

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Посадовий оклад")]
        public decimal Salary { get; set; }

        public virtual ICollection<PeoplePosition> PeoplePositions { get; set; }
    }
}
