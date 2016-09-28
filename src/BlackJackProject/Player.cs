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
            Console.Clear();


        }


        public string Name { get; set; }
        public Double Cash { get; set; }
        public Stack<Cards> myCards = new Stack<Cards>();
        public bool isIFat = false;
    }


}