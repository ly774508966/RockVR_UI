using System;
using System.Text;
using System.Collections;

namespace RockVR.Utils {
    public class StringUtils {

        public static string RandomString(int size = 10) {
            Random random = new Random((int)DateTime.Now.Ticks);
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++) {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 97)));                 
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}
