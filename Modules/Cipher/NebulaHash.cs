using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthAPI.Modules.Cipher
{
    public static class NebulaHash
    {
        public static int ComputeConstantHash(this string str)
        {
            const long MulPrime = 467;
            const long SurPrime = 2147283659;

            long val = 0;
            foreach (char c in str)
            {
                val *= MulPrime;
                val += c;
                val %= SurPrime;
            }
            return (int)(val % SurPrime);
        }
    }
}
