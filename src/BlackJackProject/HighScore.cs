using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJackProject
{
    public class HighScore
    {
        public HighScore(string name, int cash)
        {
            Name = name;
            Cash = cash;


        }
        public string Name { get; set; }
        public int Cash { get; set; }

    }
}
