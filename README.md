Medo
====

Collection of useful classes.

Each class is implemented in a single file intended for direct inclusion into
your project without any dependencies between classes.


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


## CryptPassword (Medo.Security.Cryptography)

Various password generation algorithms compatible with Unix crypt.


## OneTimePassword (Medo.Security.Cryptography)

Implementation of HOTP (RFC 4226) and TOTP (RFC 6238) one-time password
algorithms.


## Pbkdf2 (Medo.Security.Cryptography)

Generic PBKDF2 implementation.


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
