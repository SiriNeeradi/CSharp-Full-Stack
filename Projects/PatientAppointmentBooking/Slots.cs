using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking
{
    public class Slot
    {
        public DateOnly date {  get; set; }
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }

        public Slot(TimeOnly start, TimeOnly end, DateOnly date)
        {
            Start = start;
            End = end;
            this.date = date;
        }

        public override string ToString()
        {
            return $"{date}  {Start:HH:mm} - {End:HH:mm}";
        }
    }
}
