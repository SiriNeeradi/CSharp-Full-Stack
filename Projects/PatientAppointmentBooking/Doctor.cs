using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking
{
    // Doctor class (auto-registers into Department)
    public class Doctor
    {
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public List<Slot> Slots { get; private set; } = new List<Slot>();

        public Doctor(string name, string departmentName, Department departmentStore)
        {
            Name = name;
            DepartmentName = departmentName;
            departmentStore.AddDoctor(departmentName, this);
        }

        public void SetAvailability(TimeOnly startTime,TimeOnly endTime,DateOnly date)
        {
            Slots.Clear();
            TimeOnly current = startTime;
            while (true)
            {
                TimeOnly slotEnd = current.AddMinutes(30);
                if (slotEnd > endTime) break;
                Slots.Add(new Slot(current, slotEnd,date));
                current = slotEnd;
            }
        }
    }
}
