﻿using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ACSE.Classes.Utilities
{
    public static class PatternUtility
    {
        private static byte[] DecompressC4(byte[] C4ImageData)
        {
            byte[] DecompressedData = new byte[C4ImageData.Length * 2];
            int DecompressIdx = 0;

            for (int i = 0; i < C4ImageData.Length; i++)
            {
                DecompressIdx = i * 2;
                DecompressedData[DecompressIdx] = (byte)(C4ImageData[i] >> 4);
                DecompressedData[DecompressIdx + 1] = (byte)(C4ImageData[i] & 0x0F);
            }

            return DecompressedData;
        }

        private static byte[] CompressC4(byte[] ImageData)
        {
            byte[] CompressedData = new byte[ImageData.Length / 2];
            int Index = 0;
            byte CondensedPixel = 0;

            for (int i = 0; i < CompressedData.Length; i++)
            {
                Index = i * 2;
                CondensedPixel = (byte)(((ImageData[Index] & 0x0F) << 4) | (ImageData[Index + 1] & 0x0F));
                CompressedData[i] = CondensedPixel;
            }

            return CompressedData;
        }

        private static byte[] C4ImageSubroutineDecode(byte[] C4ImageData, uint Width = 32, uint Height = 32)
        {
            uint BlockXCount = Width / 8;
            uint BlockYCount = Width / 8;

            byte[] OutputBuffer = new byte[C4ImageData.Length];
            uint PixelIndex = 0;

            for (int YBlock = 0; YBlock < BlockYCount; YBlock++)
            {
                for (int XBlock = 0; XBlock < BlockXCount; XBlock++)
                {
                    for (int YPixel = 0; YPixel < 8; YPixel++)
                    {
                        for (int XPixel = 0; XPixel < 8; XPixel++)
                        {
                            int OutputBufferIndex = (int)((Width * 8 * YBlock) + YPixel * Width + XBlock * 8 + XPixel);
                            OutputBuffer[OutputBufferIndex] = C4ImageData[PixelIndex];
                            PixelIndex++;
                        }
                    }
                }
            }

            return OutputBuffer;
        }

        private static byte[] C4ImageSubroutineEncode(byte[] C4ImageData, uint Width = 32, uint Height = 32)
        {
            uint BlockXCount = Width / 8;
            uint BlockYCount = Width / 8;

            byte[] OutputBuffer = new byte[C4ImageData.Length];
            uint OutputBufferIndex = 0;

            for (int YBlock = 0; YBlock < BlockYCount; YBlock++)
            {
                for (int XBlock = 0; XBlock < BlockXCount; XBlock++)
                {
                    for (int YPixel = 0; YPixel < 8; YPixel++)
                    {
                        for (int XPixel = 0; XPixel < 8; XPixel++)
                        {
                            int PixelIndex = (int)((Width * 8 * YBlock) + YPixel * Width + XBlock * 8 + XPixel);
                            OutputBuffer[OutputBufferIndex] = C4ImageData[PixelIndex];
                            OutputBufferIndex++;
                        }
                    }
                }
            }

            return OutputBuffer;
        }

        public static byte[] DecodeC4(byte[] C4ImageData, uint Width = 32, uint Height = 32)
        {
            return C4ImageSubroutineDecode(DecompressC4(C4ImageData), Width, Height);
        }

        public static byte[] EncodeC4(byte[] ImageData, uint Width = 32, uint Height = 32)
        {
            return CompressC4(C4ImageSubroutineEncode(ImageData, Width, Height));
        }

        public static Bitmap C4PaletteMapToBitmap(byte[] DecodedC4ImageData, uint[] Palette, uint Width = 32, uint Height = 32)
        {
            byte[] PatternBitmapBuffer = new byte[4 * Width * Height];

            for (int i = 0; i < DecodedC4ImageData.Length; i++)
                Buffer.BlockCopy(BitConverter.GetBytes(Palette[DecodedC4ImageData[i] - 1]), 0, PatternBitmapBuffer, i * 4, 4);

            Bitmap Pattern_Bitmap = new Bitmap(32, 32, PixelFormat.Format32bppArgb);
            BitmapData bitmapData = Pattern_Bitmap.LockBits(new Rectangle(0, 0, 32, 32), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(PatternBitmapBuffer, 0, bitmapData.Scan0, PatternBitmapBuffer.Length);
            Pattern_Bitmap.UnlockBits(bitmapData);

            return Pattern_Bitmap;
        }
    }
}
