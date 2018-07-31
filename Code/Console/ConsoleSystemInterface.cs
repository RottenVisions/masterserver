using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RottenVisions.Networking.MasterServer;
using RottenVisions.Tools;
using System.Threading;
using Random = System.Random;

namespace RottenVisions.Windows
{

    public class ConsoleSystemInterface : MonoBehaviour
    {
        #region ASCII Names

        private string[] bannersOld = new string[]
        {
            @"__________                                                            
    ___                              _                   
   /   |  _____________  ____  _____(_____  ____         
  / /| | / ___/ ___/ _ \/ __ \/ ___/ / __ \/ __ \        
 / ___ |(__  / /__/  __/ / / (__  / / /_/ / / / /        
/_/ _|_/____/\___/\___/_/ /_/____/_/\____/_/_/_/         
   / | / ___  / /__      ______  _____/ /__(_____  ____ _
  /  |/ / _ \/ __| | /| / / __ \/ ___/ //_/ / __ \/ __ `/
 / /|  /  __/ /_ | |/ |/ / /_/ / /  / ,< / / / / / /_/ / 
/_/ |_/\___/\__/ |__/|__/\____/_/  /_/|_/_/_/ /_/\__, /  
                                                /____/   
¸,ø¤º°`°º¤ø,¸¸,ø¤º° [я๏ţţ€ɲ ˅ɨ$ɨ๏ɲ$ 2015] °º¤ø,¸¸,ø¤º°`°º¤ø,¸",
            @"   ____                                          
   _____                                    .__                   
  /  _  \   ______ ____  ____   ____   _____|__| ____   ____      
 /  /_\  \ /  ____/ ____/ __ \ /    \ /  ___|  |/  _ \ /    \     
/    |    \\___ \\  \__\  ___/|   |  \\___ \|  (  <_> |   |  \    
\____|__  /____  >\___  \___  |___|  /____  |__|\____/|___|  /    
 _______\/     \/ __  \/    \/     \/     \__   .__        \/     
 \      \   _____/  |___  _  _____________|  | _|__| ____   ____  
 /   |   \_/ __ \   __\ \/ \/ /  _ \_  __ |  |/ |  |/    \ / ___\ 
/    |    \  ___/|  |  \     (  <_> |  | \|    <|  |   |  / /_/  >
\____|__  /\___  |__|   \/\_/ \____/|__|  |__|_ |__|___|  \___  / 
        \/     \/                              \/       \/_____/  
▁ ▂ ▄ ▅ ▆ ▇ █ [řǿƮƮƹɲ ˅ɨȿɨǿɲȿ 2015] █ ▇ ▆ ▅ ▄ ▂ ▁"
        };

        private string[][] banners = new string[][]
        {
            new string[]
            {
                @"     ___                              _                   ",
                @"    /   |  _____________  ____  _____(_____  ____         ",
                @"   / /| | / ___/ ___/ _ \/ __ \/ ___/ / __ \/ __ \        ",
                @"  / ___ |(__  / /__/  __/ / / (__  / / /_/ / / / /        ",
                @" /_/ _|_/____/\___/\___/_/ /_/____/_/\____/_/_/_/         ",
                @"   / | / ___  / /__      ______  _____/ /__(_____  ____ _ ",
                @"  /  |/ / _ \/ __| | /| / / __ \/ ___/ //_/ / __ \/ __ `/ ",
                @" / /|  /  __/ /_ | |/ |/ / /_/ / /  / ,< / / / / / /_/ /  ",
                @"/_/ |_/\___/\__/ |__/|__/\____/_/  /_/|_/_/_/ /_/\__, /   ",
                @"                                                /____/    "
            },

            new string[]
            {
                @"   _____                                    .__                   ",
                @"  /  _  \   ______ ____  ____   ____   _____|__| ____   ____ ",
                @" /  /_\  \ /  ____/ ____/ __ \ /    \ /  ___|  |/  _ \ /    \     ",
                @"/    |    \\___ \\  \__\  ___/|   |  \\___ \|  (  <_> |   |  \    ",
                @"\____|__  /____  >\___  \___  |___|  /____  |__|\____/|___|  /    ",
                @" _______\/     \/ __  \/    \/     \/     \__   .__        \/     ",
                @" \      \   _____/  |___  _  _____________|  | _|__| ____   ____  ",
                @" /   |   \_/ __ \   __\ \/ \/ /  _ \_  __ |  |/ |  |/    \ / ___\ ",
                @"/    |    \  ___/|  |  \     (  <_> |  | \|    <|  |   |  / /_/  >",
                @"\____|__  /\___  |__|   \/\_/ \____/|__|  |__|_ |__|___|  \___  / ",
                @"        \/     \/                              \/       \/_____/  ",
            }
        };

        private string[] copywrites = new string[]
        {
            @"(¯`·._.·(¯`·._.· Rotten Visions 2018 ·._.·´¯)·._.·´¯)",
        };

        string[] selectedBanner;

        #region Text Animation

        [SerializeField] [Tooltip("Should animation play when starting server?")]
        private bool showAnimation = false;

        [SerializeField] [Tooltip("Animation speed of text in miliseconds")]
        private int animSpeedMili = 50;

        #endregion

        #endregion

        public delegate void ReceiveInput(string input);

        public static ReceiveInput receiveInput;

        private void Start()
        {
            //Selects a random banner from a list of banners
            selectedBanner = banners[new Random().Next(0, banners.Length)];

            if (NetworkMaster.Instance.MastServer)
            {
                if (showAnimation)
                    DrawBannerAnimation(selectedBanner, animSpeedMili, ConsoleColor.DarkGray);

                DrawBanner(selectedBanner, ConsoleColor.DarkGray);
                DrawBannerOneLineRandom(copywrites, ConsoleColor.DarkRed);
                Console.WriteLine("Type 'help' to view all the available commands and further information");
            }
        }

        private void DrawBanner(string[] banner, ConsoleColor fColor = ConsoleColor.White,
            ConsoleColor bColor = ConsoleColor.Black)
        {
            foreach (string line in banner)
                WriteLineToConsole(line, fColor, bColor);
        }

        private void DrawBannerOneLineRandom(string[] banner, ConsoleColor fColor = ConsoleColor.White,
            ConsoleColor bColor = ConsoleColor.Black)
        {
            //Randomly Display a banner
            WriteLineToConsole(banner[new Random().Next(0, banner.Length)], fColor, bColor);

            //Debug.Log(banner[new Random().Next(0, banner.Length)]);
        }

        private void DrawBannerAnimation(string[] banner, int speed, ConsoleColor fColor = ConsoleColor.White,
            ConsoleColor bColor = ConsoleColor.Black)
        {
            //Hide Cursor
            Console.CursorVisible = false;

            Console.WriteLine("\n\n");

            var maxLength = selectedBanner.Aggregate(0, (max, line) => Math.Max(max, line.Length));

            var x = Console.BufferWidth / 2 - maxLength / 2;

            for (int y = -selectedBanner.Length; y < Console.WindowHeight + selectedBanner.Length; y++)
            {
                ConsoleDraw(selectedBanner, x, y, fColor, bColor);
                Thread.Sleep(speed);
            }

            //Show Cursor once animation finishes
            Console.CursorVisible = true;
        }

        /// <summary>
        /// Animation of the text!
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        static void ConsoleDraw(IEnumerable<string> lines, int x, int y, ConsoleColor fColor = ConsoleColor.White,
            ConsoleColor bColor = ConsoleColor.Black)
        {
            if (x > Console.WindowWidth) return;
            if (y > Console.WindowHeight) return;

            var trimLeft = x < 0 ? -x : 0;
            int index = y;

            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;

            var linesToPrint =
                from line in lines
                let currentIndex = index++
                where currentIndex > 0 && currentIndex < Console.WindowHeight
                select new
                {
                    Text = new String(line.Skip(trimLeft)
                        .Take(Math.Min(Console.WindowWidth - x, line.Length - trimLeft)).ToArray()),
                    X = x,
                    Y = y++
                };

            Console.Clear();

            foreach (var line in linesToPrint)
            {
                Console.SetCursorPosition(line.X, line.Y);
                WriteLineToConsole(line.Text, fColor, bColor);
            }
        }

        void OnEnable()
        {
            receiveInput += OnReceiveInput;
        }

        void OnReceiveInput(string input)
        {
            ConvertMasterSeverInput(input);
        }

        void ConvertMasterSeverInput(string input)
        {
            string command = input;
            if (input.Contains(" "))
            {
                command = input.Before(" ");
            }

            string args = input.After(" ");
            //Convert to lowercase to unify
            command = command.ToLowerInvariant();
            Debug.Log(command + "-" + args);
            switch (command)
            {
                case "stop":
                    WriteToConsole(string.Format("Master Server Stopped"), ConsoleColor.Red);
                    NetworkMaster.Instance.Stop();
                    break;
                case "quit":
                    WriteToConsole(string.Format("Quitting Master Server..."), ConsoleColor.Red);
                    NetworkMaster.Instance.Quit();
                    break;
                case "reset":
                    WriteToConsole(string.Format("Master Server Resetting..."), ConsoleColor.Red);
                    NetworkMaster.Instance.Reset();
                    WriteToConsole(string.Format("Done!"), ConsoleColor.Cyan);
                    break;
                case "list":
                    WriteToConsole(string.Format("Listing: "), ConsoleColor.Blue);
                    WriteToConsole(string.Format("{0} ", NetworkMaster.Instance.GetHostsCount()), ConsoleColor.Cyan);
                    WriteToConsole(string.Format("Servers - "), ConsoleColor.Blue);
                    EndConsoleLine();
                    NetworkMaster.Instance.ListHosts();
                    break;
                case "start":
                case "initialize":
                    NetworkMaster.Instance.InitializeServer();
                    WriteToConsole(string.Format("Master Server Initialized Successfully"), ConsoleColor.Cyan);
                    break;
                case "setport":
                    if (string.IsNullOrEmpty(args))
                    {
                        Debug.LogWarning(string.Format("Port: {0} - is not valid, please set a port like 12345", args));
                        break;
                    }

                    NetworkMaster.Instance.SetPort(int.Parse(args));
                    WriteToConsole(string.Format("Master Server Port set to: "), ConsoleColor.Blue);
                    WriteToConsole(string.Format("{0}", NetworkMaster.Instance.GetPort()), ConsoleColor.Red);
                    EndConsoleLine();
                    break;
                case "listport":
                case "port":
                    WriteToConsole(string.Format("Master Server Port: "), ConsoleColor.Blue);
                    WriteToConsole(string.Format("{0}", NetworkMaster.Instance.GetPort()), ConsoleColor.Red);
                    EndConsoleLine();
                    break;
                case "help":
                case "cmds":
                case "commands":
                    WriteLineToConsole(string.Format("" +
                                                     "  <<<Available Commands>>> \n" +
                                                     "<--------------------------> \n" +
                                                     "NOTE: [command2 | command3 | command4] signifies alternate methods of commands \n" +
                                                     "Example: command1 will also do the same as the main command, as will command2 \n" +
                                                     "<--------------------------> \n" +
                                                     "help - Shows the Help Menu [cmds | commands] \n" +
                                                     "start - Starts / Initializes the Master Server [initialize] \n" +
                                                     "reset - Resets the Master Server \n" +
                                                     "quit - Stops the Master Server \n" +
                                                     "list - Lists all the registered servers of the Master Server [stop] \n" +
                                                     "setport - Sets the listening port of the Master Server \n" +
                                                     "listport - Lists the current listening port of the Master Server [port]"),
                        ConsoleColor.Gray);
                    break;
                case "test":

                    break;
                default:
                    Debug.LogWarning(string.Format("Command: {0} - does not exist! Args: {1}", command, args));
                    break;
            }
        }

        public static void WriteLineToConsole(string message, ConsoleColor fColor = ConsoleColor.White,
            ConsoleColor bColor = ConsoleColor.Black)
        {
            Console.BackgroundColor = bColor;
            Console.ForegroundColor = fColor;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteToConsole(string message, ConsoleColor fColor = ConsoleColor.White,
            ConsoleColor bColor = ConsoleColor.Black)
        {
            Console.BackgroundColor = bColor;
            Console.ForegroundColor = fColor;
            Console.Write(message);
            Console.ResetColor();
        }

        public void EndConsoleLine()
        {
            Console.Write("\n");
        }

        public string TrimFromLeftToFirstSpace(string s)
        {
            return s == null || s.IndexOf(' ') == -1
                ? s
                : String.Join(" ", s.Split(' ').Skip(1).ToArray());
        }


    }
}