using System;

namespace Kee5Engine
{
    class Program
    {

        [System.Runtime.InteropServices.DllImport("nvapi64.dll", EntryPoint = "fake")]
        static extern int LoadNvApi64();

        [System.Runtime.InteropServices.DllImport("nvapi.dll", EntryPoint = "fake")]
        static extern int LoadNvApi32();

        /// <summary>
        /// Remove that pesky Intel
        /// </summary>
        private static void RemoveIntel()
        {
            try
            {
                if (Environment.Is64BitProcess)
                    LoadNvApi64();
                else
                    LoadNvApi32();
            }
            catch { } // will always fail since 'fake' entry point doesn't exists
        }

        /// <summary>
        /// Run the game!
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            RemoveIntel();
            Game game = new Game();
        }
    }
}
