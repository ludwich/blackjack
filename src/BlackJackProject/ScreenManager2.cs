using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJackProject
{
    partial class ScreenManager
    {
        public async void DrawTheHighScores(HighScoreManager hs)
        {
            Console.Clear();
            List<HighScore> theHighScores;
           
            if (hs.Online)
            {
                var theList = await hs.GetTheScoresFromMyJson();
                theHighScores = theList;
            }
            else 
            {
                theHighScores = hs.GetTheScoresFromFile();
            }
            
            int x = 30, y = 10;
            Console.SetCursorPosition(x+10, y);
            Console.WriteLine("HighScore");
            y += 2;
            for (int i = 0; i < theHighScores.Count; i++)
            {
                Console.SetCursorPosition(x, y);
                Console.Write($"{i+1}. {theHighScores[i].Name}");
                Console.SetCursorPosition(x+20, y);
                Console.Write($"{theHighScores[i].Cash}");
                Console.WriteLine();
                y++;
            }
            Console.ReadKey();
        }

        public async void SetTheHighScores(HighScoreManager hs) //5x20
        {
            List<HighScore> theHighScores;

            if (hs.Online)
            {
                var theList = await hs.GetTheScoresFromMyJson();
                theHighScores = theList;
            }
            else
            {
                theHighScores = hs.GetTheScoresFromFile();
            }

            string name, cash, tmp;

            for (int i = 0; i < 5; i++)
            {
                tmp = theHighScores[i].Cash.ToString();
                name = theHighScores[i].Name + new string(' ', 10 - theHighScores[i].Name.Length);

                if (tmp.Length < 10)
                {
                    cash = tmp + new string(' ', 9 - tmp.Length);
                }
                else
                {
                    cash = tmp.Substring(0, 9);
                }

                hsTable[i] = $"{name} ${cash}";
            }         
        }
    }
}
