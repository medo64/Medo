using System;
using System.Security.Cryptography;
using Xunit;

namespace Medo.Tests.Convert.Base58 {
    using Medo.Convert;

    public class Tests {

        [Fact(DisplayName = "Base58: Empty ToString")]
        public void Empty_ToString() {
            var input = Array.Empty<byte>();
            var output = Base58.ToString(input);
            Assert.Equal("", output);
        }

        [Fact(DisplayName = "Base58: Empty AsBytes")]
        public void Empty_AsBytes() {
            var input = "";
            var output = Base58.AsBytes(input);
            Assert.Equal("", BitConverter.ToString(output));
        }


        [Fact(DisplayName = "Base58: Basic ToString")]
        public void Basic_ToString() {
            var input = new byte[] { 1, 2, 3 };
            var output = Base58.ToString(input);
            Assert.Equal("Ldp", output);
        }

        [Fact(DisplayName = "Base58: Basic AsBytes")]
        public void Basic_AsBytes() {
            var input = "Ldp";
            var output = Base58.AsBytes(input);
            Assert.Equal("01-02-03", BitConverter.ToString(output));
        }


        [Fact(DisplayName = "Base58: LeadingZero ToString")]
        public void LeadingZero_ToString() {
            var input = new byte[] { 0, 0, 1 };
            var output = Base58.ToString(input);
            Assert.Equal("112", output);
        }

        [Fact(DisplayName = "Base58: LeadingZero AsBytes")]
        public void LeadingZero_AsBytes() {
            var input = "112";
            var output = Base58.AsBytes(input);
            Assert.Equal("00-00-01", BitConverter.ToString(output));
        }


        [Fact(DisplayName = "Base58: MiddleZero ToString")]
        public void MiddleZero_ToString() {
            var input = new byte[] { 0x00, 0xFF, 0x00 };
            var output = Base58.ToString(input);
            Assert.Equal("1LQX", output);
        }

        [Fact(DisplayName = "Base58: MiddleZero AsBytes")]
        public void MiddleZero_AsBytes() {
            var input = "1LQX";
            var output = Base58.AsBytes(input);
            Assert.Equal("00-FF-00", BitConverter.ToString(output));
        }


        [Fact(DisplayName = "Base58: All 0x00 ToString")]
        public void All00_ToString() {
            var input = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                   };
            var output = Base58.ToString(input);
            Assert.Equal("1111111111111111111111111111111111111111111111111111111111111111", output);
        }

        [Fact(DisplayName = "Base58: All 0x00 AsBytes")]
        public void All00_AsBytes() {
            var input = "1111111111111111111111111111111111111111111111111111111111111111";
            var output = Base58.AsBytes(input);
            Assert.Equal("00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00", BitConverter.ToString(output));
        }


        [Fact(DisplayName = "Base58: All 0xFF ToString")]
        public void AllFF_ToString() {
            var input = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                                     0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                                     0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                                     0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                                   };
            var output = Base58.ToString(input);
            Assert.Equal("67rpwLCuS5DGA8KGZXKsVQ7dnPb9goRLoKfgGbLfQg9WoLUgNY77E2jT11fem3coV9nAkguBACzrU1iyZM4B8roQ", output);
        }

        [Fact(DisplayName = "Base58: All 0xFF AsBytes")]
        public void AllFF_AsBytes() {
            var input = "67rpwLCuS5DGA8KGZXKsVQ7dnPb9goRLoKfgGbLfQg9WoLUgNY77E2jT11fem3coV9nAkguBACzrU1iyZM4B8roQ";
            var output = Base58.AsBytes(input);
            Assert.Equal("FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF", BitConverter.ToString(output));
        }


        [Fact(DisplayName = "Base58: Long ToString")]
        public void Long_ToString() {
            var input = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                                     0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
                                     0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F,
                                     0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
                                     0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F,
                                     0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F,
                                     0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0X6C, 0x6D, 0x6E, 0x6F,
                                     0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F,
                                     0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F,
                                     0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0x9B, 0x9C, 0x9D, 0x9E, 0x9F,
                                     0xA0, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xAB, 0xAC, 0xaD, 0xAE, 0xAF,
                                     0xB0, 0xB1, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xBB, 0xBC, 0xBD, 0xBE, 0xBF,
                                     0xC0, 0xC1, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xCB, 0xCC, 0xCD, 0xCE, 0xCF,
                                     0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF,
                                     0xE0, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xEB, 0xEC, 0xED, 0xEE, 0xEF,
                                     0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xFF,
                                   };
            var output = Base58.ToString(input);
            Assert.Equal("1cWB5HCBdLjAuqGGReWE3R3CguuwSjw6RHn39s2yuDRTS5NsBgNiFpWgAnEx6VQi8csexkgYw3mdYrMHr8x9i7aEwP8kZ7vccXWqKDvGv3u1GxFKPuAkn8JCPPGDMf3vMMnbzm6Nh9zh1gcNsMvH3ZNLmP5fSG6DGbbi2tuwMWPthr4boWwCxf7ewSgNQeacyozhKDDQQ1qL5fQFUW52QKUZDZ5fw3KXNQJMcNTcaB723LchjeKun7MuGW5qyCBZYzA1KjofN1gYBV3NqyhQJ3Ns746GNuf9N2pQPmHz4xpnSrrfCvy6TVVz5d4PdrjeshsWQwpZsZGzvbdAdN8MKV5QsBDY", output);
        }

        [Fact(DisplayName = "Base58: Long AsBytes")]
        public void Long_AsBytes() {
            var input = "1cWB5HCBdLjAuqGGReWE3R3CguuwSjw6RHn39s2yuDRTS5NsBgNiFpWgAnEx6VQi8csexkgYw3mdYrMHr8x9i7aEwP8kZ7vccXWqKDvGv3u1GxFKPuAkn8JCPPGDMf3vMMnbzm6Nh9zh1gcNsMvH3ZNLmP5fSG6DGbbi2tuwMWPthr4boWwCxf7ewSgNQeacyozhKDDQQ1qL5fQFUW52QKUZDZ5fw3KXNQJMcNTcaB723LchjeKun7MuGW5qyCBZYzA1KjofN1gYBV3NqyhQJ3Ns746GNuf9N2pQPmHz4xpnSrrfCvy6TVVz5d4PdrjeshsWQwpZsZGzvbdAdN8MKV5QsBDY";
            var output = Base58.AsBytes(input);
            Assert.Equal("00-01-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F-80-81-82-83-84-85-86-87-88-89-8A-8B-8C-8D-8E-8F-90-91-92-93-94-95-96-97-98-99-9A-9B-9C-9D-9E-9F-A0-A1-A2-A3-A4-A5-A6-A7-A8-A9-AA-AB-AC-AD-AE-AF-B0-B1-B2-B3-B4-B5-B6-B7-B8-B9-BA-BB-BC-BD-BE-BF-C0-C1-C2-C3-C4-C5-C6-C7-C8-C9-CA-CB-CC-CD-CE-CF-D0-D1-D2-D3-D4-D5-D6-D7-D8-D9-DA-DB-DC-DD-DE-DF-E0-E1-E2-E3-E4-E5-E6-E7-E8-E9-EA-EB-EC-ED-EE-EF-F0-F1-F2-F3-F4-F5-F6-F7-F8-F9-FA-FB-FC-FD-FE-FF", BitConverter.ToString(output));
        }


        [Fact(DisplayName = "Base58: Basic Text")]
        public void Text_Basic() {
            var text = "Testing";

            var base58 = Base58.ToString(text);
            var output = Base58.AsString(base58);

            Assert.Equal(text, output);
        }


        private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();

        [Fact(DisplayName = "Base58: Randomized")]
        public void Randomized() {
            var random = new byte[64];
            Random.GetBytes(random);

            var base58 = Base58.ToString(random);
            var bytes = Base58.AsBytes(base58);

            Assert.Equal(BitConverter.ToString(random), BitConverter.ToString(bytes));
        }


        [Fact(DisplayName = "Base58: Exception - Invalid Character")]
        public void Exception_InvalidCharacter() {
            Assert.Throws<FormatException>(() => {
                var bytes = Base58.AsBytes("!");
            });
        }


    }
}
