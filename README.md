Medo
====

Collection of useful classes.


## Config (Medo.Configuration)

Reading and writing settings from ini-like file.


## [IniFile](src/Medo/Configuration/IniFile.md) (Medo.Configuration)

Reading ini files.


## Base58 (Medo.Convert)

Base58 encoder/decoder with leading-zero preservation.


## LifetimeWatch (Medo.Diagnostics)

Timer that fires upon creation of object and stops with it's disposal.


## UnhandledCatch (Medo.Diagnostics)

Global handling of unhandled errors.


## BarcodePattern (Medo.Drawing)

Barcode drawing.


## SimplePngImage (Medo.Drawing)

A simple PNG image reader/writer.


## Terminal (Medo.IO)

Basic terminal operations supporting ANSI sequences.


## ExponentialMovingAverage (Medo.Math)

Calculates exponential moving average for added items.


## LinearCalibration (Medo.Math)

Linear calibration using a least square regression.


## LinearInterpolation (Medo.Math)

Returns adjusted value based on given reference points.


## MovingAverage (Medo.Math)

Calculates moving average for the added items.


## SimpleAverage (Medo.Math)

Calculates average for added items.


## WeightedMovingAverage (Medo.Math)

Calculates weighted moving average for the added items.


## WelfordVariance (Medo.Math)

Calculating variance and standard deviation from the streaming data using Welford's online algorithm.


## TrivialNtpClient (Medo.Net)

Simple NTP client.


## AssemblyInformation (Medo.Reflection)

Returns various info about the assembly that started process.


## Crc8 (Medo.Security.Checksum)

Computes hash using the standard 8-bit CRC algorithm. The following CRC-8
variants are supported: AUTOSAR, BLUETOOTH, CCITT, CDMA2000, DALLAS, DARC,
DVB-S2, GSM-A, GSM-B, HITAG, I-432-1, I-CODE, ITU, LTE, MAXIM, MAXIM-DOW,
MIFARE, MIFARE-MAD, NRSC-5, OpenSAFETY, ROHC, SAE-J1850, SMBUS, TECH-3250, and
WCDMA2000.


## Crc16 (Medo.Security.Checksum)

Computes hash using 16-bit CRC algorithm. The following CRC-16 variants are
supported: ACORN, ARC, AUG-CCITT, AUTOSAR, BUYPASS, CDMA2000, CCITT,
CCITT-FALSE, CCITT-TRUE, CMS, DARC, DDS-110, DECT-R, DECT-X, DNP, EN-13757, EPC,
EPC-C1G2, GENIBUS, GSM, I-CODE, IBM-3740, ISO-HDLD, IBM-SDLC, IEC-61158-2,
IEEE 802.3, ISO-IEC-14443-3-A, ISO-IEC-14443-3-B, KERMIT, LHA, LJ1200, LTE,
MAXIM, MAXIM-DOW, MCRF4XX, MODBUS, NRSC-5, OPENSAFETY-A, OPENSAFETY-B, PROFIBUS,
RIELLO, SPI-FUJITSU, T10-DIF, TELEDISK, TMS37157, UMTS, USB, V-41-LSB, V-41-MSB,
VERIFONE, X-25, XMODEM, and ZMODEM.


## Crc32 (Medo.Security.Checksum)

Computes hash using 32-bit CRC algorithm. The following CRC-16 variants are
supported: AAL5, ADCCP, AIXM, AUTOSAR, BASE91-C, BASE91-D, BZIP2, CASTAGNOLI,
CD-ROM-EDC, CKSUM, DECT-B, IEEE-802.3, INTERLAKEN, ISCSI, ISO-HDLC, JAMCRC,
MPEG-2, PKZIP, POSIX, V-42, XFER, and XZ.


## Damm (Medo.Security.Checksum)

Computes checksum using Damm's algorithm from numerical input. This algorithm
allows detection of all single-digit errors and all adjacent transposition errors.


## Fletcher16 (Medo.Security.Checksum)

Computes checksum using Fletcher-16 algorithm.


## Iso7064 (Medo.Security.Checksum)

Computes hash using ISO 7064 algorithm from numerical input.


## RivestCipher4Managed (Medo.Security.Cryptography)

Rivest Cipher 4 (RC4) algorithm implementation.


## CryptPassword (Medo.Security.Cryptography)

Various password generation algorithms compatible with Unix crypt.


## OneTimePassword (Medo.Security.Cryptography)

Implementation of HOTP (RFC 4226) and TOTP (RFC 6238) one-time password
algorithms.


## Pbkdf2 (Medo.Security.Cryptography)

Generic PBKDF2 implementation.


## TwofishManaged (Medo.Security.Cryptography)

Twofish algorithm implementation.


## CsvTextOutput (Medo.Text)

Writing comma separated values.


## ParameterExpansion (Medo.Text)

Performs basic shell parameter expansion.


## Placeholder (Medo.Text)

Composite formatting based on placeholder name.


## PerSecondCounter (Medo.Timers)

Measures throughput per second.


## PerSecondLimiter (Medo.Timers)

Limits per-second throughput.
