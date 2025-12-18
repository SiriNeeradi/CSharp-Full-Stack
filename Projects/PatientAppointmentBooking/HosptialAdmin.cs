using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppointmentBooking
{
    public class HosptialAdmin
    {
        public Department hospital = new Department();
       
        
        public HosptialAdmin()
        {
            // Create doctors (they auto-register to department)
            Doctor d1 = new Doctor("Dr. Smith", "Cardiology", hospital);
            Doctor d2 = new Doctor("Dr. John", "Neurology", hospital);
            Doctor d3 = new Doctor("Dr. Emma", "Cardiology", hospital);


            // Set availability for each doctor (use today's date so only HH:mm prints)
            TimeOnly today = new TimeOnly(0,0);

            DateTime now=DateTime.Today;
            d1.SetAvailability(today.AddHours(11), today.AddHours(11.5),DateOnly.FromDateTime(now).AddDays(1));   // 11:00 - 12:00
            d2.SetAvailability(today.AddHours(10), today.AddHours(10.5), DateOnly.FromDateTime(now).AddDays(1));  // 10:00 - 11:00
            d3.SetAvailability(today.AddHours(8.5), today.AddHours(9), DateOnly.FromDateTime(now).AddDays(1)); // 08:30 - 9:00

            Console.WriteLine("Welcome to the Hospital Scheduling System.");
            Console.WriteLine("Patients will be served until all slots are exhausted.\n");
        }
        public Dictionary<int,Patient> patients=new Dictionary<int,Patient>();
        bool flag;

        /// <summary>
        /// Creating aQueue the holds objects of Wating list members
        /// </summary>
        
        public Queue<Patient> wl = new Queue<Patient>();
        //public static int i = 1;
        string patientName;
        string department;
        int uniqueNumber;
        ConsoleKeyInfo key;
        public void BookSlot()
        {
            Console.WriteLine("Enter ESC to exit");
            while (true)
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("ESC key pressed. Exiting...");
                    break;
                }
                while (true)
                {
                    Console.WriteLine("Enter the name:");
                    patientName = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(patientName))
                        break;

                    Console.WriteLine("Name cannot be empty.");
                }

                // Show department list with numbers
                var deptList = hospital.Departments.Keys.ToList();
                if (deptList.Count == 0)
                {
                    Console.WriteLine("No departments found. Exiting.");
                    break;
                }


                Console.WriteLine("\nAvailable Departments:");
                for (int i = 0; i < deptList.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {deptList[i]}");
                }
                int selection;
                string chosenDept = null;
                while (true)
                {
                    Console.Write("Select department by number: ");
                    bool result = int.TryParse(Console.ReadLine(), out selection);
                    if (result)
                    {
                        if (selection > deptList.Count)
                        {
                            Console.WriteLine("\nEnter the Values Shown Above");
                            continue;
                        }
                        else
                        {
                            for (int i = 0; i < deptList.Count; i++)
                            {
                                if (i == (selection - 1))
                                {
                                    chosenDept = deptList[i];
                                    break;
                                }

                            }
                            break;

                        }
                    }
                    else
                    {
                        Console.WriteLine("\nDo Enter Alphabets");
                        continue;
                    }
                }

                // Unique number validation (exactly 4 digits)
                while (true)
                {
                    Console.WriteLine("Enter the unique number (4 digits):");
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out uniqueNumber) && input.Length == 4)
                    {
                        
                        break;
                    }

                    Console.WriteLine("Invalid input. Unique number must be exactly 4 digits.");
                }


                var doctorsInDept = hospital.Departments[chosenDept];
                // Collect (doctor, earliestSlot) pairs where doctor has at least one slot
                var doctorEarliestSlots = doctorsInDept
                    .Where(d => d.Slots.Any())
                    .Select(d => new { Doctor = d, Earliest = d.Slots.Min(s => s.Start) })
                    .ToList();

                if (!doctorEarliestSlots.Any())
                {
                    if (wl.Count < 1)
                    {
                        Console.WriteLine($"No available slots currently in {chosenDept}. Added to Waiting List.\n");
                        wl.Enqueue(new Patient(patientName, chosenDept, uniqueNumber));
                        continue;
                    }
                    else
                    {
                        Console.WriteLine($"No more waiting List and No slots in {chosenDept}.Have A nice day");
                        continue;
                    }
                }

                // Choose the doctor with the earliest Start time
                var best = doctorEarliestSlots.OrderBy(x => x.Earliest).FirstOrDefault();
                Doctor selectedDoctor = best.Doctor;
                // The doctor's earliest slot is the one with matching Start
                Slot slotToBook = selectedDoctor.Slots.OrderBy(s => s.Start).First();

                // "Book" -> remove the slot from the doctor's list
                selectedDoctor.Slots.Remove(slotToBook);

                foreach (var pat in patients)
                {
                    flag = false;
                    if(pat.Value.Name.ToLower()==patientName.ToLower() && pat.Value.Department==selectedDoctor.DepartmentName && pat.Value.uniqueNumber==uniqueNumber)
                    {
                        Console.WriteLine("Already Registered Accountt");
                        Console.WriteLine("==============Your Booking Details===============");
                        Console.WriteLine($"  Department : {chosenDept}");
                        Console.WriteLine($"  Doctor     : {selectedDoctor.Name}");
                        Console.WriteLine($"  Slot       : {slotToBook}");
                        Console.WriteLine($"  Remaining slots for {selectedDoctor.Name}: {selectedDoctor.Slots.Count}\n");
                        flag = true;
                        break;

                    }
                    
                }
                if (flag)
                    continue;
                patients.Add(uniqueNumber, new Patient(patientName, selectedDoctor.DepartmentName, uniqueNumber, selectedDoctor.Name,slotToBook.date,slotToBook.Start,slotToBook.End));
                
                //foreach (var pat in patients)
                //{
                //    Console.WriteLine(pat.Key);
                //    Console.WriteLine(pat.Value.Name);
                //    Console.WriteLine(pat.Value.Department);
                //    Console.WriteLine(pat.Value.DoctorName);
                //    Console.WriteLine($"{pat.Value.slotdate} -- {pat.Value.Start} to {pat.Value.End}");
                //    Console.WriteLine(pat.Value.uniqueNumber);

                //}


                // Confirm booking
                Console.WriteLine($"\nBooked for patient '{patientName}':");
                Console.WriteLine($"  Department : {chosenDept}");
                Console.WriteLine($"  Doctor     : {selectedDoctor.Name}");
                Console.WriteLine($"  Slot       : {slotToBook}");
                Console.WriteLine($"  Remaining slots for {selectedDoctor.Name}: {selectedDoctor.Slots.Count}\n");
            }
            if(!(key.Key == ConsoleKey.Escape))
            {
                Console.WriteLine("Appotiment Bokking Program will exit.");
            }

            
        }

        // Helper to check if any slot remains in hospital
        static bool AnySlotsLeft(Department hospital)
        {
            foreach (var kv in hospital.Departments)
            {
                foreach (var doc in kv.Value)
                {
                    if (doc.Slots.Count > 0) return true;
                }
            }
            return false;
        }
    }
}
