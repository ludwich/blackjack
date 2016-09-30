using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJackProject
{
    public class ScreenManager
    {
        private TableManager tb;
        private Player[] players;
        private string[][] screen = new string[3][] { new string[25], new string[25], new string[25] };

        public ScreenManager(TableManager t, Player[] p)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            tb = t;
            players = p;

            SetSpaces(0, 0, 9);
            SetSpaces(0, 23, 2);
            SetSpaces(1, 7, 4);
            SetSpaces(2, 0, 9);
            SetSpaces(2, 23, 2);
        }

        public void RefreshTable()
        {
            /* testing-skit
            players[1].myCardsSplit = players[2].myCards;
            players[1].Text = "BLACKJACK!!";
            players[1].BettingCashSplit = 5;
            players[2].myCardsSplit = players[2].myCards;
            players[2].Text = "u won $10!!";
            players[2].BettingCashSplit = 50;
            players[3].myCardsSplit = players[2].myCards;
            players[3].TextSplit = "nice split!!";
            players[3].BettingCashSplit = 15;
            players[3].isActivePlayerSplit = true;
            */

            Console.Clear();
            SetScreen();
            PrintScreen();
        }

        public void SetSpaces(int c, int r, int l)
        {
            for (int i = r; i < r + l; i++)
            {
                screen[c][i] = new string(' ', 26);
            }
        }

        private void SetScreen()
        {
            // Dealer
            SetBox(players[0], 1, 0, false);

            // Player 1           
            if (players[1] == null)
            {
                SetSpaces(2, 9, 14);
            }
            else
            {
                SetBox(players[1], 2, 9, false);

            }

            // Player 2
            if (players[2] == null)
            {
                SetSpaces(1, 11, 14);
            }
            else
            {
                SetBox(players[2], 1, 11, false);

            }

            // Player 3
            if (players[3] == null)
            {
                SetSpaces(0, 9, 14);
            }
            else
            {
                SetBox(players[3], 0, 9, false);
            }
        }

        private void SetBox(Player p, int c, int r, bool s)
        {
            string[] tmp;
            Stack<Cards> tmpCards;
            bool tmpActivePlayer;
            int tmpBettingCash;
            string tmpText;
            int j;

            if (s) // vid split
            {
                tmpCards = p.myCardsSplit;
                tmpActivePlayer = p.isActivePlayerSplit;
                tmpBettingCash = p.BettingCashSplit;
                tmpText = p.TextSplit;
                j = 2;
            }
            else
            {
                tmpCards = p.myCards;
                tmpActivePlayer = p.isActivePlayer;
                tmpBettingCash = p.BettingCash;
                tmpText = p.Text;

                if (p.isDealer) //ändra till att kolla om underklassen är av Dealer
                {
                    j = 1;
                }
                else
                {
                    j = 0;
                }
            }

            tmp = GetCardParts(tmpCards.ToArray());
            var parts = new string[8 - j];

            if (!s)
            {
                parts[0] = new string(' ', 5) + GetName(p.Name) + new string(' ', 11);

                if (!p.isDealer)
                {
                    parts[1] = new string(' ', 5) + GetBalance(p.Cash) + new string(' ', 9);
                }
            }

            parts[2 - j] = GetValue(tmpCards) + tmp[0];
            parts[3 - j] = GetArrow(tmpActivePlayer) + tmp[1];
            parts[4 - j] = new string(' ', 2) + tmp[2];
            parts[5 - j] = new string(' ', 2) + tmp[3];
            parts[6 - j] = new string(' ', 2) + tmp[4];
            parts[7 - j] = new string(' ', 2) + GetBet(tmpBettingCash) + " " + GetText(tmpText);

            int k = 0;
            for (int i = r; i < r + parts.Length; i++)
            {
                screen[c][i] = parts[k];
                k++;
            }

            if (p.isDealer) // Vi har ritat ut dealern, avsluta
            {
                return;
            }

            if (s) // Vi har ritat ut splitten, avsluta
            {
                return;
            }

            if (p.myCardsSplit != null) // Split finns på spelaren
            {
                SetBox(p, c, r + 8, true);
            }

            else
            {
                SetSpaces(c, r + 8, 6);
            }

        }

        private void PrintScreen()
        {

            Console.WriteLine(" " + new string('_', 80));
            for (int i = 0; i < 25; i++)
            {
                Console.WriteLine("| " + screen[0][i] + screen[1][i] + screen[2][i] + " |");
            }
            Console.WriteLine("|" + new string('_', 80) + "|");
        }

        private string[] GetCardParts(Cards[] c) // 5 x 24
        {

            var parts = new string[5];

            if (c.Length > 0)
            {
                parts[0] = " " + new string('_', 4);
                parts[1] = "|" + GetFace(c[0]) + new string(' ', 2) + "|";
                parts[2] = "|" + GetSuit(c[0]) + new string(' ', 3) + "|";
                parts[3] = "|" + new string(' ', 4) + "|";
                parts[4] = "|" + new string('_', 4) + "|";

                for (int i = 1; i < c.Length; i++)
                {
                    parts[0] = parts[0] + new string('_', 3);
                    parts[1] = "|" + GetFace(c[i]) + parts[1];
                    parts[2] = "|" + GetSuit(c[i]) + " " + parts[2];
                    parts[3] = "|" + new string(' ', 2) + parts[3];
                    parts[4] = "|" + new string('_', 2) + parts[4];
                }

                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = parts[i] + new string(' ', 24 - parts[i].Length);
                }
            }

            else
            {
                var b = new string(' ', 24);
                parts = new string[5] { b, b, b, b, b };
            }

            return parts;

        }

        private string GetSuit(Cards c) // 1 x 1
        {
            switch (c.Suits)
            {
                case Suits.Spades:
                    return "\u2660";
                case Suits.Clubs:
                    return "\u2663";
                case Suits.Diamonds:
                    return "\u2666";
                case Suits.Hearts:
                    return "\u2665";
                default:
                    return "";
            }
        }

        private string GetFace(Cards c) // 1 x 2
        {
            switch (c.Face)
            {
                case Face.Ace:
                    return "A ";
                case Face.Two:
                    return "2 ";
                case Face.Three:
                    return "3 ";
                case Face.Four:
                    return "4 ";
                case Face.Five:
                    return "5 ";
                case Face.Six:
                    return "6 ";
                case Face.Seven:
                    return "7 ";
                case Face.Eight:
                    return "8 ";
                case Face.Nine:
                    return "9 ";
                case Face.Ten:
                    return "10";
                case Face.J:
                    return "J ";
                case Face.Q:
                    return "Q ";
                case Face.K:
                    return "K ";
                default:
                    return "";
            }
        }

        private string GetName(string n) // 1 x 10
        {
            if (n == null)
            {
                return new string(' ', 10);
            }
            else
            {
                return n + new string(' ', 10 - n.Length);
            }
        }

        private string GetBalance(double b) // 1 x 12
        {
            if (b == -1)
            {
                return new string(' ', 12);
            }
            else
            {
                string tmp = b.ToString();

                if (tmp.Length < 12)
                {
                    return "$" + tmp + new string(' ', 11 - tmp.Length);
                }
                else
                {
                    return "$" + tmp.Substring(0, 11);
                }


            }
        }

        private string GetValue(Stack<Cards> s) // 1 x 2
        {
            int v = tb.CheckHandValue(s);
            if (s == null || v == 0)
            {
                return new string(' ', 2);
            }
            else
            {
                return new string(' ', 2 - v.ToString().Length) + v.ToString();
            }
        }

        private string GetArrow(bool a) // 1 x 2
        {
            if (a)
            {
                return "->";
            }
            else
            {
                return new string(' ', 2);
            }
        }

        private string GetBet(int b) // 1 x 4
        {
            if (b == 0)
            {
                return new string(' ', 4);
            }
            else
            {
                return "$" + b.ToString() + new string(' ', 3 - b.ToString().Length);
            }
        }

        private string GetText(string t) // 1 x 19
        {
            if (t == null)
            {
                return new string(' ', 19);
            }
            else
            {
                return t + new string(' ', 19 - t.Length);
            }

        }


    }
}