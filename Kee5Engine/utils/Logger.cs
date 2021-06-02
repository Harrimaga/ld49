using System;
using System.Collections.Generic;
using System.Text;

namespace Kee5Engine.utils
{
    public enum LogType
    {
        SUCCESS,
        INFO,
        WARNING,
        CRITICAL
    }

    public class Logger
    {
        private string _name;
        private List<Tuple<string, LogType>> _writables;

        public Logger(string name)
        {
            _name = name;
            _writables = new List<Tuple<string, LogType>>();
        }

        public void Log(string msg, LogType type)
        {
            _writables.Add(new Tuple<string, LogType>(msg, type));
        }

        private void PushToConsole()
        {
            foreach (Tuple<string, LogType> msg in _writables)
            {
                switch (msg.Item2)
                {
                    case LogType.SUCCESS:
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case LogType.INFO:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case LogType.WARNING:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogType.CRITICAL:
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }

                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {_name.ToUpper()}: {msg.Item1}");
            }

            _writables.Clear();
            Console.ResetColor();
        }

        public void Update()
        {
            PushToConsole();
        }
    }
}
