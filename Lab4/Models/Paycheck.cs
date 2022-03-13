using System;
using System.Collections.Generic;

namespace Lab4
{
    public partial class Paycheck
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public DateTime MonthDate { get; set; }
        public decimal Salary { get; set; }

        public virtual Person Person { get; set; } = null!;
    }
}
