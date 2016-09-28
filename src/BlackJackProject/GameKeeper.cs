using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJackProject
{//-lksdnkjöadfkjgbadfvbjado
    public class GameKeeper
    {
        //Bordets variabler
        Player[] tableOfPlayers;
        Stack<Cards> tableDeck;
        ScreenManager scrm;
        TableManager tb;
        //Konstruktor
        public GameKeeper()
        {


            tableOfPlayers = new Player[4] { new Dealer(), null, null, null };
            tb = new TableManager();
            tableDeck = Cards.GetAFreshDeck();

            scrm = new ScreenManager(tableOfPlayers);

        }




        //Loop för själva spelandet
        public void GameRunning()
        {
            int nrOfPlayers;

            while (true)
            {
                Console.Write("How many players (max 3): ");            //MÅSTE LÄGGA IN KOLL PÅ ANTALET SPELARE (NY METOD)
                string input = Console.ReadLine();
                if (int.TryParse(input, out nrOfPlayers))
                {
                    for (int i = 1; i <= nrOfPlayers; i++)
                    {
                        tableOfPlayers[i] = new Player();
                    }
                    break;
                }

            }


            while (true)
            {
                //Dela ut 2 kort till varje spelare vid bordet, inkl. Dealern på första platsen
                for (int i = 0; i < 2; i++)
                {
                    for (int k = 0; k < tableOfPlayers.Length; k++)
                    {
                        if (tableOfPlayers[k] != null)
                        { Cards.DrawACard(tableOfPlayers[k], tableDeck); }
                    }
                }

                //Låt varje spelare satsa på sin hand
                for (int k = 1; k < tableOfPlayers.Length; k++)
                {
                    if (tableOfPlayers[k] != null)
                    {
                        PlayYourHand(tableOfPlayers[k]);
                    }

                }



                
            }
        }

        //Metod för att låt<a en spelare spela/satsa färdigt sin hand
        public void PlayYourHand(Player p1)
        {
            string playerChoice = "";
            bool isRunning = true;

            while (isRunning)
            {
                Console.SetCursorPosition(0, 3);
                Console.WriteLine($"Your handvalue is {tb.CheckHandValue(p1.myCards)} Do you want to draw a card? (Y/N)");
                playerChoice = Console.ReadLine().ToLower();

                if (playerChoice == "y")
                {
                    Cards.DrawACard(p1, tableDeck);
                    if (tb.Isfat(p1.myCards))
                    {
                        Console.WriteLine("You busted!");
                        isRunning = false;
                    }


                }
                else if (playerChoice == "n")
                {
                    isRunning = false;
                }
                else
                {
                    Console.WriteLine($"{playerChoice} is not a valid input!");
                }
            }

        }




        //Sätt en ny spelare vid bordet
        public void SeatPlayer(Player p1)
        {
            for (int i = 0; i < tableOfPlayers.Length; i++)
            {
                if (tableOfPlayers[i] == null)                               //Sätter spelare p1 på första lediga plats på bordet
                {
                    tableOfPlayers[i] = p1;
                }
            }
        }


        //Ta bort en spelare från bordet
        public void RemovePlayer(Player p1)
        {
            for (int i = 0; i < tableOfPlayers.Length; i++)
            {
                if (tableOfPlayers[i] == p1)
                {
                    tableOfPlayers[i] = null;                               //Tar bort Spelare p1 från tableOfPlayers
                }
            }
        }

        //Kontrollera om det finns plats på bordet
        public bool IsTableFull()
        {
            bool fullTable = true;

            for (int i = 0; i < tableOfPlayers.Length; i++)
            {
                if (tableOfPlayers[i] == null)
                {
                    fullTable = false;                                                //Det finns minst en ledig plats vid bordet
                }
            }
            return fullTable;
        }


















    }


}
