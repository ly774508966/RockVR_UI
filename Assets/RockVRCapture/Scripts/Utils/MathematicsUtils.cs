using System.Collections;

namespace RockVR.Utils {

    public class MathematicsUtils {

        public static bool IsPowerOfTwo(int number) {
            return (number != 0) && ((number & (number - 1)) == 0);
        }

        public static float StringToFloat(string str) {
            float result;
            if (float.TryParse (str, out result)) {
                return result;
            }
            return (float)0.00;
        }
    }
}