﻿using System;

namespace OpenVIII
{
    public class ATBTimer
    {
        private bool First = true;

        public int ATBBarIncrement { get; private set; } = 0;
        public float ATBBarPos { get; private set; } = 0;
        public bool Done => Percent >= 1f;
        public float Percent
        {
            get
            {
                var percent = Math.Abs(ATBBarPos / Damageable.ATBBarSize);
                return percent > 1f ? 1f : percent;
            }
        }

        private Damageable Damageable;

        public ATBTimer(Damageable damageable) => Refresh(damageable);

        /// <summary>
        /// Refresh damageable and start new turn. if Damageable is changed.
        /// </summary>
        /// <param name="damageable">Character,GF,Enemy</param>
        public void Refresh(Damageable damageable)
        {
            if (damageable != Damageable)
            {
                Damageable = damageable;
                FirstTurn();
            }
        }

        /// <summary>
        /// Start new turn.
        /// </summary>
        public void NewTurn()
        {
            if (First)
            {
                ATBBarPos = Damageable?.ATBBarStart() ?? 0;
                First = false;
            }
            else
                ATBBarPos = 0;
        }

        /// <summary>
        /// Start over.
        /// </summary>
        public void FirstTurn()
        {
            First = true;
            Damageable?.Charging();
            NewTurn();
        }

        /// <summary>
        /// Reset to defaultState
        /// </summary>
        public void Reset() => FirstTurn();

        public bool Update()
        {
            if (Damageable == null || Damageable.IsDead)
            {
                return false;
            }

            if (ATBBarPos > 0)
            {
                ATBBarPos = 0;
                return true;
            }

            if (Done) 
            {
                return false;
            }

            var TotalMilliseconds = Memory.ElapsedGameTime.TotalMilliseconds;
            ATBBarIncrement = Damageable.BarIncrement(); // 60 ticks per second.
            ATBBarPos += checked((float)(ATBBarIncrement * TotalMilliseconds / 60));
            // if TotalMilliseconds is 1000 then it'll increment 60 times. So this should be right.
            return true;
        }
    }
}
