using System;
using System.Collections.Generic;

namespace Lab4
{
    public partial class PeoplePosition
    {
        public int PersonId { get; set; }
        public int PositionId { get; set; }
        public DateTime Start { get; set; }
        public DateTime? Finish { get; set; }

        public virtual Person Person { get; set; } = null!;
        public virtual Position Position { get; set; } = null!;
    }
}
