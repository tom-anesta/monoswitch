using System;

namespace monoswitchExample
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (exampleGame game = new exampleGame())
            {
                game.Run();
            }
        }
    }
#endif
}

