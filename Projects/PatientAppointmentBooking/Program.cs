
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace AppointmentBooking
{
    public class Program
    {
        static CancellationTokenSource cts = new CancellationTokenSource();
        static readonly object _lock = new object();
        static void AssignSlotsFromWaitingList(HosptialAdmin admin, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                lock (_lock)
                {
                    if (admin.wl.Count == 0)
                    {
                        Thread.Sleep(2000);
                        continue;
                    }

                    Patient waitingPatient = admin.wl.Peek();

                    // Check requested department exists
                    if (!admin.hospital.Departments.ContainsKey(waitingPatient.Department))
                    {
                        admin.wl.Dequeue(); // invalid department request
                        continue;
                    }

                    var doctors = admin.hospital.Departments[waitingPatient.Department];

                    Doctor matchedDoctor = null;
                    Slot matchedSlot = null;

                    foreach (var doctor in doctors)
                    {
                        // If patient requested a specific doctor, match it
                        if (!string.IsNullOrEmpty(waitingPatient.DoctorName) &&
                            !doctor.Name.Equals(waitingPatient.DoctorName, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        if (doctor.Slots.Any())
                        {
                            matchedDoctor = doctor;
                            matchedSlot = doctor.Slots.OrderBy(s => s.Start).First();
                            break;
                        }
                    }

                    // If no slot found, skip and retry later
                    if (matchedDoctor == null || matchedSlot == null)
                    {
                        Thread.Sleep(2000);
                        continue;
                    }

                    // Remove slot & patient from waiting list
                    admin.wl.Dequeue();
                    matchedDoctor.Slots.Remove(matchedSlot);

                    admin.patients.Add(
                        waitingPatient.uniqueNumber,
                        new Patient(
                            waitingPatient.Name,
                            waitingPatient.Department,
                            waitingPatient.uniqueNumber,
                            matchedDoctor.Name,
                            matchedSlot.date,
                            matchedSlot.Start,
                            matchedSlot.End
                        )
                    );

                    lock (_lock)
                    {
                        Console.WriteLine();
                        Console.WriteLine("===== WAITING LIST AUTO BOOKED =====");
                        Console.WriteLine($"Patient    : {waitingPatient.Name}");
                        Console.WriteLine($"Department : {waitingPatient.Department}");
                        Console.WriteLine($"Doctor     : {matchedDoctor.Name}");
                        Console.WriteLine($"Slot       : {matchedSlot}");
                        Console.WriteLine("===================================");
                        Console.WriteLine();
                    }

                }

                Thread.Sleep(2000);
            }
        }



        public static void Main(string[] args)
        {

            // Run until all slots are completed
            HosptialAdmin admin = new HosptialAdmin();
            //Department department = new Department();
            

            // START BACKGROUND SLOT ASSIGNER
            Task.Run(() => AssignSlotsFromWaitingList(admin, cts.Token));



            while (true)
            {
                lock (_lock)
                {
                    Console.WriteLine("\n--- Patient Appointment System ---");
                    Console.WriteLine("1. Appointment Booking");
                    Console.WriteLine("2. Cancellation");
                    Console.WriteLine("3. WL Printing");
                    Console.WriteLine("4. Exit");
                    Console.Write("Enter your choice: ");
                }


                string choiceInput = Console.ReadLine();

                // Validate menu choice
                if (!int.TryParse(choiceInput, out int choice))
                {
                    Console.WriteLine("Invalid choice. Enter a number (1, 2, or 3).");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        admin.BookSlot();
                        break;

                    case 2:
                        Console.Write("Enter Appointment ID to cancel: ");
                        string idInput = Console.ReadLine();

                        // Validate appointment ID
                        if (int.TryParse(idInput, out int appointmentId))
                        {
                            if (admin.patients.ContainsKey(appointmentId))
                            {
                                Patient patient = admin.patients[appointmentId];

                                TimeOnly startTime = patient.Start;
                                TimeOnly currentTime = new TimeOnly(0, 0);

                                TimeSpan difference = startTime.ToTimeSpan() - currentTime.ToTimeSpan();

                                if (difference.TotalHours > 3)
                                {
                                    lock (_lock)
                                    {
                                        AddSlot(patient);
                                        admin.patients.Remove(appointmentId);
                                    }

                                    Console.WriteLine("Appointment cancelled successfully.");
                                }
                                else
                                {
                                    Console.WriteLine("You can not cancel now because cancellation window is closed");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"No appointment found with ID {appointmentId}");
                            }

                        }
                        else
                        {
                            Console.WriteLine("Invalid Appointment ID. ID must be a number.");
                        }
                        break;

                    case 4:
                        Console.WriteLine("Exiting system. Thank you!");
                        cts.Cancel();
                        Console.WriteLine("Exiting system. Thank you!");
                       
                        return;
                    case 3:
                        foreach(var item in admin.wl)
                        {
                            Console.WriteLine(item.Name);
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please choose 1, 2, or 3.");
                        break;
                }
            }
            void AddSlot(Patient patient)
            {
                foreach(var item in admin.hospital.Departments)
                {
                    if(patient.Department == item.Key)
                    {
                        foreach(var item2 in item.Value)
                        {
                            if (patient.DoctorName == item2.Name)
                            {
                                item2.Slots.Add(new Slot(patient.Start, patient.End, DateOnly.FromDateTime(DateTime.Today).AddDays(1)));

                            }
                        }
                    }


                }

            }
        }
    }



}

