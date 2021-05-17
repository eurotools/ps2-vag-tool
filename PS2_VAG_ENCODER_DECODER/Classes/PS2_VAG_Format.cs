﻿using System;
using System.IO;

namespace PS2_VAG_ENCODER_DECODER
{
    internal static class PS2_VAG_Format
    {
        //*===============================================================================================
        //* Definitions and Classes
        //*===============================================================================================
        private static int[,] vag_lut = new int[5, 2]
        {
            {   0,   0 },
            {  60,   0 },
            { 115, -52 },
            {  98, -55 },
            { 122, -60 }
        };

        private struct vag_chunk
        {
            byte shift_factor;
            byte predict_nr; /* swy: reversed nibbles due to little-endian */
            byte flag;
            byte[] s;
        };

        private enum vag_flag
        {
            VAGF_NOTHING = 0, /* Nothing*/
            VAGF_END_MARKER_AND_DEC = 1, /* End marker + decode*/
            VAGF_LOOP_REGION = 2, /* Loop region*/
            VAGF_LOOP_END = 3, /* Loop end*/
            VAGF_START_MARKER = 4, /* Start marker*/
            VAGF_UNK = 5, /* ?*/
            VAGF_LOOP_START = 6, /* Loop start*/
            VAGF_END_MARKER_AND_SKIP = 7  /* End marker + don't decode */
        };

        //Defines
        private static int VAG_MAX_LUT_INDX = vag_lut.Length - 1;
        private static int VAG_SAMPLE_BYTES = 14;
        private static int VAG_SAMPLE_NIBBL = VAG_SAMPLE_BYTES * 2;

        //*===============================================================================================
        //* Encoding / Decoding Functions
        //*===============================================================================================
        public static byte[] DecodeVAG_ADPCM(byte[] VagFileData, int NumSamples)
        {
            byte[] outp;

            using (MemoryStream DecodedData = new MemoryStream())
            {
                using (BinaryWriter BWriter = new BinaryWriter(DecodedData))
                {
                    int hist1 = 0;
                    int hist2 = 0;

                    /* swy: loop for each 16-byte chunk */
                    for (int i = 0; i < VagFileData.Length; i++)
                    {
                        int[] unpacked_nibbles = new int[VAG_SAMPLE_NIBBL];

                        /* swy: unpack one of the 28 'scale' 4-bit nibbles in the 28 bytes; two 'scales' in one byte */
                        for (int j = 0, nib = 0; j < VAG_SAMPLE_BYTES; j++)
                        {
                            short sample_byte = VagFileData[j];

                            unpacked_nibbles[nib++] = (sample_byte & 0x0F) >> 0;
                            unpacked_nibbles[nib++] = (sample_byte & 0xF0) >> 4;
                        }

                        /* swy: decode each of the 14*2 ADPCM samples in this chunk */
                        for (int j = 0; j < VAG_SAMPLE_NIBBL; j++)
                        {
                            /* swy: same as multiplying it by 4096; turn the signed nibble into a signed int first, though */
                            int scale = unpacked_nibbles[j] << 12;

                            /* swy: don't overflow the LUT array access; limit the max allowed index */
                            byte predict_nr = (byte)Math.Min(i, VAG_MAX_LUT_INDX);

                        }
                    }
                }
                outp = DecodedData.ToArray();
            }
            return outp;
        }
    }
}