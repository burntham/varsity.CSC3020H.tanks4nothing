using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace tanks4nothing
{
    /// <summary>
    /// Class used for the timer
    /// </summary>
    class Timer
    {

        //Time (Seconds each game should run)
        int timeS;
        DateTime beginingTime;
        DateTime currentTime;
        int totalPaused;
        int tempPauseTime;
        DateTime startPaused;
        bool paused = false;
        int test;

        public Timer(int timeS_)
        {
            beginingTime = DateTime.Now;
            totalPaused = 0;
            timeS = timeS_;
            test = timeS;
            update();
        }

        public void update()
        {
            currentTime = DateTime.Now;
        }

        public int elapsedTime()
        {
            return paused ? 1 : ((timeS - (int)(currentTime.Subtract(beginingTime)).TotalSeconds) + totalPaused);;
        }

        public void startPause()
        {
            if (!paused)
            {
                startPaused = DateTime.Now;
                paused = true;
            }

        }

        public void stopPause()
        {
            tempPauseTime = 0;
            tempPauseTime = (int)DateTime.Now.Subtract(startPaused).TotalSeconds;
            totalPaused += tempPauseTime;
            //Console.WriteLine(totalPaused);
            paused = false;

        }



    }
}
