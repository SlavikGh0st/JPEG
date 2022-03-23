using System.Collections.Generic;

namespace JPEG.NT.Packers
{
    public class ZigZag8x8Packer : IPacker<byte>
    {
        public IEnumerable<byte> Pack(byte[,] input)
        {
            return new[]
            {
                input[0, 0], input[0, 1], input[1, 0], input[2, 0], input[1, 1], input[0, 2], input[0, 3], input[1, 2],
                input[2, 1], input[3, 0], input[4, 0], input[3, 1], input[2, 2], input[1, 3],  input[0, 4], input[0, 5],
                input[1, 4], input[2, 3], input[3, 2], input[4, 1], input[5, 0], input[6, 0], input[5, 1], input[4, 2],
                input[3, 3], input[2, 4], input[1, 5],  input[0, 6], input[0, 7], input[1, 6], input[2, 5], input[3, 4],
                input[4, 3], input[5, 2], input[6, 1], input[7, 0], input[7, 1], input[6, 2], input[5, 3], input[4, 4],
                input[3, 5], input[2, 6], input[1, 7], input[2, 7], input[3, 6], input[4, 5], input[5, 4], input[6, 3],
                input[7, 2], input[7, 3], input[6, 4], input[5, 5], input[4, 6], input[3, 7], input[4, 7], input[5, 6],
                input[6, 5], input[7, 4], input[7, 5], input[6, 6], input[5, 7], input[6, 7], input[7, 6], input[7, 7]
            };
        }

        public byte[,] Unpack(IReadOnlyList<byte> input)
        {
            return new[,]
            {
                { input[0], input[1], input[5], input[6], input[14], input[15], input[27], input[28] },
                { input[2], input[4], input[7], input[13], input[16], input[26], input[29], input[42] },
                { input[3], input[8], input[12], input[17], input[25], input[30], input[41], input[43] },
                { input[9], input[11], input[18], input[24], input[31], input[40], input[44], input[53] },
                { input[10], input[19], input[23], input[32], input[39], input[45], input[52], input[54] },
                { input[20], input[22], input[33], input[38], input[46], input[51], input[55], input[60] },
                { input[21], input[34], input[37], input[47], input[50], input[56], input[59], input[61] },
                { input[35], input[36], input[48], input[49], input[57], input[58], input[62], input[63] }
            };
        }
    }
}