using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJackProject
{
    public class HighScore
    {
        public HighScore(string name, double cash)
        {
            Name = name;
            Cash = cash;


        }
        public string Name { get; set; }
        public double Cash { get; set; }

    }
}
