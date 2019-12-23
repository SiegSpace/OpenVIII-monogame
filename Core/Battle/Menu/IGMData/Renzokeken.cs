﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenVIII.Encoding.Tags;
using System;

namespace OpenVIII.IGMData
{
    public class Renzokeken : IGMData.Base
    {
        #region Fields

        private const int Renzokeken_Gradient_Width = 192;
        private static object locker = new object();
        private static Texture2D pixel;
        private static Color Renzokenken_Seperator_Color;
        private static Color Renzokenken_Seperator_Color_Faded;
        private byte _count = 0;
        private int _hits = 7;
        private double delayMS;
        private Slide<Color> HitSlider = new Slide<Color>(Color.White, Color.TransparentBlack, HitTime, Color.Lerp);
        private Color newattack;

        #endregion Fields

        #region Properties

        private static TimeSpan HitTime => TimeSpan.FromMilliseconds(300d);
        private static TimeSpan RenzoDelay => TimeSpan.FromMilliseconds(500d);
        private static TimeSpan RenzoTime => TimeSpan.FromMilliseconds(2000d);

        #endregion Properties

        #region Methods

        public static Renzokeken Create(Rectangle? pos = null) => Create<Renzokeken>(15, 1, new IGMDataItem.Box { Pos = pos ?? new Rectangle(24, 501, 912, 123), Title = Icons.ID.SPECIAL }, 0, 0, Memory.State?[Characters.Squall_Leonhart]);

        public static void ThreadUnsafeOperations()
        {
            lock (locker)
            {
                if (pixel == null)
                {
                    pixel = new Texture2D(Memory.graphics.GraphicsDevice, 1, 1);
                    pixel.SetData(new Color[] { Color.White });
                }
                Renzokenken_Seperator_Color = Memory.Icons.MostSaturated(Icons.ID.Renzokeken_Seperator, 6);
                Renzokenken_Seperator_Color_Faded = Memory.Icons.MostSaturated(Icons.ID.Renzokeken_Seperator, 2);
            }
        }

        public override bool Inputs()
        {
            skipsnd = true;
            if (Input2.DelayedButton(FF8TextTagKey.RotateRight))
            {
                Inputs_OKAY();
                return true;
            }
            return base.Inputs();
        }

        public override bool Inputs_CANCEL() => base.Inputs_OKAY();

        public override bool Inputs_OKAY()
        {
            foreach (Menu_Base i in ITEM)
            {
                if (i?.GetType() == typeof(IGMDataItem.Gradient.Renzokeken))
                {
                    if (((IGMDataItem.Gradient.Renzokeken)i).Trigger)
                    {
                        skipsnd = true;
                        base.Inputs_OKAY();
                        HitSlider.Restart();
                        i.Hide();
                        _count++;
                        break;
                    }
                }
            }

            return true;
        }

        public override void Reset()
        {
            Hide();
            ITEM[Count - 5, 0].Hide();
            ITEM[Count - 6, 0].Hide();
            _count = 0;
            delayMS = 0d;
            base.Reset();
        }

        public void Reset(int hits)
        {
            _hits = hits;
            Refresh();
            Reset();
        }

        public override bool Update()
        {
            if (!Enabled) return false;
            bool done = false;
            bool hot = false;

            ((IGMDataItem.Icon)ITEM[Count - 5, 0]).Data = Icons.ID._0_Hit_ + _count;
            ((IGMDataItem.Texture)ITEM[Count - 4, 0]).Color = HitSlider.Update();

            int hotcnt = 0;
            int pos = 0;
            foreach (Menu_Base i in ITEM)
            {
                if (i?.GetType() == typeof(IGMDataItem.Gradient.Renzokeken) && pos++ < _hits)
                {
                    IGMDataItem.Gradient.Renzokeken gr = (IGMDataItem.Gradient.Renzokeken)i;
                    done = !gr.Done || done;
                    hot = gr.Trigger || hot;
                    if (gr.Done)
                        hotcnt++;
                }
            }
            if (!done)
            {
                if ((delayMS += Memory.gameTime.ElapsedGameTime.TotalMilliseconds) > 1000)
                {
                    //Damageable.EndTurn(); //gets stuck if the current player isn't squall
                    Menu.BattleMenus.GetCurrentBattleMenu().Damageable.EndTurn();
                }
            }
            if (hot)
            {
                ((IGMDataItem.Icon)ITEM[Count - 3, 0]).Palette = 6;
                ((IGMDataItem.Icon)ITEM[Count - 2, 0]).Palette = 6;
                ((IGMDataItem.Texture)ITEM[0, 0]).Color = Renzokenken_Seperator_Color;
                ITEM[Count - 1, 0].Show();
                ITEM[Count - 5, 0].Hide();
                ITEM[Count - 6, 0].Hide();
            }
            else
            {
                ((IGMDataItem.Icon)ITEM[Count - 3, 0]).Palette = 2;
                ((IGMDataItem.Icon)ITEM[Count - 2, 0]).Palette = 2;
                ((IGMDataItem.Texture)ITEM[0, 0]).Color = Renzokenken_Seperator_Color_Faded;
                if ((hotcnt >= _hits) || !done)// && ITEM[Count - 1, 0].Enabled)
                {
                    if (_count >= _hits)
                        ITEM[Count - 6, 0].Show();
                    else if (_count > 0)
                        ITEM[Count - 5, 0].Show();
                }
                //else if (hotcnt > 0) Debug.WriteLine(hotcnt);

                ITEM[Count - 1, 0].Hide();
            }
            base.Update();
            return true;
        }

        protected override void Init()
        {
            base.Init();
            ThreadUnsafeOperations();

            Memory.Icons[Icons.ID.Renzokeken_Seperator][0].Offset = Vector2.Zero;

            EntryGroup split = Memory.Icons[Icons.ID.Renzokeken_Seperator];
            EntryGroup e = Memory.Icons[Icons.ID.Renzokeken_Seperator];
            Rectangle r = CONTAINER.Pos; //new Rectangle(40, 524, 880, 84);
            r.Inflate(-16, -20);
            r.X += r.X % 4;
            r.Y += r.Y % 4;
            r.Width += r.Width % 4;
            r.Height += r.Height % 4;
            ITEM[0, 0] = new IGMDataItem.Texture { Data = pixel, Pos = r, Color = Renzokenken_Seperator_Color_Faded };
            r.Inflate(-4, -4);
            ITEM[1, 0] = new IGMDataItem.Texture { Data = pixel, Pos = r, Color = Color.Black };
            float scale = (float)r.Height / e.Height;
            int w = (int)(e.Width * scale);
            ITEM[Count - 3, 0] = new IGMDataItem.Icon { Data = Icons.ID.Renzokeken_Seperator, Pos = new Rectangle(r.X + 80, r.Y, w, r.Height), Scale = new Vector2(scale) };
            ITEM[Count - 2, 0] = new IGMDataItem.Icon { Data = Icons.ID.Renzokeken_Seperator, Pos = new Rectangle(r.X + 208, r.Y, w, r.Height), Scale = new Vector2(scale) };
            Rectangle hotspot = new Rectangle(r.X + 80 + (w / 2), r.Y + 4, 208 - 80, r.Height - 8);
            ITEM[Count - 4, 0] = new IGMDataItem.Texture { Data = pixel, Pos = hotspot, Color = Color.TransparentBlack };
            //Rectangle hotspotbox = hotspot;
            hotspot.Width += (int)(hotspot.Width * .50f);
            Rectangle tr = new Rectangle(r.X + 208 + (w / 2), r.Y + 4, 0, r.Height - 4);

            Memory.Icons[Icons.ID.Trigger_][0].Offset = Vector2.Zero;

            e = Memory.Icons[Icons.ID.Trigger_];
            scale = ((float)r.Height - 8) / e.Height;
            w = (int)(e.Width * scale);
            int trigwidtharea = (r.Right - tr.Left);
            int xbak = tr.X;
            tr.X += trigwidtharea / 2 - w / 2;

            ITEM[Count - 1, 0] = new IGMDataItem.Icon { Data = Icons.ID.Trigger_, Pos = tr, Palette = 6, Scale = new Vector2(scale) };// { Color = rc};

            newattack = new Color(104, 80, 255);
            Rectangle pos = new Rectangle(r.X, r.Y + 4, Renzokeken_Gradient_Width, r.Height - 8);
            r.Inflate(-4, -4);
            for (int x = 0; x <= _hits && x <= 7; x++)
                ITEM[2 + x, 0] = IGMDataItem.Gradient.Renzokeken.Create(pos, newattack, Renzokenken_Seperator_Color, 1f, hotspot, r, time: RenzoTime, TimeSpan.FromTicks(RenzoDelay.Ticks * x));
            float totalx = 0;
            for (byte i = 0; i <= 7; i++)
            {
                e = Memory.Icons[Icons.ID._0_Hit_ + i];
                totalx += e[0].Offset.X;
            }
            float avgx = (float)Math.Round(totalx / 8);
            for (byte i = 0; i <= 7; i++)
            {
                Memory.Icons[Icons.ID._0_Hit_ + i][0].Offset.X = avgx;
            }
            e = Memory.Icons[Icons.ID._0_Hit_];
            scale = ((float)r.Height) / e.Height;
            w = (int)(e.Width * scale);
            tr.X = xbak + trigwidtharea / 2 - w / 2;
            ITEM[Count - 5, 0] = new IGMDataItem.Icon { Data = Icons.ID._0_Hit_, Pos = tr, Scale = new Vector2(scale) };

            e = Memory.Icons[Icons.ID.Perfect__];
            scale = ((float)r.Height) / e.Height;
            w = (int)(e.Width * scale);
            tr.X = xbak + trigwidtharea / 2 - w / 2;
            ITEM[Count - 6, 0] = new IGMDataItem.Icon { Data = Icons.ID.Perfect__, Pos = tr, Palette = 8, Scale = new Vector2(scale) };

            Reset();
            Cursor_Status = Cursor_Status.Enabled | Cursor_Status.Static | Cursor_Status.Hidden;
        }

        protected override void RefreshChild()
        {
            int pos = 0;
            foreach (Menu_Base i in ITEM)
            {
                if (i.GetType() == typeof(IGMDataItem.Gradient.Renzokeken))
                {
                    IGMDataItem.Gradient.Renzokeken rg = (IGMDataItem.Gradient.Renzokeken)i;
                    if (pos++ < _hits)
                        rg.Show();
                    else
                        rg.Hide();
                }
            }
            base.RefreshChild();
        }

        #endregion Methods
    }
}