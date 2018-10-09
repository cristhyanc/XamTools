using System;

using SkiaSharp;

namespace XamTools.DrawingTool.SkiaSharpExtention
{
    class TouchManipulationInfo
    {
        public SKPoint PreviousPoint { set; get; }

        public SKPoint NewPoint { set; get; }
    }
}
