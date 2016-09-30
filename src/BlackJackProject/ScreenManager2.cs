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
            Console.SetCursorPosition(x, y);
            Console.WriteLine("HighScore");
            y += 2;
            for (int i = 1; i < theHighScores.Count; i++)
            {
                Console.SetCursorPosition(x, y);
                Console.Write($"{i}. {theHighScores[i].Name}");
                Console.Write($"{theHighScores[i].Cash}");
                Console.WriteLine();
                y++;
            }
            Console.ReadKey();
        }
    }
}
