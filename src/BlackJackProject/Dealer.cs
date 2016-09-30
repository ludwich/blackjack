using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJackProject
{
    public class Dealer : Player
    {
        public Dealer ()
        {
            Name = "Dealer";
            isDealer = true;
            BettingCash = 0;
        }
    }
}
