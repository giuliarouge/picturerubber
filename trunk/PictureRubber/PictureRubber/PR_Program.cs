using System;

namespace PictureRubber
{
    static class PR_Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread()]
        static void Main(string[] args)
        {
            PR_Main game = PR_Main.GetInstance();
            game.Run();
        }
    }
}

