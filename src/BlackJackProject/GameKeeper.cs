using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJackProject
{
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


            bool isRunning = true;

            while (isRunning)
            {
                if (RemainingSeats() > 0)
                {
                    AddPlayer();
                }

                ClearHands();

                //Placera anropet av bettingen här PlaceYourBets();

                NewRound();

                //Låt varje spelare spela sin hand
                for (int k = 1; k < tableOfPlayers.Length; k++)
                {
                    if (tableOfPlayers[k] != null)
                    {
                        PlayYourHand(tableOfPlayers[k]);
                    }

                }

                //Låt dealern dra sina kort
                while (tb.CheckHandValue(tableOfPlayers[0].myCards) < 17)
                {
                    Cards.DrawACard(tableOfPlayers[0], tableDeck);
                    scrm.RefreshTable();
                }

                //Kontrollera vinnare
                for (int k = 1; k < tableOfPlayers.Length; k++)
                {
                    if (tableOfPlayers[k] != null)
                    {
                        int winner = tb.CheckWinner((Dealer)tableOfPlayers[0], tableOfPlayers[k]);
                        if (winner == 1)
                        {
                            Console.WriteLine("Dealer wins");
                        }
                        else if (winner == 2)
                        {
                            Console.WriteLine($"{tableOfPlayers[k].Name} wins!");
                        }
                        else if (winner == 3)
                        {
                            Console.WriteLine("PUSH!");

                        }
                        else
                        {
                            Console.WriteLine("Something went wrong....");

                        }
                    }

                }
                Console.ReadKey();

                //Kolla om det ska spelas en ny omgång
                while (true)
                {
                    Console.Write("Do you want to play a new round of Black Jack?");
                    string input = Console.ReadLine().ToLower();
                    if (input == "n")
                    {
                        isRunning = false;
                        break;
                    }
                    else if (input == "y")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("I don't understand....");
                    }
                }

            }
        }

        //Kolla hur många lediga platser som finns kvar på bordet
        private int RemainingSeats()
        {
            int remaining = 0;

            for (int i = 1; i < tableOfPlayers.Length; i++)
            {
                if (tableOfPlayers[i] == null)
                {
                    remaining += 1;
                }
            }
            return remaining;
        }

        //Lägg till spelare på bordet
        private void AddPlayer()
        {
            int nrOfPlayers;

            while (true)
            {
                Console.Write($"How many players (max {RemainingSeats()}): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out nrOfPlayers) && (nrOfPlayers <= RemainingSeats()) && (nrOfPlayers > 0))
                {
                    for (int i = 1; i <= nrOfPlayers; i++)
                    {
                        tableOfPlayers[Array.IndexOf(tableOfPlayers, null)] = new Player("");
                    }
                    break;
                }

            }
        }

        //Ta bort spelare från bordet
        private void RemovePlayer(Player p)
        {
            tableOfPlayers[Array.IndexOf(tableOfPlayers, p)] = null;
        }

        //Töm alla spelares gamla händer
        private void ClearHands()
        {
            for (int i = 0; i < tableOfPlayers.Length; i++)
            {
                if (tableOfPlayers[i] != null)
                {
                    tableOfPlayers[i].myCards.Clear();
                    //scrm.RefreshTable();
                }
            }
        }


        //Dela ut 2 kort till varje spelare vid bordet, inkl. Dealern på första platsen
        private void NewRound()
        {

            if (tableOfPlayers[0].myCards.Count == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int k = 0; k < tableOfPlayers.Length; k++)
                    {
                        if (tableOfPlayers[k] != null)
                        {
                            Cards.DrawACard(tableOfPlayers[k], tableDeck);
                            scrm.RefreshTable();
                        }
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
                scrm.RefreshTable();
                Console.SetCursorPosition(0, 25);
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
