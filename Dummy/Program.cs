using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dummy
{
    class Program
    {
        

        int AddOne(int i) => i + 1;

        delegate int ExampleDelegate(int i);

        void Main(string[] args)
        {

            // Originial delegate initialization syntax with a named method.
            ExampleDelegate a = AddOne;

            Func<int, int> b = AddOne;

            // Delegate initialization with an inline anonymous method.
            // This syntax was introduced with C# version 2.0.
            ExampleDelegate bx = delegate (int i) { return i + 1; };

            // C# version 3.0 introduces a 
            ExampleDelegate c = i => i + 1;
            



            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
            Console.WriteLine("Hello World!");
            Console.ReadKey();
            Func<int, int> addOne = x => x + 1;

            int result = addOne(2);
            List<string> redVehicles = 
                new List<string>();
            var bla = new Animal();

            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
        }


        class Animal
        {
            private int _legs;
            private bool _canSwim;
            private bool _canFly;

            static int _y = 2;

            Func<int, bool> f = x =>
            {
                _y++;
                return x > _y;
            };

            private void Calculations()
            {
                int DoMathOperation(Func<int, int, int> f, int i1, int i2)
                {
                    return f(i1, i2);
                }

                Func<int, int, int> multiplication = (i1, i2) => i1 * i2;
                int a = DoMathOperation(multiplication, 3, 2);

            }


            private IEnumerable<int> bla = Enumerable.Range(1, 10)
                    .Where(delegate(int i) {return false;}) //filter
                ;
            public void ding()
            {

                var blx = 3;
                IEnumerable<int> blax = Enumerable.Range(1, 10)
                        .Where(i =>
                        {
                            blx++;
                            blx--;
                            blx += 12;
                            blx = 18 + i;
                            blx = i;
                            return i % 2 == 0;
                        }) //filter
                    ;
                Helper.WriteSomething();
            }
            // [""100"", ""80"", ""60"", ""40"", ""20""]


            void Crop(int x, int y)
            {
                List<string> vehicles = new List<string>(){"Red Car", "Red Plane", "Blue Car"};

                List<string> redVehicles = vehicles
                    .Where(t => t.StartsWith("Red"))
                    .Select(s => s + "-foo")
                    .Select(s => s + "-bar")
                    .ToList();
            }
            public Animal(int legs = 0)
            {
                IEnumerable<int> numbers = Enumerable.Range(0, 5);
                int y = 2;
                int z = 3;
                ++z;
                var  doubleNumbers = numbers.Select(x =>
                {
                    z++;
                    return x * y * z;
                });
            }


        }

        static class Helper
        {
            public static void WriteSomething()
            {
                Console.WriteLine("Something");
            }
        }
    }
    class Animalb
    {
        private int _a;
        private int _b;
        private int _c;

        private void ChangeA()
        {
            _a = 1;
        }

        private void ChangeB()
        {
            _b = 1;
        }

        private void ChangeC()
        {
            _c = 1;
        }

        private void ChangeAB()
        {
            _a = 1;
            _b = 1;
        }

        private void ChangeAC()
        {
            _a = 1;
            _c = 1;
        }


    }
}
