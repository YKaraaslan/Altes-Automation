using System;

namespace DatabaseCreater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started Creating databases...");
            Main main = new Main();
            main.onStart();
            Console.WriteLine("\n\nCreating databases finished...");
            Console.ReadLine();
        }
    }
}
