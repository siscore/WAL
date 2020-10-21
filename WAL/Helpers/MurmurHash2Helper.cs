using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAL.Helpers
{
    public static class MurmurHash2Helper
    {
        public static long ComputeFileHash(string path, bool normalizeWhitespace = false)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return (long)ComputeHash((Stream)fileStream, 0L, normalizeWhitespace);
            }
        }

        public static long ComputeNormalizedFileHash(string path)
        {
            return ComputeFileHash(path, true);
        }

        public static uint ComputeHash(byte[] input, bool normalizeWhitespace = false)
        {
            return ComputeHash((Stream)new MemoryStream(input), 0L, normalizeWhitespace);
        }

        public static uint ComputeHash(Stream input, long precomputedLength = 0, bool normalizeWhitespace = false)
        {
            long num1 = precomputedLength != 0L ? precomputedLength : input.Length;
            byte[] buffer = new byte[65536];
            if (precomputedLength == 0L & normalizeWhitespace)
            {
                long position = input.Position;
                num1 = ComputeNormalizedLength(input, buffer);
                input.Seek(position, SeekOrigin.Begin);
            }
            uint num2 = (uint)(1UL ^ (ulong)num1);
            uint num3 = 0;
            int num4 = 0;
            label_3:
            int num5 = input.Read(buffer, 0, buffer.Length);
            if (num5 != 0)
            {
                for (int index = 0; index < num5; ++index)
                {
                    byte b = buffer[index];
                    if (!normalizeWhitespace || !IsWhitespaceCharacter(b))
                    {
                        num3 |= (uint)b << num4;
                        num4 += 8;
                        if (num4 == 32)
                        {
                            uint num6 = num3 * 1540483477U;
                            uint num7 = (num6 ^ num6 >> 24) * 1540483477U;
                            num2 = num2 * 1540483477U ^ num7;
                            num3 = 0U;
                            num4 = 0;
                        }
                    }
                }
                goto label_3;
            }
            else
            {
                if (num4 > 0)
                    num2 = (num2 ^ num3) * 1540483477U;
                uint num6 = (num2 ^ num2 >> 13) * 1540483477U;
                return num6 ^ num6 >> 15;
            }
        }

        private static bool IsWhitespaceCharacter(byte b)
        {
            if ((int)b != 9 && (int)b != 10 && (int)b != 13)
                return (int)b == 32;
            return true;
        }

        public static long ComputeNormalizedLength(Stream input, byte[] buffer)
        {
            long num1 = 0;
            if (buffer == null)
                buffer = new byte[65536];
            label_2:
            int num2 = input.Read(buffer, 0, buffer.Length);
            if (num2 == 0)
                return num1;
            for (int index = 0; index < num2; ++index)
            {
                if (!IsWhitespaceCharacter(buffer[index]))
                    ++num1;
            }
            goto label_2;
        }
    }
}
