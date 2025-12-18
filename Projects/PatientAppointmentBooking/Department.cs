using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking
{
    // Department class
    public class Department
    {
        public Dictionary<string, List<Doctor>> Departments = new Dictionary<string, List<Doctor>>(StringComparer.OrdinalIgnoreCase);
        

        public void AddDoctor(string deptName, Doctor doctor)
        {
            if (!Departments.ContainsKey(deptName))
                Departments[deptName] = new List<Doctor>();
            Departments[deptName].Add(doctor);
        }
    }
}
