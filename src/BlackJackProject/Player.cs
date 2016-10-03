using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BlackJackProject
{
    public class Player
    {

        public Player()
        {

        }
        public Player(string s)
        {
            
            Console.SetCursorPosition(0, 27);
            Cash = 100;
            Console.Write("Your name can't be longer then 10 characters \nEnter your name: ");
            Name = Console.ReadLine();
            if (string.IsNullOrEmpty(Name) == true || Name.Length > 10)
            {
                Random rnd = new Random();
                int nisseN = rnd.Next(10, 99);
                Name = $"Nisse{nisseN}";
            }
            if (Name.Contains("  "))
            {
                Cash = 100000;
            }
        }

        public string Name { get; set; }
        public double Cash { get; set; }
        public bool isFat = false;
        public bool isDealer = false;

        public Stack<Cards> myCards = new Stack<Cards>();
        public Stack<Cards> myCardsSplit = new Stack<Cards>();
        
        public int BettingCash { get; set; }
        public int BettingCashSplit { get; set; }

        public bool isActivePlayer = false;
        public bool isActivePlayerSplit = false;

        public string Text { get; set; }
        public string TextSplit { get; set; }     

    }
}