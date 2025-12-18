using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking
{
    public class Patient
    {
        public string Name;
        public string Department;
        public string DoctorName;
        public readonly int uniqueNumber;
        public DateOnly slotdate;
        public TimeOnly End;
        public TimeOnly Start;
        



        public Patient(string name, string department, int uniqueNumber,string DoctorName,DateOnly slotTime,TimeOnly start,TimeOnly end)
        {
            Name = name;
            Department = department;
            this.uniqueNumber = uniqueNumber;
            this.DoctorName = DoctorName;
            slotdate = slotTime;
            Start = start;
            End = end;
            

        }
        public Patient(string name, string department, int uniqueNumber)
        {
            Name = name;
            Department = department;
            this.uniqueNumber = uniqueNumber;
          


        }
    }
}
