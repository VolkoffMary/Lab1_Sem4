using System;
using System.Collections.Generic;

namespace Lab4
{
    public partial class Position
    {
        public Position()
        {
            PeoplePositions = new HashSet<PeoplePosition>();
        }

        public int Id { get; set; }
        public string PositionName { get; set; } = null!;
        public decimal Salary { get; set; }

        public virtual ICollection<PeoplePosition> PeoplePositions { get; set; }
    }
}
