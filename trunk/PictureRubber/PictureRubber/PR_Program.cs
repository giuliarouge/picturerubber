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
            using (PR_Main game = new PR_Main())
            {
                game.Run();
            }
        }
    }
}

