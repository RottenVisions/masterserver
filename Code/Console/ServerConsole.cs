using System;
using RottenVisions.Windows;
using UnityEngine;

namespace RottenVisions.Windows
{
    public class ServerConsole : MonoBehaviour
    {
        public string title = "Desysia Master Server";
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

        ConsoleWindow console = new ConsoleWindow();
        ConsoleInput input = new ConsoleInput();

        string strInput;

        public static bool created;

        public static bool Created
        {
            get { return created; }
        }

        //
        // Create console window, register callbacks
        //
        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            console.Initialize();
            console.SetTitle(title);

            input.OnInputText += OnInputText;

            Application.logMessageReceived += HandleLog;

            created = true;

            //Debug.Log("Console Started");
        }

        //
        // Text has been entered into the console
        // Run it as a console command
        //
        void OnInputText(string obj)
        {
            ConsoleSystemInterface.receiveInput(obj);
        }

        //
        // Debug.Log* callback
        //
        void HandleLog(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Warning)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (type == LogType.Error)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (type == LogType.Assert)
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            else
                Console.ForegroundColor = ConsoleColor.White;

            // We're half way through typing something, so clear this line ..
            if (Console.CursorLeft != 0)
                input.ClearLine();

            Console.WriteLine(message);

            // If we were typing something re-add it.
            input.RedrawInputLine();
        }

        public static void ConsoleWrite(string msg, ConsoleColor color)
        {
            //Cache
            var cachedColor = Console.ForegroundColor;
            //Set
            Console.ForegroundColor = color;
            //Write
            Console.WriteLine(msg);
            //Reset
            Console.ForegroundColor = cachedColor;
        }

        //
        // Update the input every frame
        // This gets new key input and calls the OnInputText callback
        //
        void Update()
        {
            input.Update();
        }

        //
        // It's important to call console.ShutDown in OnDestroy
        // because compiling will error out in the editor if you don't
        // because we redirected output. This sets it back to normal.
        //
        void OnDestroy()
        {
            console.Shutdown();
        }

#endif
    }
}