// Copyright (C) 2015 Gamecube
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SharpBoyAdvance
{
    public unsafe static class Imaging
    {
        #region Graphics

        /// <summary>
        /// Reads an uncompressed 4-bit or 8-bit image with the given length (width * height)
        /// and the given palette which either contains 16 (4-bit) or 256 (8-bit) color entries.
        /// </summary>
        /// <param name="rom"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="is4bpp"></param>
        /// <param name="palette"></param>
        /// <returns></returns>
        public static Bitmap ReadDecodedImage(this Romfile rom, uint offset, int length, bool is4bpp, int tile, Color[] palette)
        {
            if (is4bpp == true)
            {
                #region 4-bits per pixel code
                byte[] data = rom.ReadBytes(length / 2);
                int count = ((length / 2) / 32);

                var bitmap = new Bitmap((tile * 8), ((int)(Math.Ceiling(
                    length / (decimal)tile)) * 8), PixelFormat.Format4bppIndexed);
                int width = bitmap.Width, height = bitmap.Height;
                var bitdata = bitmap.LockBits(new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);

                int stride = bitdata.Stride;
                byte* pntr = bitdata.GetPointer();
                var counter = 0;

                var cpal = bitmap.Palette;
                for (int i = 0; i < 16; i++)
                    cpal.Entries[i] = palette[i];
                bitmap.Palette = cpal;

                for (int y = 0; y < height - 7; y += 8)
                {
                    for (int x = 0; x < width - 7; x += 8)
                    {
                        for (int y2 = 0; y2 < 8; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2 += 2)
                            {
                                byte value = data[counter];
                                bitmap.FastSet4bpp(x + x2 + 1, y + y2, stride, pntr, palette[(value & 0xF0) >> 4]);
                                bitmap.FastSet4bpp(x + x2, y + y2, stride, pntr, palette[value & 0x0F]);
                                counter += 1;
                            };
                        };
                    };
                };

                bitmap.UnlockBits(bitdata);
                return bitmap;
                #endregion
            }
            else
            {
                #region 8-bits per pixel code
                byte[] data = rom.ReadBytes(length);
                int count = (length / 64);

                var bitmap = new Bitmap((tile * 8), ((int)(Math.Ceiling(
                    length / (decimal)tile)) * 8), PixelFormat.Format8bppIndexed);
                int width = bitmap.Width, height = bitmap.Height;
                var bitdata = bitmap.LockBits(new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

                int stride = bitdata.Stride;
                byte* pntr = bitdata.GetPointer();
                var counter = 0;

                var cpal = bitmap.Palette;
                for (int i = 0; i < 256; i++)
                    cpal.Entries[i] = palette[i];
                bitmap.Palette = cpal;

                for (int y = 0; y < height - 7; y += 8)
                {
                    for (int x = 0; x < width - 7; x += 8)
                    {
                        for (int y2 = 0; y2 < 8; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                byte value = data[counter];
                                bitmap.FastSet8bpp(x + x2, y + y2, stride, pntr, palette[value]);
                                counter += 1;
                            };
                        };
                    };
                };

                bitmap.UnlockBits(bitdata);
                return bitmap;
                #endregion
            }
        }

        /// <summary>
        /// Reads a compressed 4-bit or 8-bit image with the given palette
        /// which either contains 16 (4-bit) or 256 (8-bit) color entries.
        /// </summary>
        /// <param name="rom"></param>
        /// <param name="offset"></param>
        /// <param name="is4bpp"></param>
        /// <param name="tile"></param>
        /// <param name="palette"></param>
        /// <returns></returns>
        public static Bitmap ReadEncodedImage(this Romfile rom, uint offset, bool is4bpp, int tile, Color[] palette)
        {
            byte[] data = Decryption.Decode(rom.Binary, offset);
            int length = data.Length;

            if (is4bpp == true)
            {
                #region 4-bits per pixel code
                int count = ((length / 2) / 32);

                var bitmap = new Bitmap((tile * 8), ((int)(Math.Ceiling(
                    length / (decimal)tile)) * 8), PixelFormat.Format4bppIndexed);
                int width = bitmap.Width, height = bitmap.Height;
                var bitdata = bitmap.LockBits(new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);

                int stride = bitdata.Stride;
                byte* pntr = bitdata.GetPointer();
                var counter = 0;

                var cpal = bitmap.Palette;
                for (int i = 0; i < 16; i++)
                    cpal.Entries[i] = palette[i];
                bitmap.Palette = cpal;

                for (int y = 0; y < height - 7; y += 8)
                {
                    for (int x = 0; x < width - 7; x += 8)
                    {
                        for (int y2 = 0; y2 < 8; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2 += 2)
                            {
                                byte value = data[counter];
                                bitmap.FastSet4bpp(x + x2 + 1, y + y2, stride, pntr, palette[(value & 0xF0) >> 4]);
                                bitmap.FastSet4bpp(x + x2, y + y2, stride, pntr, palette[value & 0x0F]);
                                counter += 1;
                            };
                        };
                    };
                };

                bitmap.UnlockBits(bitdata);
                return bitmap;
                #endregion
            }
            else
            {
                #region 8-bits per pixel code
                int count = (length / 64);

                var bitmap = new Bitmap((tile * 8), ((int)(Math.Ceiling(
                    length / (decimal)tile)) * 8), PixelFormat.Format8bppIndexed);
                int width = bitmap.Width, height = bitmap.Height;
                var bitdata = bitmap.LockBits(new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

                int stride = bitdata.Stride;
                byte* pntr = bitdata.GetPointer();
                var counter = 0;

                var cpal = bitmap.Palette;
                for (int i = 0; i < 256; i++)
                    cpal.Entries[i] = palette[i];
                bitmap.Palette = cpal;

                for (int y = 0; y < height - 7; y += 8)
                {
                    for (int x = 0; x < width - 7; x += 8)
                    {
                        for (int y2 = 0; y2 < 8; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                byte value = data[counter];
                                bitmap.FastSet8bpp(x + x2, y + y2, stride, pntr, palette[value]);
                                counter += 1;
                            };
                        };
                    };
                };

                bitmap.UnlockBits(bitdata);
                return bitmap;
                #endregion
            }
        }

        /// <summary>
        /// Writes an uncompressed image at the given offset.
        /// </summary>
        /// <param name="rom"></param>
        /// <param name="bmp"></param>
        /// <param name="offset"></param>
        public static void WriteDecodedImage(this Romfile rom, Bitmap bmp, uint offset)
        {
            int height = bmp.Height;
            int width = bmp.Width;
            int position = 0;

            var rect = new Rectangle(0, 0, width, height);
            var data = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
            var pntr = data.GetPointer();
            var stride = data.Stride;

            if (bmp.PixelFormat == PixelFormat.Format4bppIndexed)
            {
                #region 4-bits per pixel code
                byte[] bytes = new byte[((width * height) / 2)];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        bytes[position] = bmp.FastGet4Bpp(x, y, stride, pntr);
                        position += 1;
                    };
                };

                rom.Offset = offset;
                rom.WriteBytes(bytes);
                #endregion
            }
            else
            {
                #region 8-bits per pixel code
                byte[] bytes = new byte[(width * height)];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        bytes[position] = bmp.FastGet8Bpp(x, y, stride, pntr);
                        position += 1;
                    };
                };

                rom.Offset = offset;
                rom.WriteBytes(bytes);
                #endregion
            }

            bmp.UnlockBits(data);
        }

        /// <summary>
        /// Writes a compressed image at the given offset.
        /// </summary>
        /// <param name="rom"></param>
        /// <param name="bmp"></param>
        /// <param name="offset"></param>
        public static void WriteEncodedImage(this Romfile rom, Bitmap bmp, uint offset)
        {
            int height = bmp.Height;
            int width = bmp.Width;
            int position = 0;

            var rect = new Rectangle(0, 0, width, height);
            var data = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
            var pntr = data.GetPointer();
            var stride = data.Stride;

            if (bmp.PixelFormat == PixelFormat.Format4bppIndexed)
            {
                #region 4-bits per pixel code
                byte[] bytes = new byte[((width * height) / 2)];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        bytes[position] = bmp.FastGet4Bpp(x, y, stride, pntr);
                        position += 1;
                    };
                };

                rom.Offset = offset;
                rom.WriteBytes(Decryption.Encode(bytes));
                #endregion
            }
            else
            {
                #region 8-bits per pixel code
                byte[] bytes = new byte[(width * height)];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        bytes[position] = bmp.FastGet8Bpp(x, y, stride, pntr);
                        position += 1;
                    };
                };

                rom.Offset = offset;
                rom.WriteBytes(Decryption.Encode(bytes));
                #endregion
            }

            bmp.UnlockBits(data);
        }

        #endregion

        #region Palette

        /// <summary>
        /// Reads an uncompressed palette at the given offset.
        /// </summary>
        /// <param name="rom"></param>
        /// <param name="offset"></param>
        /// <param name="colors"></param>
        /// <returns></returns>
        public static Color[] ReadDecodedPalette(this Romfile rom, uint offset, int colors)
        {
            byte[] data = rom.ReadBytes(colors * 2);
            var entries = new Color[colors];
            int processed = 0;

            for (int i = 0; i < colors; i++)
            {
                var hword = (ushort)((data[processed + 1] << 8) | data[processed]);
                int blue = (((hword & 0x7C00) >> 10) * 8);
                int green = (((hword & 0x3E0) >> 5) * 8);
                int red = ((hword & 0x1F) * 8);

                entries[i] = Color.FromArgb(red, green, blue);
                processed += 2;
            };

            return entries;
        }

        /// <summary>
        /// Reads a compressed palette at the given offset.
        /// </summary>
        /// <param name="rom"></param>
        /// <param name="offset"></param>
        /// <param name="colors"></param>
        /// <returns></returns>
        public static Color[] ReadEncodedPalette(this Romfile rom, uint offset, int colors)
        {
            byte[] data = Decryption.Decode(rom.Binary, offset);
            var entries = new Color[colors];
            int processed = 0;

            for (int i = 0; i < colors; i++)
            {
                var hword = (ushort)((data[processed + 1] << 8) | data[processed]);
                int blue = (((hword & 0x7C00) >> 10) * 8);
                int green = (((hword & 0x3E0) >> 5) * 8);
                int red = ((hword & 0x1F) * 8);

                entries[i] = Color.FromArgb(red, green, blue);
                processed += 2;
            };

            return entries;
        }

        /// <summary>
        /// Writes an uncompressed palette at the given offset.
        /// </summary>
        /// <param name="rom"></param>
        /// <param name="palette"></param>
        /// <param name="offset"></param>
        public static void WriteDecodedPalette(this Romfile rom, Color[] palette, uint offset)
        {
            int position = 0;
            int length = palette.Length;
            byte[] bytes = new byte[length * 2];

            for (int i = 0; i < length; i++)
            {
                var color = palette[i];
                byte red = (byte)(Math.Floor(color.R / 8f));
                byte green = (byte)(Math.Floor(color.G / 8f));
                byte blue = (byte)(Math.Floor(color.B / 8f));

                var value = (ushort)(red | (green << 5) | (blue << 10));
                bytes[position + 1] = (byte)((value & 0xFF00) >> 8);
                bytes[position] = (byte)(value & 0xFF);
                position += 2;
            };

            rom.Offset = offset;
            rom.WriteBytes(bytes);
        }

        /// <summary>
        /// Writes a compressed palette at the given offset.
        /// </summary>
        /// <param name="rom"></param>
        /// <param name="palette"></param>
        /// <param name="offset"></param>
        public static void WriteEncodedPalette(this Romfile rom, Color[] palette, uint offset)
        {
                        int position = 0;
            int length = palette.Length;
            byte[] bytes = new byte[length * 2];

            for (int i = 0; i < length; i++)
            {
                var color = palette[i];
                byte red = (byte)(Math.Floor(color.R / 8f));
                byte green = (byte)(Math.Floor(color.G / 8f));
                byte blue = (byte)(Math.Floor(color.B / 8f));

                var value = (ushort)(red | (green << 5) | (blue << 10));
                bytes[position + 1] = (byte)((value & 0xFF00) >> 8);
                bytes[position] = (byte)(value & 0xFF);
                position += 2;
            };

            rom.Offset = offset;
            rom.WriteBytes(Decryption.Encode(bytes));
        }

        #endregion
    }
}