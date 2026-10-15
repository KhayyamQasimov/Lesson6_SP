using System;
using System.Collections.Generic;
using System.Threading;

public class MenuItem
{
    public string Text;
    public Action Do;
}

public class ConsoleUI
{
    private ThreadManager manager;
    private int selected = 0;
    private bool running = true;

    public ConsoleUI(ThreadManager manager)
    {
        this.manager = manager;
    }

    public void Run()
    {
        Console.CursorVisible = false;
        Console.Clear();

        while (running)
        {
            var items = BuildMenu();

            if (selected >= items.Count) selected = items.Count - 1;

            Draw(items);

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow && selected > 0) selected--;
                else if (key == ConsoleKey.DownArrow && selected < items.Count - 1) selected++;
                else if (key == ConsoleKey.Enter) items[selected].Do();
            }

            Thread.Sleep(100);
        }

        Console.Clear();
    }

    private List<MenuItem> BuildMenu()
    {
        var items = new List<MenuItem>();

        items.Add(new MenuItem { Text = "Create New", Do = () => manager.Create() });
        items.Add(new MenuItem { Text = "Places  -  (azalt)", Do = () => manager.DecreasePlaces() });
        items.Add(new MenuItem { Text = "Places  +  (artir)", Do = () => manager.IncreasePlaces() });

        foreach (var t in manager.GetCreated())
        {
            int id = t.Id;
            items.Add(new MenuItem { Text = $"Thread {id}  ->  Waiting-e at", Do = () => manager.SendToWaiting(id) });
        }

        foreach (var t in manager.GetWorking())
        {
            int id = t.Id;
            items.Add(new MenuItem { Text = $"Thread {id}  ->  Release et", Do = () => manager.Release(id) });
        }

        items.Add(new MenuItem { Text = "Exit", Do = () => running = false });

        return items;
    }

    private void Draw(List<MenuItem> items)
    {
        var working = manager.GetWorking();
        var waiting = manager.GetWaiting();
        var created = manager.GetCreated();

        Console.SetCursorPosition(0, 0);
        Console.WriteLine("".PadRight(70));
        Console.WriteLine("Working Threads".PadRight(22) + "Waiting Threads".PadRight(22) + "Created Threads".PadRight(22));
        Console.WriteLine("".PadRight(70));

        int rows = 8;
        for (int i = 0; i < rows; i++)
        {
            string a = i < working.Count ? $"Thread {working[i].Id}->{working[i].Remaining}" : "";
            string b = i < waiting.Count ? $"Thread {waiting[i].Id}" : "";
            string c = i < created.Count ? $"Thread {created[i].Id}" : "";
            Console.WriteLine((a.PadRight(22) + b.PadRight(22) + c.PadRight(22)).PadRight(70));
        }

        Console.WriteLine("".PadRight(70));
        Console.WriteLine(("Places in Semaphore: " + manager.Places).PadRight(70));
        Console.WriteLine("".PadRight(70));

        for (int i = 0; i < items.Count; i++)
        {
            if (i == selected)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(("  > " + items[i].Text).PadRight(40));
                Console.ResetColor();
                Console.WriteLine("".PadRight(30));
            }
            else
            {
                Console.WriteLine(("    " + items[i].Text).PadRight(70));
            }
        }

        for (int i = 0; i < 5; i++)
            Console.WriteLine("".PadRight(70));

    }
}