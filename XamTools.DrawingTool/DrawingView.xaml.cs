﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamTools.DrawingTool.SkiaSharpExtention;
using XamTools.DrawingTool.SkiaSharpExtention.TouchTracking;

namespace XamTools.DrawingTool
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrawingView : ContentView
    {
        private bool pendingCircle;
        private bool pendingArrow;
        const string arrowResourceID = "XamTools.DrawingTool.Images.Arrow.png";
        const string circleResourceID = "XamTools.DrawingTool.Images.Arrow.png";

        List<TouchManipulationBitmap> bitmapCollection =
            new List<TouchManipulationBitmap>();

        Dictionary<long, TouchManipulationBitmap> bitmapDictionary =
            new Dictionary<long, TouchManipulationBitmap>();

        public DrawingView()
        {
            InitializeComponent();          
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKCanvas canvas = args.Surface.Canvas;
            canvas.Clear();

            foreach (TouchManipulationBitmap bitmap in bitmapCollection)
            {
                bitmap.Paint(canvas);
            }
        }

        void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            // Convert Xamarin.Forms point to pixels
            Point pt = args.Location;
            SKPoint point =
                new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                            (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    for (int i = bitmapCollection.Count - 1; i >= 0; i--)
                    {
                        TouchManipulationBitmap bitmap = bitmapCollection[i];

                        if (bitmap.HitTest(point))
                        {
                            // Move bitmap to end of collection
                            bitmapCollection.Remove(bitmap);
                            bitmapCollection.Add(bitmap);

                            // Do the touch processing
                            bitmapDictionary.Add(args.Id, bitmap);
                            bitmap.ProcessTouchEvent(args.Id, args.Type, point);
                            canvasView.InvalidateSurface();
                            break;
                        }
                    }
                    break;

                case TouchActionType.Moved:
                    if (bitmapDictionary.ContainsKey(args.Id))
                    {
                        TouchManipulationBitmap bitmap = bitmapDictionary[args.Id];
                        bitmap.ProcessTouchEvent(args.Id, args.Type, point);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    if (bitmapDictionary.ContainsKey(args.Id))
                    {
                        TouchManipulationBitmap bitmap = bitmapDictionary[args.Id];
                        bitmap.ProcessTouchEvent(args.Id, args.Type, point);
                        bitmapDictionary.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;
            }
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            pendingCircle = true;
            pendingArrow = false;

            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(circleResourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                SKBitmap bitmap = SKBitmap.Decode(skStream);
                bitmapCollection.Add(new TouchManipulationBitmap(bitmap)
                {
                    Matrix = SKMatrix.MakeTranslation(50, 50),
                });
               
            }            
            canvasView.InvalidateSurface();
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            pendingCircle = false;
            pendingArrow = true;
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(arrowResourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                SKBitmap bitmap = SKBitmap.Decode(skStream);
                bitmapCollection.Add(new TouchManipulationBitmap(bitmap)
                {
                    Matrix = SKMatrix.MakeTranslation(50, 50),
                });

            }
            canvasView.InvalidateSurface();
        }
    }
}