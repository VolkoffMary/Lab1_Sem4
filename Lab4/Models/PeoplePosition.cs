using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab4
{
    public partial class PeoplePosition
    {
        public int PersonId { get; set; }
        public int PositionId { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Початок")]
        public DateTime Start { get; set; }

        [Display(Name = "Кінець")]
        public DateTime? Finish { get; set; }

        public virtual Person Person { get; set; } = null!;
        public virtual Position Position { get; set; } = null!;
    }
}
