using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJackProject
{
    public enum Suits { Hearts, Spades, Diamonds, Clubs };
    public enum Face { Ace =1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, J, Q, K }
    public class Cards
    {
        public Cards(int color, int value)
        {
            Suits = (Suits)color;
            Face = (Face)value;
            Value = value;

        }

        public Suits Suits { get; set; }
        public Face Face { get; set; }
        private int _value;

        public int Value
        {
            get { return _value; }
            set
            {
                if (value > 10)
                {
                    _value = 10;
                }
                else
                {
                    _value = value;
                }
            }
        }




        public static Stack<Cards> GetAFreshDeck()
        {
            List<Cards> AFreshDeck = new List<Cards>(312);
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 1; k < 14 ; k++)
                    {
                        AFreshDeck.Add(new Cards(j, k));
                    }
                }
            }
            Stack<Cards> NewStackOfCards =  Shuffle(AFreshDeck);
            return NewStackOfCards;
        }

        static Random _random = new Random();
        public static Stack<Cards> Shuffle(List<Cards> list)
        {
            Stack<Cards> MyCards = new Stack<Cards>(312);
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                Cards value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            for (int i = 0; i < list.Count; i++)
            {
                MyCards.Push(list[i]);
            }
            return MyCards;
        }

        public static void DrawACard(Player p, Stack<Cards> deck)
        {
    
            p.myCards.Push(deck.Pop());
        }
    



    }
}
