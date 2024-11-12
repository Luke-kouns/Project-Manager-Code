// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using ProjectManager;

int Clamp(int min, int max, int value)
{
    if (value < min) value = min;
    if (value > max) value = max;
    return value;
}

/* A program that creates a dynamic clanader which allows you to navigate through different objects and search for objects based on specific characteristics
 * 
 */


EventManager eventManager = new();


init();
while (true)
{
    MainInputLoop();
}

// Functions and Methods

void init()
{
    // Checks if the save file path exists, if it doesnt, create a new one.
    if (!File.Exists(EventManager.GetSavePath()))
    {
        using (StreamWriter sw = File.CreateText(EventManager.GetSavePath()))
        {
            sw.WriteLine("New File");
        }
    }

    Console.WriteLine("Loading...");

    // Initilizes the event manager.
    eventManager.Init();

    Console.WriteLine("Loading Done");
}

void MainInputLoop()
{
    Console.Clear();
    Console.WriteLine("Current number of events: " + eventManager.GetNumberOfEvents() + "\n\n");
    Console.WriteLine(ManagerUI.MAIN_OPTIONS_LIST);
    string key = InputManagement.ReadConsoleUnnullable();
    switch (key)
    {
        case "1":
            EventManagerLoop();
            break;
        case "2":
            DisplayEventLoop();
            break;
        case "3":
            Console.Clear();
            eventManager.DisplayAll();
            Console.WriteLine("\nPress enter to continue");
            Console.ReadLine();
            break;
        case "4":
            eventManager.SaveAll();
            break;
        default:
            break;
    }
}

void EventManagerLoop()
{
    // Used to eliminate impossible answers (without using recursion)
    bool done = false;

    // Until told to end the loop, keep going
    while (!done)
    {

        // Clears the screen and prints the main page
        Console.Clear();
        Console.WriteLine(ManagerUI.EVENTS_MANAGER_PAGE_TOP);
        Console.WriteLine("Events:");
        int i = 1;
        foreach (Event e in eventManager.GetDictionary().Values)
        {
            Console.WriteLine(i + ": " + e.name);
            i++;
        }
        Console.WriteLine(ManagerUI.EVENTS_MANAGER_PAGE_BOTTOM);

        string key = InputManagement.ReadConsoleUnnullable();
        switch (key)
        {
            case "1": // Adds a new event into the event manager.
                Console.WriteLine("\nPlease input a name for your event");
                string add_name = InputManagement.GetUnusedName(eventManager);
                Console.WriteLine("\nPlease input the time it would take to complete this event (in minutes)");
                int time_to_complete = InputManagement.ReadConsoleUnnullableInt();
                Console.WriteLine("\nPlease input the year the event is due");
                int year = Clamp(1, 10000, InputManagement.ReadConsoleUnnullableInt());
                Console.WriteLine("\nPlease input the month the event is due (1-12)");
                int month = Clamp(1, 12, InputManagement.ReadConsoleUnnullableInt());
                Console.WriteLine("\nPlease input the day the event is due");
                int day = Clamp(1, 31, InputManagement.ReadConsoleUnnullableInt());
                Console.WriteLine("\nPlease input the importance of this event (1 - 10)");
                int importance = InputManagement.ReadConsoleUnnullableInt();
                Console.WriteLine("\nPlease write a description of the event (if there is no need for a description, type nothing then press enter)");
                string description = InputManagement.ReadConsoleUnnullable();

                // Adds the new event
                eventManager.AddEvent(add_name, time_to_complete, new(year, month, day), importance, description);

                // Waits for input before continuing
                Console.WriteLine("Please press enter to continue");
                Console.ReadLine();
                break;
            case "2": // Removes the event of the given name
                Console.WriteLine("\nPlease input the name or the number of the event to remove.");
                string remove_name = InputManagement.ReadConsoleUnnullable();
                eventManager.RemoveEvent(remove_name);

                // Waits for input before continuing
                Console.WriteLine("Please press enter to continue");
                Console.ReadLine();
                break;
            case "3":
                done = true;
                break;
            default:
                Console.WriteLine("Invalid Input, try again");
                break;
        }
    }
}

void DisplayEventLoop()
{
    // Used to eliminate impossible answers (without using recursion)
    bool done = false;

    Console.Clear();
    Console.WriteLine(ManagerUI.EVENT_GET_PAGE); // Only done once at the beginning.

    // Repeats until the back number is enterd.
    while (!done)
    {
        string key = InputManagement.ReadConsoleUnnullable();
        switch (key)
        {
            case "1": // Displays the event with the given name
                Console.WriteLine("Please Input the name of the event to get");
                string got_name = InputManagement.ReadConsoleUnnullable();
                try
                {
                    Event got_event = eventManager.GetEvent(got_name);
                    Console.WriteLine("\n" + got_event.EventString(ManagerUI.months));
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("\nThat event does not exist");
                }
                break;
            case "2":
                try
                {
                    Console.Clear();
                    eventManager.SortByTimeToComplete();
                    eventManager.DisplayAll();
                    Console.WriteLine("Please press enter to continue");
                    Console.ReadLine();
                    done = true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("\nThere are no events to sort");
                }
                break;
            case "3":
                try
                {
                    Console.Clear();
                    eventManager.SortByImportance();
                    eventManager.DisplayAll();
                    Console.WriteLine("Please press enter to continue");
                    Console.ReadLine();
                    done = true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("\nThere are no events to sort");
                }
                break;
            case "5":
                done = true;
                break;
            default:
                Console.WriteLine("Invalid Input, try again");
                break;
        }
    }
}
