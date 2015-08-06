using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;

namespace OctoAwesome.Runtime
{
    internal class UpdateDomain
    {
        private IUniverse universe;

        private Stopwatch watch;
        private Thread thread;

        public List<ActorHost> ActorHosts { get; private set; }

        public bool Running { get; set; }

        public WorldState State { get; private set; }

        public UpdateDomain(Stopwatch watch)
        {
            this.watch = watch;
            ActorHosts = new List<ActorHost>();

            Running = true;
            State = WorldState.Running;

            thread = new Thread(UpdateLoop);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.BelowNormal;
            thread.Start();
        }

        private void UpdateLoop()
        {
            TimeSpan lastCall = new TimeSpan();
            TimeSpan frameTime = new TimeSpan(0, 0, 0, 0, 16);
            while (Running)
            {
                GameTime gameTime = new GameTime(
                    watch.Elapsed, frameTime); 
                lastCall = watch.Elapsed;

                foreach (var actorHost in ActorHosts)
                    actorHost.Update(gameTime);

                var timeDiff = frameTime - (watch.Elapsed - lastCall);
                if (timeDiff.Ticks > 0)
                    Thread.Sleep(timeDiff);
            }
        }
    }
}
