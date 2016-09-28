using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJackProject
{
    public class ScreenManager
    {
        private Player[] players;

        /// <summary>
        /// Ritar upp bordet
        /// </summary>
        /// <param name="p">En array av spelare</param>
        /// 
        public ScreenManager(Player[] p)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            players = p;
        }

        public void RefreshTable()
        {
            Console.Clear();
            foreach (var p in players)
            {
                if (p != null)
                {
                    PrintPlayer(p.myCards.ToArray());
                }
               
            }
        }

        private static void PrintPlayer(Cards[] c)
        {
            if (c.Length > 0)
            {
                string part1 = " " + new string('_', 4);
                string part2 = "|" + GetFace(c[0]) + new string(' ', 2) + "|";
                string part3 = "|" + GetSuit(c[0]) + new string(' ', 3) + "|";
                string part4 = "|" + new string(' ', 4) + "|";
                string part5 = "|" + new string('_', 4) + "|";

                for (int i = 1; i < c.Length; i++)
                {
                    part1 = part1 + new string('_', 3);
                    part2 = "|" + GetFace(c[i]) + part2;
                    part3 = "|" + GetSuit(c[i]) + " " + part3;
                    part4 = "|" + new string(' ', 2) + part4;
                    part5 = "|" + new string('_', 2) + part5;
                }
                Console.WriteLine($"{part1}\n{part2}\n{part3}\n{part4}\n{part5}");
            }
        }

        private static string GetSuit(Cards c)
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

        private static string GetFace(Cards c)
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

    }
}