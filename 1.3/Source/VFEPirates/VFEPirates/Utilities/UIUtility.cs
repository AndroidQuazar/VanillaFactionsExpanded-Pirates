using UnityEngine;
using Verse;

namespace VFEPirates.Utilities
{
    public static class UIUtility
    {
        public static Rect TakeTopPart(ref this Rect rect, float pixels)
        {
            var ret = rect.TopPartPixels(pixels);
            rect.y += pixels;
            return ret;
        }

        public static Rect TakeRightPart(ref this Rect rect, float pixels)
        {
            var ret = rect.RightPartPixels(pixels);
            rect.width -= pixels;
            return ret;
        }
    }
}