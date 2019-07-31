using System.Collections.Generic;
using System.Linq;

namespace Something
{
    class Animal
    {
        private int _legs;
        private bool _canSwim;
        private bool _canFly;

        public Animal()
        {
            IEnumerable<int> bla = Enumerable.Range(1, 10)
                    .Where(i => i % 2 == 0) //filter
                ;
        }
    }
}