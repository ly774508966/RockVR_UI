using UnityEngine;
using System.Collections;

namespace RockVR.Utils {
    
    public class ImageUtils {

        public static Color[] FlipPixelsX(Color[] pixels, int width, int height) {
            for (int x1 = 0, x2 = width - 1; x1 < x2; x1++, x2--) {
                for (int y = 0; y < height; y++) {
                    int shift = y * width;
                    Color temp = pixels [shift + x2];
                    pixels [shift + x2] = pixels [shift + x1];
                    pixels [shift + x1] = temp;
                }
            }
            return pixels;
        }

        public static Color[] FlipPixelsY(Color[] pixels, int width, int height) {
            for (int x = 0; x < width; x++) {
                for (int y1 = 0, y2 = height - 1; y1 < y2; y1++, y2--) {
                    int shift1 = y1 * width, shift2 = y2 * width;
                    Color temp = pixels [shift2 + x];
                    pixels [shift2 + x] = pixels[shift1 + x];
                    pixels [shift1 + x] = temp;
                }
            }
            return pixels;
        }
    }
}