//
// Copyright (c) 2010-2023 Antmicro
//
// This file is licensed under the MIT License.
// Full license text is available in 'licenses/MIT.txt'.
//
using System.Collections.Generic;
using Antmicro.Renode.Exceptions;
using Antmicro.Renode.Peripherals.Memory;
using Antmicro.Renode.Utilities;

namespace Antmicro.Renode.Peripherals.SPI
{
    public class Cypress_S25H : GenericSpiFlash
    {
        public Cypress_S25H(MappedMemory underlyingMemory)
            : base(underlyingMemory, manufacturerId: ManufacturerId, memoryType: MemoryType,
                   writeStatusCanSetWriteEnable: true, extendedDeviceId: ExtendedDeviceID,
                   remainingIdBytes: RemainingIDBytes, deviceConfiguration: DeviceConfiguration)
        {
            if(underlyingMemory.Size < 32.MB() || underlyingMemory.Size > 128.MB())
            {
                throw new ConstructionException("Size of the underlying memory must be in range 32MB - 128MB");
            }
        }

        protected override byte GetCapacityCode()
        {
            return (byte)BitHelper.GetMostSignificantSetBitIndex((ulong)this.UnderlyingMemory.Size);
        }

        protected override byte[] GetSFDPSignature()
        {
            byte[] patchedSFDPSignature = DefaultSFDPSignature;
            byte capacity = GetCapacityCode();

            Dictionary<byte, Dictionary<ulong, byte>> patchtable = new Dictionary<byte, Dictionary<ulong, byte>>() {
                {0x19, new Dictionary<ulong, byte>() {
                    {0x107, 0x0F},
                    {0x12B, 0xE1},
                    {0x1EF, 0x01},
                    {0x1F7, 0x01},
                    {0x20F, 0x01},
                    {0x21F, 0x01}
                }},
                {0x1a, new Dictionary<ulong, byte>() {
                    {0x107, 0x1F},
                    {0x12B, 0xE3},
                    {0x1EF, 0x03},
                    {0x1F7, 0x03},
                    {0x20F, 0x03},
                    {0x21F, 0x03}
                }},
                {0x1b, new Dictionary<ulong, byte>() {
                    {0x107, 0x3F},
                    {0x12B, 0xE6},
                    {0x1EF, 0x07},
                    {0x1F7, 0x07},
                    {0x20F, 0x07},
                    {0x21F, 0x07}
                }}
            };

            foreach (KeyValuePair<ulong, byte> patch in patchtable[capacity])
            {
                patchedSFDPSignature[patch.Key] = patch.Value;
            }

            return patchedSFDPSignature;
        }

        private const byte ManufacturerId = 0x34;
        private const byte MemoryType = 0x2B; // HS-T: 0x2B, HL-T: 0x2A
        private const byte RemainingIDBytes = 0x0F;
        private const byte ExtendedDeviceID = 0x03;
        private const byte DeviceConfiguration = 0x90;

        private readonly byte[] DefaultSFDPSignature = new byte[]
        {
            // SFDP Rev D header table
            0x53, 0x46, 0x44, 0x50, 0x08, 0x01, 0x03, 0xff, 0x00, 0x00,
            0x01, 0x14, 0x00, 0x01, 0x00, 0xff, 0x84, 0x00, 0x01, 0x02,
            0x50, 0x01, 0x00, 0xff, 0x81, 0x00, 0x01, 0x16, 0xc8, 0x01,
            0x00, 0xff, 0x84, 0x00, 0x01, 0x1c, 0x58, 0x01, 0x00, 0xff,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            // Values below are not consistent with the spec
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            // SFDP Rev D parameter table
            0xe7, 0x20, 0xfa, 0xff, 0xff, 0xff, 0xff, 0x1F, 0x48, 0xeb, //0x107: 0x0f/1f/3f
            0x08, 0x6b, 0x00, 0xff, 0x88, 0xbb, 0xfe, 0xff, 0xff, 0xff,
            0xff, 0xff, 0x00, 0xff, 0xff, 0xff, 0x48, 0xeb, 0x0c, 0x20,
            0x00, 0xff, 0x00, 0xff, 0x12, 0xd8, 0x23, 0xfa, 0xff, 0x8b,
            0x82, 0xe7, 0xff, 0xE3, 0xec, 0x23, 0x19, 0x49, 0x8a, 0x85, //0x12B: 0xe1/e3/e6
            0x7a, 0x75, 0xf7, 0x66, 0x80, 0x5c, 0x8c, 0xd6, 0xdd, 0xff,
            0xf9, 0x38, 0xf8, 0xa1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xbc, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf7, 0xf5, 0xff, 0xff,
            0x7b, 0x92, 0x0f, 0xfe, 0x21, 0xff, 0xff, 0xdc, 0x00, 0x00,
            0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0xc0, 0xff, 0xc3, 0xeb,
            0xc8, 0xff, 0xe3, 0xeb, 0x00, 0x65, 0x00, 0x90, 0x06, 0x05,
            0x00, 0xa1, 0x00, 0x65, 0x00, 0x96, 0x00, 0x65, 0x00, 0x65,
            0x71, 0x65, 0x03, 0xd0, 0x71, 0x65, 0x03, 0xd0, 0x00, 0x00,
            0x00, 0x00, 0xb0, 0x2e, 0x00, 0x00, 0x88, 0xa4, 0x89, 0xaa,
            0x71, 0x65, 0x03, 0x96, 0x71, 0x65, 0x03, 0x96, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x71, 0x65,
            0x05, 0xd5, 0x71, 0x65, 0x05, 0xd5, 0x00, 0x00, 0xa0, 0x15,
            // SFDP rev D sector map parameter table
            0xFC, 0x65, 0xFF, 0x08, 0x04, 0x00, 0x80, 0x00, 0xFC, 0x65,
            0xFF, 0x40, 0x02, 0x00, 0x80, 0x00, 0xFD, 0x65, 0xFF, 0x04,
            0x02, 0x00, 0x80, 0x00, 0xFE, 0x00, 0x02, 0xFF, 0xF1, 0xFF,
            0x01, 0x00, 0xF8, 0xFF, 0x01, 0x00, 0xF8, 0xFF, 0xFB, 0x03, //0x1EF: 0x01/03/07
            0xFE, 0x01, 0x02, 0xFF, 0xF8, 0xFF, 0xFB, 0x03, 0xF8, 0xFF, //0x1F7: 0x01/03/07
            0x01, 0x00, 0xF1, 0xFF, 0x01, 0x00, 0xFE, 0x02, 0x04, 0xFF,
            0xF1, 0xFF, 0x00, 0x00, 0xF8, 0xFF, 0x02, 0x00, 0xF8, 0xFF,
            0xF7, 0x03, 0xF8, 0xFF, 0x02, 0x00, 0xF1, 0xFF, 0x00, 0x00, //0x20F: 0x01/03/07
            0xFF, 0x04, 0x00, 0xFF, 0xF8, 0xFF, 0xFF, 0x03              //0x21F: 0x01/03/07
        };
    }
}
