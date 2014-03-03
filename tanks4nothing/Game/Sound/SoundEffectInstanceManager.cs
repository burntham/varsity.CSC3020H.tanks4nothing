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

        //Play the sound associated with the manager
        //Insure all playing instances are accounted for so that they can be paused
        //only creates instances that are needed, and reuses them if possible
        public void playSound()
        {
            //possible next instance (if current is playing)
            int possibleInstance = (currentInstance + 1) % numInstances;

            if (instances[currentInstance] == null)
            {
                instances[currentInstance] = sound.CreateInstance();
                //Play around with pitch here
                instances[currentInstance].Pitch = -1.0f;
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
                    instances[currentInstance].Pitch = -1.0f;
                    instances[possibleInstance].Play();
                    currentInstance = possibleInstance;
                }else if (instances[possibleInstance].State == SoundState.Playing)
                    return;
            }
            else
            {
                instances[currentInstance].Volume = TankGame.Volume;
                instances[currentInstance].Play();
                currentInstance = (++currentInstance)%numInstances;
            }            
        }

        public void PlayLoopSingle()
        {
            instances[0] = sound.CreateInstance();
            instances[0].Volume = TankGame.Volume;
            instances[0].IsLooped = true;
            instances[0].Play();
        }

        /// <summary>
        /// Pause
        /// </summary>
        public void Pause()
        {
            for (int i = 0; i < numInstances; ++i)
                if (instances[i]!=null)
                {
                    instances[i].Volume = TankGame.Volume;
                    if (instances[i].State == SoundState.Playing)
                        instances[i].Pause();
                }
        }

        /// <summary>
        /// Resume
        /// </summary>
        public void Resume()
        {
            for (int i = 0; i < numInstances; ++i)
                if (instances[i] != null)
                {
                    if (instances[i].State == SoundState.Paused)
                    {
                        instances[i].Resume();
                        instances[i].Pitch = 0.2f;
                        instances[i].Volume = TankGame.Volume;
                    }
                }
        }

        public void resetVolume()
        {
            foreach(SoundEffectInstance sEffect in instances)
            {
                if(sEffect!=null)
                    sEffect.Volume = TankGame.Volume;
            }
        }
    }
}
