﻿using Microsoft.Xna.Framework;

namespace OpenVIII
{
    public sealed partial class Icons
    {
        #region Methods

        /// <summary>
        /// Add Manual hardcoded entries / settings here.
        /// </summary>
        /// <remarks>Maybe one day we can export these to an xml file and read it in on launch.</remarks>
        protected override void InsertCustomEntries()
        {
            if (Entries == null) return;
            var BG = new Entry
            {
                X = 0,
                Y = 48,
                Width = 256,
                Height = 16,
                CustomPalette = 1,
                Fill = Vector2.UnitX,
                Tile = Vector2.UnitY,
            };
            var Border_TopLeft = new Entry
            {
                X = 16,
                Y = 0,
                Width = 8,
                Height = 8,
                CustomPalette = 0,
            };
            var Border_Top = new Entry
            {
                X = 24,
                Y = 0,
                Width = 8,
                Height = 8,
                Tile = Vector2.UnitX,
                Offset = new Vector2(8, 0),
                End = new Vector2(-8, 0),
                CustomPalette = 0
            };
            var Border_Bottom = new Entry
            {
                X = 24,
                Y = 16,
                Width = 8,
                Height = 8,
                Tile = Vector2.UnitX,
                Snap_Bottom = true,
                Offset = new Vector2(8, -8),
                End = new Vector2(-8, 0),
                CustomPalette = 0
            };
            var Border_TopRight = new Entry
            {
                X = 32,
                Y = 0,
                Width = 8,
                Height = 8,
                Snap_Right = true,
                Offset = new Vector2(-8, 0),
                CustomPalette = 0
            };
            var Border_Left = new Entry
            {
                X = 16,
                Y = 8,
                Width = 8,
                Height = 8,
                Tile = Vector2.UnitY,
                Offset = new Vector2(0, 8),
                End = new Vector2(0, -8),
                CustomPalette = 0
            };
            var Border_Right = new Entry
            {
                X = 32,
                Y = 8,
                Width = 8,
                Height = 8,
                Tile = Vector2.UnitY,
                Snap_Right = true,
                Offset = new Vector2(-8, 8),
                End = new Vector2(0, -8),
                CustomPalette = 0
            };
            var Border_BottomLeft = new Entry
            {
                X = 16,
                Y = 16,
                Width = 8,
                Height = 8,
                Snap_Bottom = true,
                Offset = new Vector2(0, -8),
                CustomPalette = 0
            };
            var Border_BottomRight = new Entry
            {
                X = 32,
                Y = 16,
                Width = 8,
                Height = 8,
                Snap_Bottom = true,
                Snap_Right = true,
                Offset = new Vector2(-8, -8),
                CustomPalette = 0
            };

            Entries[ID.Bar_BG] = new EntryGroup(new Entry
            {
                X = 16,
                Y = 24,
                Width = 8,
                Height = 8,
                Tile = Vector2.UnitX,
                Fill = Vector2.UnitY,
                CustomPalette = 2, // 2 in 2019, was set to 0 for 2013
            });
            Entries[ID.Bar_Fill] = new EntryGroup(new Entry
            {
                X = 0,
                Y = 16,
                Width = 8,
                Height = 8,
                Tile = Vector2.UnitX,
                Fill = Vector2.UnitY,
                //Offset = new Vector2(2, 2),
                //End = new Vector2(-2, 0),
                CustomPalette = 5
            });
            Entries[ID.MenuBorder] = new EntryGroup(Border_Top, Border_Left, Border_Right, Border_Bottom, Border_TopLeft, Border_TopRight, Border_BottomLeft, Border_BottomRight);
            Entries[ID.Menu_BG_256] = new EntryGroup(BG, Border_Top, Border_Left, Border_Right, Border_Bottom, Border_TopLeft, Border_TopRight, Border_BottomLeft, Border_BottomRight);
            Entries[ID.Menu_BG_368] = new EntryGroup(BG, new Entry
            {
                X = 0,
                Y = 64,
                Offset = new Vector2(256, 0), //offset should be 256 but i had issue with 1 pixel gap should be able to get away with losing one pixel.
                Width = 112,
                Height = 16,
                CustomPalette = 1,
                Fill = Vector2.UnitX,
                Tile = Vector2.UnitY
            }, Border_Top, Border_Left, Border_Right, Border_Bottom, Border_TopLeft, Border_TopRight, Border_BottomLeft, Border_BottomRight);

            Entries[ID.DEBUG] = new EntryGroup(
                new Entry { X = 128, Y = 24, Width = 7, Height = 8, Offset = new Vector2(4, 0) },
                new Entry { X = 65, Y = 8, Width = 6, Height = 8, Offset = new Vector2(7 + 4, 0) },
                new Entry { X = 147, Y = 24, Width = 6, Height = 8, Offset = new Vector2(13 + 4, 0) },
                new Entry { X = 141, Y = 24, Width = 6, Height = 8, Offset = new Vector2(19 + 4, 0) },
                new Entry { X = 104, Y = 16, Width = 6, Height = 8, Offset = new Vector2(25 + 4, 0) }
                );

            //2 pages
            GeneratePages(ID.COMMAND, ID.COMMAND_PG1, 4, 4);
            //4 pages
            GeneratePages(ID.ABILITY, ID.ABILITY_PG1, 4, 8);
            //4 pages for 16 gfs
            GeneratePages(ID.GF, ID.GF_PG1, 4, 8);
            //13 pages for 50 spells
            GeneratePages(ID.MAGIC, ID.MAGIC_PG1, 13, 8);

            //18 pages for 198 Items
            //50 pages if squashed to 4 items per page.
            GeneratePages(ID.ITEM, ID.ITEM_PG1, 50, -4);
            //16 spells 4 pages for blue magic
            GeneratePages(ID.SPECIAL, ID.SPECIAL_PG1, 4, 8);

            //revese order of rewind so arrows draw correctly
            if (Entries.ContainsKey(ID.Rewind_Fast))
            {
                var _RR_0 = Entries[ID.Rewind_Fast][0].Clone();
                var _RR_1 = Entries[ID.Rewind_Fast][1].Clone();

                Entries[ID.Rewind_Fast] = new EntryGroup(_RR_1, _RR_0);
            }

            //override this entry to make it tile instead of have set number of elements.
            if (Entries.ContainsKey(ID.Size_08x64_Bar))
            {
                var b = Entries[ID.Size_08x64_Bar];
                var Left = b[0].Clone();
                var Center = b[1].Clone();
                var Right = b[7].Clone();
                Left.Offset = Vector2.Zero;
                Center.Offset = Vector2.Zero;
                Right.Offset = new Vector2(-8f, 0);
                Center.Tile = Vector2.UnitX;
                Right.Snap_Right = true;
                Entries[ID.Size_08x64_Bar] = new EntryGroup(Center, Left, Right);
                var tmp = ID.D_Pad_Up;
                Entries[tmp] = new EntryGroup(Entries[tmp][0], Entries[tmp][2], Entries[tmp][1], Entries[tmp][3]);
                tmp = ID.D_Pad_Down;
                Entries[tmp] = new EntryGroup(Entries[tmp][0], Entries[tmp][2], Entries[tmp][1], Entries[tmp][3]);
                tmp = ID.D_Pad_Left;
                Entries[tmp] = new EntryGroup(Entries[tmp][0], Entries[tmp][2], Entries[tmp][1], Entries[tmp][3]);
                tmp = ID.D_Pad_Right;
                Entries[tmp] = new EntryGroup(Entries[tmp][0], Entries[tmp][2], Entries[tmp][1], Entries[tmp][3]);
            }
        }

        private void GeneratePages(ID label, ID label_pg1, byte count, sbyte offset)
        {
            if (!Entries.ContainsKey(label_pg1) && !Entries.ContainsKey(label) && !Entries.ContainsKey(ID.Size_08x08_P_))
                return;

            count = (byte)MathHelper.Clamp(count, 1, 99);
            var P_ = Entries[ID.Size_08x08_P_][0].Clone();
            P_.Offset.X += Entries[label][0].Width + offset;
            P_.CustomPalette = 2;

            var _ = new Entry[10];
            _[1] = Entries[ID.Num_8x8_1_1][0].Clone();
            _[1].Offset.X += P_.Offset.X + P_.Width + 2;
            _[1].CustomPalette = 7;
            for (byte i = 2; i <= 9 && i <= count; i++)
            {
                _[i] = Entries[ID.Num_8x8_1_1 + i - 1][0].Clone();
                _[i].Offset.X = _[1].Offset.X;
                _[i].CustomPalette = _[1].CustomPalette;
            }
            Entry[] __ = null;
            if (count > 9)
            {
                __ = new Entry[10];
                __[0] = Entries[ID.Num_8x8_1_0][0].Clone();
                __[0].Offset.X += P_.Offset.X + P_.Width + 2 + _[1].Width;
                __[0].CustomPalette = 7;
                for (byte i = 1; i <= 9; i++)
                {
                    __[i] = _[i].Clone();
                    __[i].Offset.X = __[0].Offset.X;
                }
            }
            for (byte i = 1; i <= count; i++)
            {
                if (i < 10)
                    Entries[label_pg1 + i - 1] = new EntryGroup(Entries[label][0], P_, _[i]);
                else if (i >= 10 && __ != null)
                    Entries[label_pg1 + i - 1] = new EntryGroup(Entries[label][0], P_, _[i / 10], __[i % 10]);
            }
        }

        #endregion Methods
    }
}