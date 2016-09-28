using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJackProject
{
    public class TableManager
    {
        //Konstruktor
        public TableManager()
        {
        }



        //Metoder
        //Kontrollera värdet på en hand
        public int CheckHandValue(Stack<Cards> hand)
        {
            int hValue = 0;
            int aceValue = 0;

            bool iHaveAce = false;

            foreach (var card in hand)
            {
                if (card.Face.Equals(Face.Ace))
                {
                    iHaveAce = true;
                    break;
                }
            }

            if (iHaveAce)                                                   //Hantera ess som 1 eller 11?
            {
                foreach (var item in hand)
                {
                    aceValue += item.Value;

                }

                if ((aceValue + 10) <= 21)                                   //Räkna esset som 11
                {
                    hValue = aceValue + 10;
                }

                else
                {
                    hValue = aceValue;                                      //Räkna esset som 1
                }
            }

            else
            {
                foreach (var item in hand)
                {
                    hValue += item.Value;

                }
            }

            return hValue;
        }

        //Kontrollera vilken spelare som har bästa hand
        public int CheckWinner(Dealer p1, Player p2)
        {
            int h1 = CheckHandValue(p1.myCards);
            int h2 = CheckHandValue(p2.myCards);
            Console.WriteLine($"{h1}:{h2}");

            if (h1 < h2 && h2 <= 21)           //Spelare 2 har bättre hand
            {
                
                return 2;
            }
            else if (h1 > h2 && h1 <= 21)    //Spelare 1 har bättre hand
            {
               
                return 1;
            }
            else if (h1==h2 && h1<=21 && h2 <=21)                                                                                                     //Båda spelare har samma värde på sina händer
            {
              
                return 3;
            }
            else
            {
             
                return -1;
            }

        }


        //Kontrollera om handen är större än 21
        public bool Isfat(Stack<Cards> h)
        {
            if (CheckHandValue(h) > 21)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //Kontrollera om handen är BLACK JACK
        public bool IsBlackJack(Stack<Cards> h)
        {
            bool iHaveAce = false;
            foreach (var card in h)
            {
                if (card.Face == Face.Ace)
                {
                    iHaveAce = true;
                }
            }
            if (iHaveAce && CheckHandValue(h) == 21)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




    }
}
