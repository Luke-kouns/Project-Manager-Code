using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProjectManager
{
    // A class containing all the UI strings and other reasources for the calander.
    class ManagerUI
    {
        // The string for the list of main options (mostly other lists)
        public const string MAIN_OPTIONS_LIST =
            "                      Event Calander\n" +
            "           Please type the number you want to do\n\n" +
            "1) Manage Events\n" +
            "2) Display Events\n" +
            "3) Display All\n" +
            "4) Save\n";

        // The string containing event management options (add, remove, etc)
        public const string EVENTS_MANAGER_PAGE_TOP =
            "                      Event Manager\n" +
            "           Please type the number you want to do\n\n";

        public const string EVENTS_MANAGER_PAGE_BOTTOM =
            "\n\n1) Add Event\n" +
            "2) Remove Event\n" +
            "3) Back\n";

        // The string containing event display options (get, sort, etc)
        public const string EVENT_GET_PAGE =
            "                       Display Events\n" +
            "           Please type the number you want to do\n\n" +
            "1) Display Event\n" +
            "2) Sort And Display: Time To Complete\n" +
            "3) Sort And Display: Importance\n" +
            "4) Sort And Display: Soonest Due\n" +
            "5) Back\n";

        public static Dictionary<int, string> months = new Dictionary<int, string>
        {
            {1, "January"},
            {2, "Febuary"},
            {3, "March"},
            {4, "April"},
            {5, "May"},
            {6, "June"},
            {7, "July"},
            {8, "August"},
            {9, "September"},
            {10, "October"},
            {11, "November"},
            {12, "December"}
        };
    }
    
    // A class for managing inputs in the project
    class InputManagement
    {
        // Gets input from the console while eliminating the option of it being null.
        public static string ReadConsoleUnnullable()
        {
            // Creates a nullable string which is set to the next line of characters.
            string? output = Console.ReadLine();

            // If the output is null, then write a message in the console and call this function again.
            if (output == null)
            {
                Console.WriteLine("That input is null, please put in a valid input\n");
                output = ReadConsoleUnnullable();
            }

            // When a non null responce is given return a straight string.
            return output;
            
        }

        // Gets string input from the console while eliminating the option of it being null.
        public static string ReadConsoleUnnullableString()
        {
            // Creates a nullable string which is set to the next line of characters.
            string? output = Console.ReadLine();

            // If the output is null, then write a message in the console and call this function again.
            if (output == null)
            {
                Console.WriteLine("That input is null, please put in a valid input\n");
                output = ReadConsoleUnnullableString();
            }

            try
            {
                float bad = float.Parse(output); // try to parse the string, if you can, get a new value
                Console.WriteLine("Please dont input a number");
                return ReadConsoleUnnullableString();
            }
            catch
            {
                // When a non null responce is given return a straight string.
                return output;
            }
        }

        // Gets integer input from the console while eliminating the option of it being null.
        public static int ReadConsoleUnnullableInt()
        {
            // Creates a nullable string which is set to the next line of characters.
            string? output = Console.ReadLine();

            // If the output is null, then write a message in the console and call this function again.
            if (output == null)
            {
                Console.WriteLine("\nThat input is null, please put in a valid input");
                output = ReadConsoleUnnullableString();
            }

            // When a non null responce is given try return a parsed int.
            try
            {
                return int.Parse(output);
            }
            catch // if an exception is thrown, try again.
            {
                Console.WriteLine("\nThat input is not a integer, please try again");
                return ReadConsoleUnnullableInt();
            }
        }

        // A function to get an inputted name for a function that has not been used before.
        public static string GetUnusedName(EventManager eventManager)
        {
            string name = ReadConsoleUnnullableString(); // Gets and inputted name.

            // tries to check for the name and returns the name if it does not already exist
            // if it does already exist, then get a new name.
            try
            {
                if (eventManager.CheckForEvent(name))
                {
                    Console.WriteLine("\nThat name already exists, please input a new one");
                    return GetUnusedName(eventManager);
                }
                else
                {
                    return name;
                }
            }
            catch (NullReferenceException) // If there are no names, then this one is good.
            {
                return name;
            }
        }
    }

    // time till structure, used to track the time until something (key difference from DateTime, can have values that are zero.)
    public struct TimeTill(int year, int month, int day)
    {
        public int year = year;
        public int month = month;
        public int day = day;
    }

    // Event Structure, the basic container for a task to event to be completed
    public struct Event
    {
        public string name; // The name of the event
        public string description; // A description of the event

        // Time Based conditions
        public float time_to_complete; // The time in minutes the task will take to complete 
        public DateTime date_due; // The date the task is due 
        public TimeTill time_till_due; // The time until the task it due

        // The importance of the assignment, out of 10
        private int _importance; // Backing Store
        public int Importance
        {
            get => _importance;
            set
            {
                if (value < 0)
                {
                    _importance = 0;
                }
                else if (value > 10)
                {
                    _importance = 10;
                }
                else
                {
                    _importance = value;
                }
            }
        }


        // The base constructor
        public Event(string name, float time_to_complete, DateTime date_due, int importance, string description)
        {
            this.name = name;
            this.time_to_complete = time_to_complete;
            this.date_due = date_due;
            this.Importance = importance;
            this.description = description;

            // Sets the time until due
            this.time_till_due = CalculateTimeTillDue(DateTime.Now);
        }

        // Calculates the difference between two dates.
        readonly public TimeTill CalculateTimeTillDue(DateTime start_date)
        {
            int day_diff = date_due.Day - start_date.Day;
            int month_diff = date_due.Month - start_date.Month;
            int year_diff = date_due.Year - start_date.Year;

            return new(year_diff, month_diff, day_diff);
        }

        // Sets the event up to be printed, will look like this 
        /*                 Event Name (30 minutes)
         *                 
         *                  Due December 12, 2024
         *                Time Left: 1 month, 5 days
         *                   Importance: 10 / 10
         *                   
         *  This is an example of what the description could possibly be like,
         *I have no reference for how long it will be on average.*/
         // Param months, the dictonary of months to display.
        public string EventString(Dictionary<int, string> months)
        {
            return "                 " + name + " (" + time_to_complete + " minutes)\n\n" +
                "                  Due " + months[date_due.Month] + " " + date_due.Day + ", " + date_due.Year + "\n" +
                "                Time Left: " + time_till_due.year + " years, " + time_till_due.month + " month, " + time_till_due.day + " days\n" +
                "                   Importance: " + Importance + " / 10\n\n" +
                "   " + description;
        }

        // A function that saves the event into the save text file.
        // Param sw: The StreamWriter with a refernce to the save file.
        public void SaveEvent(StreamWriter sw)
        {
            sw.WriteLine(this.name);
            sw.WriteLine(this.time_to_complete);
            sw.WriteLine(this.date_due.Year);
            sw.WriteLine(this.date_due.Month);
            sw.WriteLine(this.date_due.Day);
            sw.WriteLine(this.Importance);
            sw.WriteLine(this.description);     
        }
    }

    public class EventManager
    {
        private Dictionary<string, Event> events = [];

        private const string save_path = @"C:\Users\LBKou\OneDrive\Desktop\RandomCodeProjects\ProjectManager\ProjectManagerSaveText.txt";
        
        // A method to get the path of the save file
        public static string GetSavePath()
        {
            return save_path;
        }

        // A method for accessing the amount of events from outside the class.
        public int GetNumberOfEvents()
        {
            return events.Count;
        }

        // A method for getting the main dict
        public Dictionary<string, Event> GetDictionary()
        {
            return events;
        }

        // Checks for an event with the given name
        // Param name: The name of the event to check for (key)
        // Exceptions: NullReferenceException
        public bool CheckForEvent(string name)
        {
            if (events.Count == 0)
            {
                throw new NullReferenceException();
            }
            return events.ContainsKey(name);
        }

        // For use in the events manager, gets an event's name based on its listed number.
        // Param index: the index of the event to get the name of
        public string GetEventKeyByIndex(int index)
        {
            return events.ElementAt(index).Key;
        }

        // Adds and event to the event managers event dictonary.
        public void AddEvent(string name, float time_to_complete, DateTime date_due, int importance, string description)
        {
            try
            {
                if (CheckForEvent(name))
                {
                    Console.WriteLine("\nName already used, event not added\n");
                }
                else
                {
                    events.Add(name, new(name, time_to_complete, date_due, importance, description));
                    Console.WriteLine("\nEvent \"" + name + "\" added\n");
                }
            }
            catch (NullReferenceException) 
            {
                events.Add(name, new(name, time_to_complete, date_due, importance, description));
                Console.WriteLine("\nEvent \"" + name + "\" added\n");
            }
        }
        public void AddEvent(Event event_to_add)
        {
            try
            {
                if (CheckForEvent(event_to_add.name))
                {
                    Console.WriteLine("Name already used");
                }
                else
                {
                    events.Add(event_to_add.name, event_to_add);
                }
            }
            catch (NullReferenceException)
            {
                events.Add(event_to_add.name, event_to_add);
            }
        }

        // Removes the element of the listed key (either the name of the event or an associative int.
        public void RemoveEvent(string name)
        {
            try
            {
                // Checks to make sure the event exists.
                if (CheckForEvent(name) == false)
                {
                    // Tries to get a number out of the inputted name, if an exception is thrown the event doesn't exist
                    try
                    {
                        // removes the event at the given index 
                        string key = GetEventKeyByIndex(int.Parse(name) - 1);
                        events.Remove(key);
                        Console.WriteLine("\nEvent named \"" + key + "\" removed");
                    }
                    catch
                    {
                        Console.WriteLine("\nThat Event does not exist (check the name or number)");
                    }
                }
                else
                {
                    events.Remove(name); // removes the event
                    Console.WriteLine("\nEvent named \"" + name + "\" removed");
                }  
            }
            catch
            {
                Console.WriteLine("\nNo events exist");
            }
        }
        
        // The initilizer code, for setting the event dictionary from the save file
        public void Init()
        {
            LoadAll();
        }

        // Displayes all events in the catalouge
        public void DisplayAll()
        {
            if (events.Count == 0)
            {
                Console.WriteLine("There are currently no events");
                return;
            }
            
            foreach (Event e in events.Values)
            {
                Console.WriteLine(e.EventString(ManagerUI.months) + "\n");
            }
        }

        // Returns an event with the given name.
        // Param name: the name of the event
        // Exception: NullReferenceException
        public Event GetEvent(string name)
        {
            if (CheckForEvent(name)) return events[name];
            throw new NullReferenceException();
        }


        // Gets whichever event has the shortest time to complete.
        public Event GetShortestToDo()
        {
            // If there are no events, then throw a null refernce exception.
            if (events.Count == 0) throw new NullReferenceException();

            // Sets the shortest time to do to the first event in the dictionary 
            Event shortest_to_do = events.First().Value;

            // Iterates through each value in the events dictonary 
            foreach (Event e in events.Values)
            {
                // Checks the see if the new event takes less time than the current shortest event,
                // if so, mark it as the new shortest event.
                if (e.time_to_complete < shortest_to_do.time_to_complete)
                {
                    shortest_to_do = e;
                }
            }

            return shortest_to_do; // Returns the shortest event.
        }

        // A function to sort the event manager dictionary based on the time to complete.
        public void SortByTimeToComplete()
        {
            // Makes sure there are events to sort.
            if (events.Count == 0) throw new ArgumentOutOfRangeException();

            int origional_count = events.Count; // Keeps track of the origional amount of events

            // Creates a dict to hold the events in the correct order until
            // They can be added back into the dictionary
            Dictionary<string, Event> event_intermediary = [];

            Console.WriteLine("Sorting...");

            // Repeats the number if times there are events in the origional dict
            for (int i = 0; i < origional_count; i++)
            {
                Event shortest_to_do = GetShortestToDo(); // Gets the shortest to do.
                event_intermediary.Add(shortest_to_do.name, shortest_to_do); // Adds it to the intermediary dict
                events.Remove(shortest_to_do.name); // and removes it from the dict
            }

            // Now, Go re-add all of the events to the dict
            events = event_intermediary;


            Console.WriteLine("Sorting Complete");
        }

        // Gets whichever event is the most important.
        public Event GetMostImportant()
        {
            // If there are no events, then throw a null refernce exception.
            if (events.Count == 0) throw new NullReferenceException();

            // Sets the most important event to do to the first event in the dictionary 
            Event most_important = events.First().Value;

            // Iterates through each value in the events dictonary 
            foreach (Event e in events.Values)
            {
                // Checks the see if the new event is more important than the current most important event,
                // if so, mark it as the new most important event.
                if (e.Importance > most_important.Importance)
                {
                    most_important = e;
                }
            }

            return most_important; // Returns the most important event
        }

        // A function to sort the event manager dictionary based on the importance of events.
        public void SortByImportance()
        {
            // Makes sure there are events to sort.
            if (events.Count == 0) throw new ArgumentOutOfRangeException();

            int origional_count = events.Count; // Keeps track of the origional amount of events

            // Creates a dict to hold the events in the correct order until
            // They can be added back into the dictionary
            Dictionary<string, Event> event_intermediary = [];

            Console.WriteLine("Sorting...");

            // Repeats the number if times there are events in the origional dict
            for (int i = 0; i < origional_count; i++)
            {
                Event most_important = GetMostImportant(); // Gets most important
                event_intermediary.Add(most_important.name, most_important); // Adds it to the intermediary dict
                events.Remove(most_important.name); // and removes it from the base dict
            }

            // Now, Go re-add all of the events to the dict
            events = event_intermediary;

            Console.WriteLine("Sorting Complete");
        }

        // A function that saves all of the events in the event manager to the given text file (save_file)
        public void SaveAll()
        {
            // Displays information for user
            Console.WriteLine("Saving...");

            // Gets the number of events
            int num_of_events = events.Count;

            using (StreamWriter sw = File.CreateText(save_path))
            {
                // The first line contains the number of events
                sw.WriteLine(num_of_events);

                // Then write lines for each event.
                foreach (Event e in events.Values)
                {
                    e.SaveEvent(sw);
                }
            }

            Console.WriteLine("Saving complete, press enter");
            Console.ReadLine();
        }
        
        // A function to load a single event from a save (must start at first line of event save)
        // Param sr: A stream Reader with a reference to the save file path
        public void LoadEvent(StreamReader sr)
        {
            // Sets the variables.
            string name = sr.ReadLine();
            int time_to_complete = int.Parse(sr.ReadLine());
            int year = int.Parse(sr.ReadLine());
            int month = int.Parse(sr.ReadLine());
            int day = int.Parse(sr.ReadLine());
            int importance = int.Parse(sr.ReadLine());
            string description = sr.ReadLine();

            // Creates the event.
            events.Add(name, new(name, time_to_complete, new(year, month, day), importance, description));
            // Dont worry about the null warnings, it wont happen.
        }

        // A function to load all events from the save file.
        public void LoadAll()
        {
            using (StreamReader sr = File.OpenText(save_path)) // Creates a new stream reader with a reference to the save file.
            {
                // Creates a variable for the amount of events to load.
                int num_of_events = int.Parse(sr.ReadLine());

                // Creates events accourding to that number
                for (int i = 0; i < num_of_events; i++)
                {
                    LoadEvent(sr);
                }
            }
        }
    }
}
