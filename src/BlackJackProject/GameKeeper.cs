using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BlackJackProject
{
    public class GameKeeper
    {
        //Bordets variabler
        Player[] tableOfPlayers;
        Stack<Cards> tableDeck;
        ScreenManager scrm;
        TableManager tb;
        HighScoreManager hs;
        //Konstruktor
        public GameKeeper()
        {
            tableOfPlayers = new Player[4] { new Dealer(), null, null, null };
            tb = new TableManager();
            tableDeck = Cards.GetAFreshDeck();
            hs = new HighScoreManager();
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

                PlaceYourBet();

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
                    tableOfPlayers[0].isActivePlayer = true;
                    Cards.DrawACard(tableOfPlayers[0], tableDeck);
                    scrm.RefreshTable();
                    tableOfPlayers[0].isActivePlayer = false;
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
                        hs.HighScoreWorthyCheck(tableOfPlayers);
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

        //Kolla hur många lediga platser som finns kvar på bordet, plocka bort de som inte har pengar kvar
        private int RemainingSeats()
        {
            int remaining = 0;

            for (int i = 1; i < tableOfPlayers.Length; i++)
            {
                if (tableOfPlayers[i] != null && tableOfPlayers[i].Cash == 0)
                {
                    Console.WriteLine($"{tableOfPlayers[i].Name} you are broke and will be removed from the table!");
                    Console.ReadKey();

                    RemovePlayer(tableOfPlayers[i]);
                    remaining += 1;
                }

                else if (tableOfPlayers[i] == null)
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
                Console.Write($"How many new players (max {RemainingSeats()}): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out nrOfPlayers) && (nrOfPlayers <= RemainingSeats()) && (nrOfPlayers >= 0))
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
            scrm.RefreshTable();
        }

        //Töm alla spelares gamla händer
        private void ClearHands()
        {
            for (int i = 0; i < tableOfPlayers.Length; i++)
            {
                if (tableOfPlayers[i] != null)
                {
                    tableOfPlayers[i].myCards.Clear();
                    tableOfPlayers[i].isFat = false;

                }
            }
            scrm.RefreshTable();
        }

        //Satsa dina surt förvärvade pengar din speltorsk!
        private void PlaceYourBet()
        {

            for (int i = 1; i < tableOfPlayers.Count(); i++)
            {
                if (tableOfPlayers[i] != null)
                {

                    bool isBetting = true;
                    while (isBetting)
                    {
                        Console.Write($"{tableOfPlayers[i].Name} have {tableOfPlayers[i].Cash}, how much do you want to bet? : ");
                        string input = Console.ReadLine();
                        int x;
                        if (int.TryParse(input, out x) && x <= tableOfPlayers[i].Cash && x > 0)
                        {
                            tableOfPlayers[i].BettingCash = x;
                            tableOfPlayers[i].Cash -= x;
                            isBetting = false;
                        }
                        else
                        {
                            Console.WriteLine($"{input} is not a valid option");
                        }


                    }

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
                    Cards.DrawACard(tableOfPlayers[0], tableDeck);          //Ge kort till dealern
                    scrm.RefreshTable();

                    for (int k = 1; k < tableOfPlayers.Length; k++)
                    {
                        if (tableOfPlayers[k] != null && tableOfPlayers[k].BettingCash > 0)
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
            p1.isActivePlayer = true;

            while (isRunning)
            {

                scrm.RefreshTable();
                Console.SetCursorPosition(0, 25);
                if (tb.IsBlackJack(p1.myCards))
                {
                    Console.WriteLine($"Congrats {p1.Name} you have BLACKJACK!");
                    Thread.Sleep(2000);
                    isRunning = false;
                }
                else
                {
                    Console.WriteLine($"{p1.Name} your handvalue is {tb.CheckHandValue(p1.myCards)} Do you want to draw a card? (Y/N)");
                    playerChoice = Console.ReadLine().ToLower();

                    if (playerChoice == "y")
                    {
                        Cards.DrawACard(p1, tableDeck);
                        if (tb.Isfat(p1.myCards))
                        {
                            Console.WriteLine("You busted!");
                            Thread.Sleep(2000);
                            isRunning = false;
                            p1.isFat = true;
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

            p1.isActivePlayer = false;
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
