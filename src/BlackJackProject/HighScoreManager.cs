using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlackJackProject
{

    public class HighScoreManager
    {
        private const string filePath = @"c:\temp\highscore.txt";
        public List<HighScore> GetTheScores()
        {
            List<HighScore> highScores = new List<HighScore>(10);
            try
            {
                using (StreamReader r = new StreamReader(new FileStream(@"c:\temp\highscore.txt", FileMode.Open)))
                {

                    string json = r.ReadToEnd();
                    highScores = JsonConvert.DeserializeObject<List<HighScore>>(json);
                    //return highScores;
                }
            }
            catch (FileNotFoundException)
            {
                try
                {
                    using (StreamWriter r = new StreamWriter(new FileStream(@"c:\temp\highscore.txt", FileMode.Create)))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            highScores.Add(new HighScore("", 0));
                        }
                        string output = JsonConvert.SerializeObject(highScores);
                        r.WriteLine(output);
                    }
                }
                catch (Exception)
                {

                    Console.WriteLine("Something went terribly wrong ");
                }

                Console.WriteLine("Created new savefile since i couldent locate any.");
            }

            return highScores;
        }

        public void HighScoreWorthyCheck(Player[] players)
        {
            var TheScores = GetTheScores();
            for (int i = 1; i < players.Length; i++)
            {
                if (players[i] != null)
                {
                    if (players[i].Cash > TheScores[9].Cash)
                    {
                        TheScores.Add(new HighScore(players[i].Name, players[i].Cash));
                    }
                }

            }

            var sortedList = TheScores.OrderByDescending(x => x.Cash)
                            .Take(10)
                            .ToList();
            foreach (var item in sortedList)
            {
                Console.WriteLine($"{item.Name} {item.Cash} ");
            }

            try
            {
                using (StreamWriter r = new StreamWriter(new FileStream(@"c:\temp\highscore.txt", FileMode.Open)))
                {

                    string output = JsonConvert.SerializeObject(sortedList);
                    r.WriteLine(output);

                }
            }
            catch (Exception e)
            {

                Console.WriteLine($" {e.Message} fucked it up");
            }

        }
    }
}




