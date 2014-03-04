#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
//using LightSavers.Utils;
#endregion

namespace tanks4nothing.Game.Sound
{
    /// <summary>
    /// Sound Effect Manager - Used to manage instances of each sound effect - Will also change the pitch of sounds playing back.
    /// </summary>
    public class SoundEffectInstanceManager
    {
        Player p1;
        Player p2;

        SoundEffect sound;
        SoundEffectInstance[] instances;
        float volume;
        bool paused = false, isMusic;
        int numInstances, currentInstance=0;

        public SoundEffectInstanceManager(SoundEffect sound, int instances, float volume, bool isMusic)
        {
            this.sound = sound;
            this.volume = volume;
            this.instances = new SoundEffectInstance[instances];
            this.isMusic = isMusic;
            this.numInstances = instances;
        }

        public void updatePitch()
        {
            //Timer has more than 15 seconds left: for point differences between players of 
            //more than 5,10,20,40 and 70 the pitch will be raised or lowered (player is winning 
            //or losing) 0.2,0.4,0.6,0.8, 1 octaves respectively

            int difference = Global.PlayerScores[0] - Global.PlayerScores[1];
            if (difference != 0)
            {
                int p1Winning = (difference > 0) ? 1 : -1;
                float pitch;

                if (Global.timeLeft >= 15)
                {
                    pitch = (difference > 70) ? 1.0f : (difference > 40) ? 0.8f : (difference > 20) ? 0.6f : (difference > 10) ? 0.4f : (difference > 5) ? 0.2f : 0.0f;
                }
                else
                {
                    //If the timer has less than 15 seconds left: for point differences between players of 
                    //more than 2,5,10,25 and 45 the pitch will be raised or lowered (player is winning 
                    //or losing) 0.2,0.4,0.6,0.8, 1 octaves respectively.
                    pitch = (difference > 45) ? 1.0f : (difference > 25) ? 0.8f : (difference > 10) ? 0.6f : (difference > 5) ? 0.4f : (difference > 2) ? 0.2f : 0.0f;
                }

                Global.pitch[0] = p1Winning * pitch;
                Global.pitch[1] = p1Winning * -1 * pitch;
            }
            else
            {
                Global.pitch[0] = 0.0f; Global.pitch[1] = 0.0f;
            }
            


        }

        //Play the sound associated with the manager
        //Insure all playing instances are accounted for so that they can be paused
        //only creates instances that are needed, and reuses them if possible
        /// <summary>
        /// Plays and instance sound
        /// </summary>
        /// <param name="player">Player 1 or 2? (3 if other) - this affects pitch</param>
        public void playSound(int player_)
        {
            updatePitch();

            int player = (player_ != 3) ? player_ - 1 : player_;
            //possible next instance (if current is playing)
            int possibleInstance = (currentInstance + 1) % numInstances;

            if (instances[currentInstance] == null)
            {
                instances[currentInstance] = sound.CreateInstance();
                //Play around with pitch here
                instances[currentInstance].Pitch =Global.pitch[player];
                //instances[currentInstance].Volume = TankGame.Volume;
                instances[currentInstance].Play();
            }
            else if (instances[currentInstance].State == SoundState.Playing)
            {
                if (instances[possibleInstance] == null || instances[possibleInstance].State== SoundState.Stopped)
                {
                    if(instances[possibleInstance]==null)
                        instances[possibleInstance] = sound.CreateInstance();
                    //instances[possibleInstance].Volume = TankGame.Volume;
                    //Play around with pitch here
                    instances[currentInstance].Pitch = Global.pitch[player];
                    instances[possibleInstance].Play();
                    currentInstance = possibleInstance;
                }else if (instances[possibleInstance].State == SoundState.Playing)
                    return;
            }
            else
            {
                instances[currentInstance].Volume = TankGame.Volume;
                instances[currentInstance].Pitch = Global.pitch[player];
                instances[currentInstance].Play();
                currentInstance = (++currentInstance)%numInstances;
            }
            
        }

        //public void PlayLoopSingle()
        //{
        //    instances[0] = sound.CreateInstance();
        //    instances[0].Volume = TankGame.Volume;
        //    instances[0].IsLooped = true;
        //    instances[0].Play();
        //}

        ///// <summary>
        ///// Pause
        ///// </summary>
        //public void Pause()
        //{
        //    for (int i = 0; i < numInstances; ++i)
        //        if (instances[i]!=null)
        //        {
        //            instances[i].Volume = TankGame.Volume;
        //            if (instances[i].State == SoundState.Playing)
        //                instances[i].Pause();
        //        }
        //}

        ///// <summary>
        ///// Resume
        ///// </summary>
        //public void Resume()
        //{
        //    for (int i = 0; i < numInstances; ++i)
        //        if (instances[i] != null)
        //        {
        //            if (instances[i].State == SoundState.Paused)
        //            {
        //                instances[i].Resume();
        //                //instances[i].Pitch = Global.pitch[player];
        //                instances[i].Volume = TankGame.Volume;
        //            }
        //        }
        //}

        //public void resetVolume()
        //{
        //    foreach(SoundEffectInstance sEffect in instances)
        //    {
        //        if(sEffect!=null)
        //            sEffect.Volume = TankGame.Volume;
        //    }
        //}
    }
}
