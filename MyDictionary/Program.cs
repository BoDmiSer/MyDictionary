using System;

namespace MyDictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //var mydictonary = new MyDictionaryClass<int, string>();
            //mydictonary[0] = "zero";
            //mydictonary[1] = "one";
            //mydictonary.Add(2, "two");
            //Console.WriteLine(mydictonary.Count);
            //mydictonary.Clear();
            //mydictonary.Add(2, "two");
            //mydictonary[0] = "zero";
            //mydictonary[1] = "one";
            //mydictonary[2] = "two";
            //mydictonary.Remove(2);
            //Console.WriteLine(mydictonary.Count);
            //Console.WriteLine(mydictonary[2]);

            var myDictonary2 = new MyDictionaryClass_2<int, string>();
            myDictonary2.Add(1, "one");
            myDictonary2.Add(2, "two");
            myDictonary2.Add(3, "three");
            Console.WriteLine(myDictonary2.ContainsKey(1));
            Console.WriteLine(myDictonary2.ContainsKey(4));
            myDictonary2.Add(4, "for");
            myDictonary2.Remove(4);
            Console.WriteLine(myDictonary2.ContainsKey(4));
            //foreach (var item in myDictonary2)
            //{
            //    Console.WriteLine(item+" ");
            //}
            Console.ReadKey();
        }
    }
}
