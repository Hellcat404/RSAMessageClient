using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageClient {
    internal static class StreamUtils {
        public static byte[] AddTerminator(byte[] data) {
            byte[] output = new byte[data.Length + 4];
            for (int i = 0; i < data.Length; i++) {
                output[i] = data[i];
            }
            return output;
        }

        public static byte[] RemoveTerminator(byte[] data, int length) {
            byte[] output = new byte[length - 4];
            for (int i = 0; i < output.Length; i++) {
                output[i] = data[i];
            }
            return output;
        }

        public static byte[] AddOpcode(byte[] data, int opcode) {
            byte[] output = new byte[data.Length + 1];
            output[0] = (byte)opcode;
            for (int i = 1; i < output.Length; i++) {
                output[i] = data[i - 1];
            }
            return output;
        }
    }
}
