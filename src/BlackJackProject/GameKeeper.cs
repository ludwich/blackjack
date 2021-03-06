﻿using System;
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
        Cards cd;
        ScreenManager scrm;
        TableManager tb;
        HighScoreManager hs;
        //Konstruktor
        public GameKeeper()
        {
            tableOfPlayers = new Player[4] { new Dealer(), null, null, null };
            tb = new TableManager();
            cd = new Cards();
            tableDeck = cd.GetAFreshDeck();
            hs = new HighScoreManager();
            hs.Online = HighScoreManager.IsNetworkAvailable();
            scrm = new ScreenManager(tb, tableOfPlayers, hs);
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
                        SplitHand(tableOfPlayers[k]);
                        PlayYourHand(tableOfPlayers[k]);
                    }

                }
                Thread.Sleep(800);
                tableOfPlayers[0].isActivePlayer = true;
                scrm.RefreshTable();
                //Låt dealern dra sina kort
                while (tb.CheckHandValue(tableOfPlayers[0].myCards) < 17)
                {
                    Thread.Sleep(1500);
                    cd.DrawACard(tableOfPlayers[0], tableDeck);
                    scrm.RefreshTable();
                }
                tableOfPlayers[0].Text = (tb.CheckHandValue(tableOfPlayers[0].myCards) > 21) ? "Busted" : "";

                //Kontrollera vinnare
                for (int k = 1; k < tableOfPlayers.Length; k++)
                {
                    if (tableOfPlayers[k] != null)
                    {
                        if (tableOfPlayers[k].BettingCashSplit > 0)
                        {
                            int splitwin = tb.CheckWinner((Dealer)tableOfPlayers[0], tableOfPlayers[k]);
                            if (splitwin == 1)
                            {
                                tableOfPlayers[k].TextSplit = "Loose";
                            }
                            else if (splitwin == 2)
                            {
                                tableOfPlayers[k].TextSplit = "Win";
                            }
                            else if (splitwin == 3)
                            {
                                tableOfPlayers[k].TextSplit = "PUSH";
                            }
                            else
                            {
                                Console.WriteLine("Something went wrong....");
                            }
                        }


                        int winner = tb.CheckWinner((Dealer)tableOfPlayers[0], tableOfPlayers[k]);
                        if (winner == 1)
                        {
                            tableOfPlayers[k].Text = "Loose";
                        }
                        else if (winner == 2)
                        {
                            tableOfPlayers[k].Text = "Win";
                        }
                        else if (winner == 3)
                        {
                            tableOfPlayers[k].Text = "PUSH";
                        }
                        else
                        {
                            Console.WriteLine("Something went wrong....");
                        }
                        scrm.RefreshTable();
                    }

                }
               

                //Kolla om det ska spelas en ny omgång
                while (true)
                {
                    Console.Write("Do you want to play a new round of Black Jack?");
                    string input = Console.ReadLine().ToLower();
                    if (input == "n")
                    {
                        isRunning = false;
                        hs.Online = HighScoreManager.IsNetworkAvailable();
                        hs.HighScoreWorthyCheck(tableOfPlayers);
                        scrm.DrawTheHighScores(hs);
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

            scrm.RefreshTable();
            Console.SetCursorPosition(0, 30);

            for (int i = 1; i < tableOfPlayers.Length; i++)
            {
                scrm.RefreshTable();
                if (tableOfPlayers[i] != null && tableOfPlayers[i].Cash < 1)
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
            scrm.RefreshTable();
            Console.SetCursorPosition(0, 30);
            while (true)
            {
                Console.Write($"How many new players (max {RemainingSeats()}): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out nrOfPlayers) && (nrOfPlayers <= RemainingSeats()) && (nrOfPlayers >= 0))
                {
                    for (int i = 1; i <= nrOfPlayers; i++)
                    {
                        scrm.RefreshTable();
                        tableOfPlayers[Array.IndexOf(tableOfPlayers, null)] = new Player("");
                        
                    }
                    break;
                }
            }
        }

        //Kontrollera om spelare vill splitta sin hand
        private void SplitHand(Player p1)
        {
            scrm.RefreshTable();
            Console.SetCursorPosition(0, 27);

            Cards[] check = p1.myCards.ToArray();
            if (check[0].Value == check[1].Value && p1.isActivePlayerSplit == false && p1.Cash >= p1.BettingCash && p1.BettingCash >= 2)
            {
                while (true)
                {
                    Console.Write($"{p1.Name} do you want to split?");
                    string input = Console.ReadLine().ToLower();
                    if (input == "y")
                    {
                        p1.myCardsSplit.Push(p1.myCards.Pop());     //Splitta korten
                        cd.DrawACard(p1, tableDeck);             //Dra ett nytt kort till my main deck
                        p1.isActivePlayerSplit = true;                      //Sätt split active
                        cd.DrawACard(p1, tableDeck);             //Dra ytterligare ett kort till split deck...
                        p1.Cash -= p1.BettingCash;                  //Insatsen för splitten dras från Cash
                        p1.BettingCashSplit = p1.BettingCash;
                        break;
                    }
                    else if (input == "n")
                    {
                        break;
                    }
                    else
                    {

                    }
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
            tableOfPlayers[0].isActivePlayer = false;
            for (int i = 0; i < tableOfPlayers.Length; i++)
            {
                if (tableOfPlayers[i] != null)
                {
                    tableOfPlayers[i].myCards.Clear();
                    tableOfPlayers[i].myCardsSplit.Clear();
                    tableOfPlayers[i].isActivePlayerSplit = false;
                    tableOfPlayers[i].isFat = false;
                    tableOfPlayers[i].Text = "";
                    tableOfPlayers[i].TextSplit = "";
                }
            }
            scrm.RefreshTable();
        }

        //Satsa dina surt förvärvade pengar din speltorsk!
        private void PlaceYourBet()
        {
            scrm.RefreshTable();
            Console.SetCursorPosition(0, 30);

            for (int i = 1; i < tableOfPlayers.Count(); i++)
            {
                if (tableOfPlayers[i] != null)
                {
                    scrm.RefreshTable();
                    bool isBetting = true;
                    while (isBetting)
                    {
                        Console.Write($"{tableOfPlayers[i].Name} have {tableOfPlayers[i].Cash}, how much do you want to bet? : ");
                        string input = Console.ReadLine();
                        int x;
                        if (int.TryParse(input, out x) && x <= tableOfPlayers[i].Cash && x >= 1 && input.Length <= 3)
                        {
                            tableOfPlayers[i].BettingCash = x;
                            tableOfPlayers[i].Cash -= x;
                            isBetting = false;
                        }
                        else
                        {
                            Console.WriteLine($"{input} is not a valid option. Max Bet is $999");
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
                    for (int k = 1; k < tableOfPlayers.Length; k++)
                    {
                        if (tableOfPlayers[k] != null && tableOfPlayers[k].BettingCash > 0)
                        {
                            cd.DrawACard(tableOfPlayers[k], tableDeck);
                            Thread.Sleep(240);
                            scrm.RefreshTable();
                        }
                    }
                    cd.DrawACard(tableOfPlayers[0], tableDeck);          //Ge kort till dealern
                    Thread.Sleep(240);
                    scrm.RefreshTable();
                }
            }
        }

        //Metod för att låt<a en spelare spela/satsa färdigt sin hand
        public void PlayYourHand(Player p1)
        {
            string playerChoice = "";
            bool isRunning = true;
            p1.isActivePlayer = true;
            Stack<Cards> activeDeck;
            

            //Välj vilken deck som ska användas
            if (!p1.isActivePlayerSplit)
            {
                activeDeck = p1.myCards;
            }
            else
            {
                p1.isActivePlayerSplit = true;
                p1.isActivePlayer = false;
                activeDeck = p1.myCardsSplit;
            }
            if (activeDeck.Count ==2 && tb.CheckHandValue(activeDeck) <= 11)
            {
                scrm.RefreshTable();
                Console.SetCursorPosition(0, 27);
                bool isDubbling = true;
                while (isDubbling && p1.Cash >= p1.BettingCash)
                {
                    Console.Write($"Do you want to dubbeldown? : ");
                    string answerDubbel = Console.ReadLine().ToLower();
                    if (answerDubbel == "y")
                    {
                        cd.DrawACard(p1, tableDeck);
                        isDubbling = false;
                        isRunning = false;
                        string textBox = p1.isActivePlayerSplit ? p1.TextSplit = "Dubbeldown" : p1.Text = "Dubbeldown";
                        if (p1.isActivePlayerSplit)
                        {
                            p1.Cash -= p1.BettingCashSplit;
                            p1.BettingCashSplit += p1.BettingCashSplit; 
                        }
                        else
                        {
                            p1.Cash -= p1.BettingCash;
                            p1.BettingCash += p1.BettingCash;
                        }
                        scrm.RefreshTable();
                    }
                    else if (answerDubbel == "n")
                    {
                        
                        isDubbling = false;
                    }
                    else
                    {
                        Console.Write($"{answerDubbel} is not a valid option y/n please.");
                    }
                }
            }
            while (isRunning && activeDeck.Count() > 0)
            {

                scrm.RefreshTable();
                Console.SetCursorPosition(0, 27);
                if (tb.IsBlackJack(activeDeck))
                {
                    string textBox = p1.isActivePlayerSplit ? p1.TextSplit = "BLACKJACK" : p1.Text = "BLACKJACK";
                    scrm.RefreshTable();
                    Console.WriteLine($"Congrats {p1.Name} you have BLACKJACK!");
                    Thread.Sleep(2000);
                    isRunning = false;
                }
                else 
                {
                    Console.Write($"{p1.Name} your handvalue is {tb.CheckHandValue(activeDeck)} Do you want to draw a card? (Y/N) : ");
                    playerChoice = Console.ReadLine().ToLower();

                    if (playerChoice == "y")
                    {
                        cd.DrawACard(p1, tableDeck);
                        if (tb.IsFat(activeDeck))
                        {
                            string textBox = p1.isActivePlayerSplit ? p1.TextSplit = "Busted" : p1.Text = "Busted";
                            scrm.RefreshTable();
                            Console.WriteLine("You busted!");
                            Thread.Sleep(2000);
                            isRunning = false;
                            p1.isFat = true;
                        }
                    }
                    else if (playerChoice == "n")
                    {
                        string textBox = p1.isActivePlayerSplit ? p1.TextSplit = "Stands" : p1.Text = "Stands";
                        scrm.RefreshTable();
                        isRunning = false;
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"{playerChoice} is not a valid input!");
                    }
                }
            }
            //Spela den andra handen, om du har splittat
            if (p1.isActivePlayerSplit)
            {
                p1.isActivePlayerSplit = false;
                p1.isActivePlayer = true;
                PlayYourHand(p1);
            }
            p1.isActivePlayer = false;
        }
    }
}
