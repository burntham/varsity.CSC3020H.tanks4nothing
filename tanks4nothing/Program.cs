using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace tanks4nothing
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application. - will launch if on xbox or windows
        /// </summary>
        static void Main(string[] args)
        {
            using (TankGame game = new TankGame())
            {
                game.Run();
            }
        }
    }
#endif
}

