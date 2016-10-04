using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace BlackJackProject
{
    partial class ScreenManager
    {
        private TableManager tb;
        private Player[] players;
        private string[][] screen = new string[3][] { new string[25], new string[25], new string[25] };
        private HighScoreManager hs;
        private string[] hsTable = { new string(' ', 20), new string(' ', 20), new string(' ', 20), new string(' ', 20), new string(' ', 20) };

        public ScreenManager(TableManager t, Player[] p, HighScoreManager h)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            tb = t;
            players = p;
            SetSpacesInit();
            hs = h;
            SetTheHighScores(hs);
            PrintInfoLeft();
            PrintTheHighScores();
        }

        public void RefreshTable()
        {
            Console.Clear();
            SetScreen(players[0], 1, 0);
            SetScreen(players[1], 2, 9);
            SetScreen(players[2], 1, 11);
            SetScreen(players[3], 0, 9);
            PrintScreen();
        }

        // Sets spaces that cant be updated
        private void SetSpacesInit()
        {
            SetSpaces(0, 0, 1);
            SetSpaces(0, 7, 2);
            SetSpaces(0, 23, 2);
            SetSpaces(1, 7, 4);
            SetSpaces(2, 8, 1);
            SetSpaces(2, 23, 2);
        }

        private void SetSpaces(int c, int r, int l)
        {
            for (int i = r; i < r + l; i++)
            {
                screen[c][i] = new string(' ', 26);
            }
        }

        private void PrintTheHighScores() //8x26
        {

            screen[2][0] = " \u250C" + new string('\u2500', 23) + "\u2510";
            screen[2][1] = " \u2502" + new string(' ', 7) + "Highscore" + new string(' ', 7) + "\u2502";
            for (int i = 0; i < hsTable.Length; i++)
            {
                screen[2][i + 2] = $" \u2502{i + 1} {hsTable[i]}\u2502";
            }
            screen[2][7] = " \u2514" + new string('\u2500', 23) + "\u2518";
        }

        private void PrintInfoLeft() //8x26
        {
            screen[0][1] = new string(' ', 4) + "\u250C" + new string('\u2500', 16) + "\u2510" + new string(' ', 4);
            screen[0][2] = new string(' ', 4) + "\u2502 Jonthieboi's   \u2502" + new string(' ', 4);
            screen[0][3] = new string(' ', 4) + "\u2502  Blackjack     \u2502" + new string(' ', 4);
            screen[0][4] = new string(' ', 4) + "\u2502   Min bet $1   \u2502" + new string(' ', 4);
            screen[0][5] = new string(' ', 4) + "\u2502   Max bet $999 \u2502" + new string(' ', 4);
            screen[0][6] = new string(' ', 4) + "\u2514" + new string('\u2500', 16) + "\u2518" + new string(' ', 4);
        }

        private void SetScreen(Player p, int c, int r)
        {
            if (p == null)
            {
                SetSpaces(c, r, 14);
            }
            else
            {
                SetBox(p, c, r, false);

            }
        }

        private void SetBox(Player p, int c, int r, bool s)
        {
            string[] tmp;
            Stack<Cards> tmpCards, tmpValue;
            int tmpBettingCash, j;
            string tmpText;
            bool tmpActivePlayer, dealerHideCard = p.isDealer && !p.isActivePlayer && p.myCards.Count == 2;

            if (s) // vid split
            {
                tmpCards = p.myCardsSplit;
                tmpValue = tmpCards;
                tmpActivePlayer = p.isActivePlayerSplit;
                tmpBettingCash = p.BettingCashSplit;
                tmpText = p.TextSplit;
                j = 2;
            }
            else
            {
                tmpActivePlayer = p.isActivePlayer;
                tmpCards = p.myCards;

                if (dealerHideCard) // fixar value på dealern pga. ett kort är dolt
                {
                    var tmpCardsForValue = new Stack<Cards>();
                    tmpCardsForValue.Push(p.myCards.ElementAt<Cards>(1));
                    tmpValue = tmpCardsForValue;
                }
                else
                {
                    tmpValue = tmpCards;
                }

                tmpBettingCash = p.BettingCash;
                tmpText = p.Text;

                j = (p.isDealer) ? j = 1 : j = 0;

            }

            tmp = GetCardParts(tmpCards.ToArray(), dealerHideCard);
            var parts = new string[8 - j];

            if (!s)
            {
                parts[0] = new string(' ', 5) + GetName(p.Name) + new string(' ', 11);

                if (!p.isDealer)
                {
                    parts[1] = new string(' ', 5) + GetBalance(p.Cash) + new string(' ', 9);
                }
            }

            parts[2 - j] = GetValue(tmpValue) + tmp[0];
            parts[3 - j] = GetArrow(tmpActivePlayer) + tmp[1];
            parts[4 - j] = new string(' ', 2) + tmp[2];
            parts[5 - j] = new string(' ', 2) + tmp[3];
            parts[6 - j] = new string(' ', 2) + tmp[4];
            parts[7 - j] = GetBet(tmpBettingCash) + " " + GetText(tmpText);

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

            Console.WriteLine("\u250C" + new string('\u2500', 80) + "\u2510");
            for (int i = 0; i < 25; i++)
            {
                Console.WriteLine("\u2502 " + screen[0][i] + screen[1][i] + screen[2][i] + " \u2502");
            }
            Console.WriteLine("\u2514" + new string('\u2500', 80) + "\u2518");
        }

        private string[] GetCardParts(Cards[] c, bool d) // 5 x 24
        {

            var parts = new string[5];

            if (d) // gömma andra hålkortet
            {
                parts[0] = "\u250C" + new string('\u2500', 2) + "\u252C" + new string('\u2500', 4) + "\u2510" + new string(' ', 15);
                parts[1] = "\u2502" + GetFace(c[1]) + "\u2502" + new string(' ', 4) + "\u2502" + new string(' ', 15);
                parts[2] = "\u2502" + GetSuit(c[1]) + " \u2502" + new string(' ', 4) + "\u2502" + new string(' ', 15);
                parts[3] = "\u2502" + new string(' ', 2) + "\u2502" + new string(' ', 4) + "\u2502" + new string(' ', 15);
                parts[4] = "\u2514" + new string('\u2500', 2) + "\u2534" + new string('\u2500', 4) + "\u2518" + new string(' ', 15);
            }

            else if (c.Length > 0)
            {
                parts[0] = "\u250C" + new string('\u2500', 4) + "\u2510";
                parts[1] = "\u2502" + GetFace(c[0]) + new string(' ', 2) + "\u2502";
                parts[2] = "\u2502" + GetSuit(c[0]) + new string(' ', 3) + "\u2502";
                parts[3] = "\u2502" + new string(' ', 4) + "\u2502";
                parts[4] = "\u2514" + new string('\u2500', 4) + "\u2518";

                for (int i = 1; i < c.Length; i++)
                {
                    parts[0] = "\u250C" + new string('\u2500', 2) + parts[0];
                    parts[1] = "\u2502" + GetFace(c[i]) + parts[1];
                    parts[2] = "\u2502" + GetSuit(c[i]) + " " + parts[2];
                    parts[3] = "\u2502" + new string(' ', 2) + parts[3];
                    parts[4] = "\u2514" + new string('\u2500', 2) + parts[4];
                }

                if (c.Length > 1)
                {
                    for (int i = 1; i < c.Length; i++)
                    {                        
                        StringBuilder sb = new StringBuilder(parts[0]);
                        StringBuilder sb2 = new StringBuilder(parts[4]);
                        sb[i * 3] = '\u252C';
                        sb2[i * 3] = '\u2534';
                        parts[0] = sb.ToString();
                        parts[4] = sb2.ToString();
                    }                    
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
            return n + new string(' ', 10 - n.Length);
        }

        private string GetBalance(double b) // 1 x 12
        {
            string tmp = b.ToString();
            return (tmp.Length < 12) ? "$" + tmp + new string(' ', 11 - tmp.Length) : "$" + tmp.Substring(0, 11);
        }

        private string GetValue(Stack<Cards> s) // 1 x 2
        {
            int v = tb.CheckHandValue(s);
            return (s == null || v == 0) ? new string(' ', 2) : new string(' ', 2 - v.ToString().Length) + v.ToString();
        }

        private string GetArrow(bool a) // 1 x 2
        {
            return (a) ? "->" : new string(' ', 2);

        }

        private string GetBet(int b) // 1 x 4
        {
            return (b == 0) ? new string(' ', 4) : "$" + b.ToString() + new string(' ', 3 - b.ToString().Length);

        }

        private string GetText(string t) // 1 x 21
        {
            return (t == null) ? new string(' ', 21) : t + new string(' ', 21 - t.Length);

        }
    }
}