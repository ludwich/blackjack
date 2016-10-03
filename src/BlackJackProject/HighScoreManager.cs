using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net.NetworkInformation;

namespace BlackJackProject
{

    public class HighScoreManager
    {
        private const string filePath = @"c:\temp\highscore.txt";
        public bool Online { get; set; }

        /// <summary>
        /// Get the HighScore from a file located in c:\temp\highscore.txt
        /// if not found it will create a new one
        /// </summary>
        /// <returns>L</returns>
        public List<HighScore> GetTheScoresFromFile()
        {
            List<HighScore> highScores = new List<HighScore>(10);
            try
            {
                string dirPath = @"c:\temp\";
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                using (StreamReader r = new StreamReader(new FileStream(filePath, FileMode.Open)))
                {
                    string json = r.ReadToEnd();
                    highScores = JsonConvert.DeserializeObject<List<HighScore>>(json);
                }
            }
            catch (FileNotFoundException)
            {
                try
                {
                    string dirPath = @"c:\temp\";
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }

                    using (StreamWriter r = new StreamWriter(new FileStream(filePath, FileMode.Create)))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            highScores.Add(new HighScore("", 0));
                        }
                        string output = JsonConvert.SerializeObject(highScores);
                        r.WriteLine(output);
                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine($"Something went terribly wrong. {e.Message} This should explain some of it.");
                }

                Console.WriteLine("Created new savefile since i couldent locate any.");
            }

            return highScores;
        }

        /// <summary>
        /// Saves the highscores to c:\temp\highscores.txt 
        /// </summary>
        /// <param name="sortedList">Needs a List<HighScores> to run</param>
        public void SaveTheScoresToFile(List<HighScore> sortedList)
        {
            try
            {
                using (StreamWriter r = new StreamWriter(new FileStream(filePath, FileMode.Open)))
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


        /// <summary>
        /// Reads the highscores from either file or MyJson and then checks if you ranked on the top 10
        /// </summary>
        /// <param name="players">Needs an array of player objects</param>
        public async void HighScoreWorthyCheck(Player[] players)
        {
            List<HighScore> TheScores;
            if (Online)
            {
                TheScores = await GetTheScoresFromMyJson();
            }
            else
            {
                TheScores = GetTheScoresFromFile();
            }
            bool isHigscoreNew = false;
            for (int i = 1; i < players.Length; i++)
            {
                if (players[i] != null)
                {
                    if (players[i].Cash> TheScores[9].Cash)
                    {
                        TheScores.Add(new HighScore(players[i].Name, players[i].Cash));
                        isHigscoreNew = true;
                    }
                }

            }

            var sortedList = TheScores.OrderByDescending(x => x.Cash)
                            .Take(10)
                            .ToList();

            if (Online && isHigscoreNew)
            {
                SaveHighScoreMyJson(sortedList);
            }
            else if (!Online && isHigscoreNew)
            {
                SaveTheScoresToFile(sortedList);
            }
        }
        
        /// <summary>
        /// Saves the highsores to MyJson.com. Uses same subsite on all games
        /// </summary>
        /// <param name="sortedList">Needs a List<HighScores></HighScores></param>
        public void SaveHighScoreMyJson(List<HighScore> sortedList)
        {
            Console.Clear();
            var client = new HttpClient();
            var uri = new Uri("https://api.myjson.com/bins/zs7s");


            string json = JsonConvert.SerializeObject(sortedList);
            client.BaseAddress = uri;

            var httpmessage = client.PutAsync(uri, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
            Console.SetCursorPosition(20, 10);
            Console.Write($"Uploading HighScore");
            while (!httpmessage.IsCompleted)
            {
                Console.Write($"*");
                System.Threading.Thread.Sleep(100);
            }
            Console.Write("Done.");
            System.Threading.Thread.Sleep(100);
            Console.Clear();
            httpmessage.Wait();
        }

        /// <summary>
        /// Will download the highscore from MyJson and then desrialize to a List<HighScores>.
        /// </summary>
        /// <returns>returns a List<HighScores></returns>
        public async Task<List<HighScore>> GetTheScoresFromMyJson()
        {
            List<HighScore> highScores = new List<HighScore>();
            try
            {
                Console.Clear();
                var client = new HttpClient();
                var uri = new Uri("https://api.myjson.com/bins/zs7s");
                client.BaseAddress = uri;
                var task = client.GetStringAsync(uri);
                Console.SetCursorPosition(20, 10);
                Console.Write($"Downloading HighScore");
                while (!task.IsCompleted)
                {
                    Console.Write($"*");
                    System.Threading.Thread.Sleep(100);
                }
                Console.Write("Done.");
                System.Threading.Thread.Sleep(100);
                Console.Clear();
                string data = await task;
                highScores = JsonConvert.DeserializeObject<List<HighScore>>(data);
            }
            catch (Exception e)
            {
                highScores = GetTheScoresFromFile();
                Console.WriteLine($"{e.Message} Error");
            }

            return highScores;
        }


        /// <summary>
        /// Indicates whether any network connection is available
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailable()
        {
            return IsNetworkAvailable(0);
        }

        /// <summary>
        /// Indicates whether any network connection is available.
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <param name="minimumSpeed">The minimum speed required. Passing 0 will not filter connection using speed.</param>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailable(long minimumSpeed)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // discard because of standard reasons
                if ((ni.OperationalStatus != OperationalStatus.Up) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                    continue;

                // this allow to filter modems, serial, etc.
                // I use 10000000 as a minimum speed for most cases
                if (ni.Speed < minimumSpeed)
                    continue;

                // discard virtual cards (virtual box, virtual pc, etc.)
                if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                    continue;

                // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }
            return false;
        }
    }
}




