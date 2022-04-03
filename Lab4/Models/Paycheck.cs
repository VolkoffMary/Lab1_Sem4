using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab4
{
    public partial class Paycheck
    {
        public int Id { get; set; }
        public int PersonId { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Дата")]
        public DateTime MonthDate { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Заробітня плата")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Працівник")]
        public virtual Person Person { get; set; } = null!;
    }
}
