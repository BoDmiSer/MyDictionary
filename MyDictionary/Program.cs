using System;

namespace MyDictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var mydictonary = new MyDictionaryClass<int, string>();
            mydictonary[0] = "zero";
            mydictonary[1] = "one";
            mydictonary.Add(2, "two");
            Console.WriteLine(mydictonary.Count);
            mydictonary.Clear();
            mydictonary.Add(2, "two");
            mydictonary[0] = "zero";
            mydictonary[1] = "one";
            mydictonary[2] = "two";
            mydictonary.Remove(2);
            Console.WriteLine(mydictonary.Count);
            Console.WriteLine(mydictonary[2]);
            Console.ReadKey();
        }
    }
}
