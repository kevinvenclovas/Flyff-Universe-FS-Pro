﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace FlyffUAutoFSPro._Script
{
    public class BitmapSearcher
    {
        public static List<Rectangle> SearchMultipleBitmap(Bitmap smallBitmap, Bitmap bigBitmap, double tolerance)
        {
            var rectangles = new List<Rectangle>();
            var buffer = (Bitmap)bigBitmap.Clone();
            var rectangle = Rectangle.Empty;

            do
            {
                rectangle = SearchBitmap(smallBitmap, buffer, tolerance);

                if (rectangle != Rectangle.Empty)
                {
                    rectangles.Add(rectangle);
                    var graphics = Graphics.FromImage(buffer);
                    graphics.FillRectangle(new SolidBrush(Color.Red), rectangle);
                }

            } while (rectangle != Rectangle.Empty);

            return rectangles;
        }

        public static Rectangle SearchBitmap(Bitmap bitmap1, Bitmap bitmap2, double tolerance)
        {

            if (bitmap1.Width > bitmap2.Width || bitmap1.Height > bitmap2.Height)
            {
                Bitmap aux = bitmap2;
                bitmap2 = bitmap1;
                bitmap1 = aux;
            }

            if (bitmap1.Height > bitmap2.Height)
            {
                return Rectangle.Empty;
            }

            return SearchBitmapStepTwo(bitmap1, bitmap2, tolerance);

        }

        private static Rectangle SearchBitmapStepTwo(Bitmap smallBmp, Bitmap bigBmp, double tolerance)
        {
            BitmapData smallData =
                smallBmp.LockBits(new Rectangle(0, 0, smallBmp.Width, smallBmp.Height),
                       System.Drawing.Imaging.ImageLockMode.ReadOnly,
                       System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            BitmapData bigData =
              bigBmp.LockBits(new Rectangle(0, 0, bigBmp.Width, bigBmp.Height),
                       System.Drawing.Imaging.ImageLockMode.ReadOnly,
                       System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int smallStride = smallData.Stride;
            int bigStride = bigData.Stride;

            int bigWidth = bigBmp.Width;
            int bigHeight = bigBmp.Height - smallBmp.Height + 1;
            int smallWidth = smallBmp.Width * 3;
            int smallHeight = smallBmp.Height;

            Rectangle location = Rectangle.Empty;
            int margin = Convert.ToInt32(255.0 * tolerance);

            unsafe
            {
                byte* pSmall = (byte*)(void*)smallData.Scan0;
                byte* pBig = (byte*)(void*)bigData.Scan0;

                int smallOffset = smallStride - smallBmp.Width * 3;
                int bigOffset = bigStride - bigBmp.Width * 3;

                bool matchFound = true;

                for (int y = 0; y < bigHeight; y++)
                {
                    for (int x = 0; x < bigWidth; x++)
                    {
                        byte* pBigBackup = pBig;
                        byte* pSmallBackup = pSmall;

                        //Look for the small picture.
                        for (int i = 0; i < smallHeight; i++)
                        {
                            int j = 0;
                            matchFound = true;
                            for (j = 0; j < smallWidth; j++)
                            {
                                //With tolerance: pSmall value should be between margins.
                                int inf = pBig[0] - margin;
                                int sup = pBig[0] + margin;

                                if (sup < pSmall[0] || inf > pSmall[0])
                                {
                                    matchFound = false;
                                    break;
                                }

                                pBig++;
                                pSmall++;

                            }

                            if (!matchFound) break;

                            //We restore the pointers.
                            pSmall = pSmallBackup;
                            pBig = pBigBackup;

                            //Next rows of the small and big pictures.
                            pSmall += smallStride * (1 + i);
                            pBig += bigStride * (1 + i);
                        }

                        //If match found, we return.
                        if (matchFound)
                        {
                            location.X = x;
                            location.Y = y;
                            location.Width = smallBmp.Width;
                            location.Height = smallBmp.Height;
                            break;
                        }
                        //If no match found, we restore the pointers and continue.
                        else
                        {
                            pBig = pBigBackup;
                            pSmall = pSmallBackup;
                            pBig += 3;
                        }
                    }

                    if (matchFound) break;

                    pBig += bigOffset;
                }
            }

            if (bigData != null)
                bigBmp.UnlockBits(bigData);
            if (smallData != null)
                smallBmp.UnlockBits(smallData);


            return location;
        }

    }
}
