# BearBus Protocol Specification

BearBus is a message protocol intended for use on RS-485 and similar
interfaces. It allows for communication between a single host and up to 255
devices using up to 31 command codes.

Messages can contain up to 255 bytes of extra data either CRC-8 (up to 12 bytes
of data) or CRC-16 verified (13-127 bytes). The total message length is thus
between 5 bytes (up to a single data byte) and 134 bytes (5 byte header, 127
bytes of data, and 2 bytes CRC-16).

All commands either originate from host or have host as their destination.
There is no inter-device communication.

There are three distinct versions of packet:
* Short
    * only 5-byte header with up to a single byte of embedded data
    * CRC-8 header checksum
* Basic
    * up to 12 bytes of data
    * CRC-8 header and data checksum
* Extended
    * up to 127 bytes of data
    * CRC-8 header checksum
    * CRC-8 (for up to 12 bytes of data) or CRC-16 (for more than 12 bytes of data) data checksum

Packet is defined as follows:

|       |                                        Bit                        |                            |
| Octet |        7       |       6       |       5      | 4 | 3 | 2 | 1 | 0 |                            |
|-------|----------------|---------------|--------------|---|---|---|---|---|----------------------------|
|   0:1 | Header[7:0] (`0xBB`)                                              |                            |
|   1:1 | Address[7:0]                                                      |                            |
|   2:1 | FromHost[7]    | ReplyError[6] | EmbedData[5] | CommandCode[4:0]  |                            |
|   3:1 | UseCRC16[7]    | DataLength[6:0]                                  | {EmbedData=0}              |
|   3:1 | Datum[7:0]                                                        | {EmbedData=1}              |
|   4:1 | HeaderCRC8[7:0]                                                   |                            |
|   5:N | Data                                                              | {EmbedData=0}              |
| 5+N:1 | DataCRC8[7:0]                                                     | {EmbedData=0} {UseCRC16=0} |
| 5+N:2 | DataCRC16[15:0]                                                   | {EmbedData=0} {UseCRC16=1} |


## Fields

### Header

`Header` is 8-bit field that always contains value 0xBB.

### Address

`Address` is a 8-bit field specifying device that is either sender or receiver
of the message (depending on the value of `FromHost` field).

If host is sending a packet (`FromHost`=`1`) to device address `0`, a message
is to be considered as a broadcast and should be processed by all devices.

If device is sending a packet (`FromHost`=`0`), address must be between `1` and
`255` (i.e. devices without address must not send any replies). Exception to
this is unsolicited update if device address is not set.

### FromHost

`FromHost` is 1-bit field denoting if the origin of the message is host.

If value is `0`, a message originates from a device and is intended for the
host. If value is `1`, a message originates from the host and is intended for
a device.

### ReplyError

`ReplyError` is a 1-bit field serving two different purposes.

When sent from the host, it denotes if reply to this message is requested
(value `1`) or not (value `0`).

When received from a device, it denotes if message was a success (value `0`)
or an error (value `1`)

### EmbedData

This is a 1-bit field denoting if data is embedded within header (value `1`)
or is that field used as length of data (value `0`) and the actual data gets
added after the header.

### CommandCode

`CommandCode` is a 5-bit value specifying the command code of the message.
Custom codes are between `1` and `31`. By convention value `31` is used for
setup messages.

Value `0` denotes `System` commands that must be supported and that go against
established request/reply convention. Their `ReplyRequested` bit will be
ignored. All these commands should be supported (if possible).

### UseCRC16

This flag indicates if CRC-8 is used for data (value `0`) or will data be
protected by CRC-16 (value `1`).

Strong recommendation is to have any data longer than 12 bytes protected by
CRC-16 but longer packet with only CRC-8 protection is valid too.

Field exists only if `EmbedData`=`0`. Otherwise `Datum` occupies the same
space.

### DataLength

This is the length of extra data after the header. Valid values are `0`-`127`.

Field exists only if `EmbedData`=`0`. Otherwise `Datum` occupies the same
space.

### Datum

This is a single byte of data built-in the header.

Field exists only if `EmbedData`=`1`. Otherwise `UseCRC16` and `DataLength`
occupy the same space.

### HeaderCRC8

This is a CRC-8 (polynomial `0x2F`) value calculated over all preceding header
bytes. No CRC input XOR, output XOR or reflection is to be applied.

### Data

These are all extra bytes. Exists only if `EmbedData`=`0`.

### DataCRC8

This is a CRC-8 (polynomial `0x2F`) value calculated over HeaderCRC8 and all
preceding data bytes. No CRC input XOR, output XOR, or reflection is to be
applied.

Field exists only if `EmbedData`=`0` and `UseCRC16` is `0`.

### DataCRC16

This is a CRC-16 (polynomial `0xA2EB`) value calculated over HeaderCRC8 and
all preceding data bytes. No CRC input XOR, output XOR or reflection is to be
applied.

Field is 2 octets in length.

Field exists only if `EmbedData`=`0` and `UseCRC16` is `1`.


## Example Messages

### Short Packet

This is an example of a short packet with a single data byte.

|             | Short Packet Example                   |
|-------------|----------------------------------------|
| Packet      | `BB 05 BD 42 CF`                       |
| Header      | `0xBB`                                 |
| Address     | `0x05` (5)                             |
| FromHost    | `1` (from host)                        |
| Reply/Error | `0` (don't reply)                      |
| EmbedData   | `1` (using embedded data)              |
| Command     | `0x1D` (29)                            |
| Datum       | `0x42`                                 |
| HeaderCRC8  | `0xCF`                                 |

Total length of this packet is always 5 bytes and it contains 1 data byte.

### Basic Packet

This is an example of a packet with 3 bytes of data.

|             | Basic Packet Example                   |
|-------------|----------------------------------------|
| Packet      | `BB 13 9A 03 49 42 43 44 4E`           |
| Header      | `0xBB`                                 |
| Address     | `0x13` (19)                            |
| FromHost    | `1` (from host)                        |
| Reply/Error | `0` (don't reply)                      |
| EmbedData   | `0` (using extra data)                 |
| Command     | `0x1A` (26)                            |
| Use CRC16   | `0` (CRC-8)                            |
| DataLength  | `0x03` (3)                             |
| HeaderCRC8  | `0x49`                                 |
| Data        | `0x424344`                             |
| DataCRC8    | `0x4E`                                 |

Total length of this packet is between 8 bytes (2 data bytes) and 18 bytes (12
data bytes).

### Long Packet

This is an example of a long packet with 13 bytes of data.

|             | Long Packet Example                                           |
|-------------|---------------------------------------------------------------|
| Packet      | `BB 01 81 8D 57 42 43 44 45 46 47 48 49 4A 4B 4C 4D 4E 20 A9` |
| Header      | `0xBB`                                                        |
| Address     | `0x01` (1)                                                    |
| FromHost    | `1` (from host)                                               |
| Reply/Error | `0` (don't reply)                                             |
| EmbedData   | `0` (using extra data)                                        |
| Command     | `0x01` (1)                                                    |
| Use CRC16   | `1` (CRC-16)                                                  |
| DataLength  | `0x0D` (13)                                                   |
| HeaderCRC8  | `0x57`                                                        |
| Data        | `0x42434445464748494A4B4C4D4E`                                |
| DataCRC16   | `0x20A9` (since more than 12 data bytes are used)             |

Total length of this packet is between 20 bytes (13 data bytes) and 134 bytes
(127 data bytes). Most microcontroller devices are not expected to support
this packet type.


## Procedures

### System

#### Unsolicited Update

When device is powered on, after a button press, or when overall status is
changed (e.g. an error occurs), it must send an unsolicited packet to the host.

This packet should be sent even if device has no address (i.e. it's address is
`0`).

Device can also send unsolicited packet up to once a minute if no other
communication from host is received.


|            | Unsolicited Update                     |
|------------|----------------------------------------|
| Packet     | `BB C8 00 00 DC`                       |
| Header     | `0xBB`                                 |
| Address    | `0xC8` (200)                           |
| FromHost   | `0` (from device)                      |
| Error      | `0` (no error)                         |
| EmbedData  | `0` (no data embedded)                 |
| Command    | `0x00` (`System`)                      |
| DataLength | `0x00` (no data)                       |
| HeaderCRC8 | `0xDC`                                 |

If device wants to report an error code, it can use a single data byte to do
so. Value `0` (if present) denotes no internal error. Please note that this
error will not cause `Error` bit to be set (as this is reserved for duplicate
detection).

|            | Unsolicited Update                     |
|------------|----------------------------------------|
| Packet     | `BB C9 20 01 B7`                       |
| Header     | `0xBB`                                 |
| Address    | `0xC9` (201)                           |
| FromHost   | `0` (from device)                      |
| Error      | `0` (no error)                         |
| EmbedData  | `1` (data embedded)                    |
| Command    | `0x00` (`System`)                      |
| Datum      | `0x01` (error code)                    |
| HeaderCRC8 | `0xB7`                                 |

#### Duplicate Address Detection

If device sees an unsolicited (`System`) packet from another device with
address matching its address (i.e. a duplicate), it should send an unsolicited
packet with `Error` bit set.

|            | Duplicate Detected                     |
|------------|----------------------------------------|
| Packet     | `BB 4C 40 00 BD`                       |
| Header     | `0xBB`                                 |
| FromHost   | `0` (sent from device)                 |
| Address    | `0x4C` (76)                            |
| Error      | `1` (error detected)                   |
| EmbedData  | `0` (no data embedded)                 |
| Command    | `0x00` (`System`)                      |
| Use CRC16  | `0` (CRC-8)                            |
| DataLength | `0x00` (no data)                       |
| HeaderCRC8 | `0xBD`                                 |

Packet must NOT be sent for `System` error packets (to avoid loops).

#### Update Request

Host can request an update from a device by either not having data or by having
`Datum` set to `0x00`.

|            | Status Request                         |
|------------|----------------------------------------|
| Packet     | `BB FE 80 00 F0`                       |
| Header     | `0xBB`                                 |
| Address    | `0xFE` (254)                           |
| FromHost   | `1` (from host)                        |
| Error      | `0` (no error)                         |
| EmbedData  | `0` (no data embedded)                 |
| Command    | `0x00` (`System`)                      |
| Use CRC16  | `0` (CRC-8)                            |
| DataLength | `0x00` (no data)                       |
| HeaderCRC8 | `0xD0`                                 |

Result of this action should be the same as the unsolicited status report.

|            | Status Update                          |
|------------|----------------------------------------|
| Packet     | `BB FE 00 00 BD`                       |
| Header     | `0xBB`                                 |
| Address    | `0xFE` (254)                           |
| FromHost   | `0` (from device)                      |
| Error      | `0` (no error)                         |
| EmbedData  | `1` (data embedded)                    |
| Command    | `0x00` (`System`)                      |
| Datum      | `0x00` (all ok)                        |
| HeaderCRC8 | `0xBD`                                 |

#### Device Reboot

Host can request a reboot of a single device by using 0x06 as a data.

|            | Device Reboot                          |
|------------|----------------------------------------|
| Packet     | `BB 20 A0 06 D0`                       |
| Header     | `0xBB`                                 |
| Address    | `0x20` (32)                            |
| FromHost   | `1` (from host)                        |
| Reply      | `0` (no reply)                         |
| EmbedData  | `1` (data embedded)                    |
| Command    | `0x00` (`System`)                      |
| Datum      | `0x06` (reboot)                        |
| HeaderCRC8 | `0xD0`                                 |

Reply is not supported.

#### Global Reboot

Host can also request a reboot of all devices.

|            | Broadcasted Device Reboot              |
|------------|----------------------------------------|
| Packet     | `BB 00 A0 06 3F`                       |
| Header     | `0xBB`                                 |
| FromHost   | `1` (from host)                        |
| Address    | `0x00` (0)                             |
| Reply      | `0` (no reply)                         |
| EmbedData  | `1` (data embedded)                    |
| Command    | `0x00` (`System`)                      |
| Datum      | `0x06` (reboot)                        |
| HeaderCRC8 | `0x3F`                                 |

Reply is not supported.

#### Blink Request

For the purpose of troubleshooting, one could ask device to turn on its Blink
mode (usually LED flashing at regular intervals).

|            | Turn On Blink                          |
|------------|----------------------------------------|
| Packet     | `BB 81 A0 08 49`                       |
| Header     | `0xBB`                                 |
| Address    | `0x81` (129)                           |
| FromHost   | `1` (from host)                        |
| Reply      | `0` (do not reply)                     |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x00` (`System`)                      |
| Datum      | `0x08` (set Blink on)                  |
| HeaderCRC8 | `0x49`                                 |

Reply is not supported.

#### Blink Off Request

One could ask device to turn off Blink mode.

|            | Turn Off Blink                         |
|------------|----------------------------------------|
| Packet     | `BB 81 A0 09 66`                       |
| Header     | `0xBB`                                 |
| Address    | `0x81` (129)                           |
| FromHost   | `1` (from host)                        |
| Reply      | `0` (do not reply)                     |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x00` (`System`)                      |
| Datum      | `0x09` (set Blink off)                 |
| HeaderCRC8 | `0x66`                                 |

Reply is not supported.

#### Firmware Upgrade

This action will enter a special programming mode in the device. Not all
devices are expected to support this action.

First device will stop for at least 1 second during which it will ignore any
input and clear all the buffers. Once 1 second has elapsed, all data that
follows on the same connection will be considered a part of the raw program
code. Once all memory is programmed, device will restart itself.

|            | Enter Firmware Upgrade Mode                             |
|------------|---------------------------------------------------------|
| Packet     | `BB DE 80 0C F4 FF 48 65 6C 6C 6F 20 57 6F 72 6C 64 9F` |
| Header     | `0xBB`                                                  |
| FromHost   | `1` (from host)                                         |
| Address    | `0xDE` (222)                                            |
| Reply      | `0` (no reply)                                          |
| EmbedData  | `0` (extra data follows)                                |
| Command    | `0x00` (`System`)                                       |
| Use CRC16  | `0` (CRC-8)                            |
| DataLength | `0x0C` (12)                                             |
| HeaderCRC8 | `0xF4`                                                  |
| Data       | `0xFF48656C6C6F20576F726C64` (firmware upgrade)         |
| DataCRC8   | `0x4E`                                                  |

Please note that data MUST match given signature.

Reply is not supported.


## Design Notes

### Device LEDs

Each device is recommended to have two LEDs. One LED would be dedicated to
status as follows.

| State   | Light                                         |
|---------|-----------------------------------------------|
| Normal  | LED fully turned on and blinking with traffic |
| UID     | LED is flashing fast (e.g., 10 Hz)            |

### Device Input

For most devices, it is recommended to allow entering `Config` mode (if any)
only using a button. To enter `Config` mode, one should detect press of a
button at least 3 seconds in length. Once device is in config mode, it can exit
it via short button press.

Unsolicited status update should be done on every click press (even short).

### Device Startup

When device starts, it should first listen to the bus in the increments of 0.1
second. Once it detects no traffic, it should send send unsolicited update.

### Stream Recovery

If a stream error occurs (e.g. detected by CRC-8), node should go into stream
recovery mode. Easiest way is searching for a start of the new packet
(`0xBB`). Once detected attempt should be made to read header. If it fails,
the next matching byte should be sought and the same operation should repeat.
Any erroneous packet start detection should be caught in a few bytes once
CRC-8 is checked against the assumed header.


## FAQ

### Why is there a separate header and data checksum?

This is so that one can avoid corruption of DataLength value causing an
excessive skipping of bytes.

### Why is CRC-16 used for longer data?

The maximum protection CRC-8 offers for HD=4 (detects all 3-bit errors) is only
when data length is 119 bits (14 bytes, including CRC itself) or less. Since
the majority (if not all) messages one can expect data length to be less than
14 bytes (1 byte for header CRC, 1 byte for data CRC itself, and a maximum of
12 bytes of data), it makes sense to allow for a faster and smaller CRC-8
algorithm to be used while going to a longer CRC only when absolutely needed.

This also allows for a vast majority of devices to omit support for CRC-16
altogether.

Chosen CRC-16 variant offers the same for up to 32751 bits which would detect
3-bit errors up to 4093 bytes. We only need to cover 130 bytes though (1 byte
for header CRC, 2 bytes for CRC itself, and a maximum of 127 data bytes).

### Why were polynomials 0x2F (CRC-8) and 0xA2EB (CRC-16) selected?

Selection of these polynomials is based on research done in
[Cyclic Redundancy Code (CRC) Polynomial Selection For Embedded Networks](https://users.ece.cmu.edu/~koopman/roses/dsn04/koopman04_crc_poly_embedded.pdf)
whitepaper and the updates done on [Koopman's website](https://users.ece.cmu.edu/~koopman/crc/).
Goal was to detect all 3-bit errors (HD=4) at all data lengths defined by
protocol. Not using input/output XOR and reflection was decided in order to
simplify code implementation on microcontroller.

Please note that whitepaper lists polynomials in their reversed reciprocal
form (pretty much only used by Koopman): `0x97` and `0xD175`.

### Is it possible to protect long data packet by CRC-8?

By clearing `UseCRC16` flag, it is possible to protect all 127 bytes of data
using just CRC-8. Do note this is not recommended as this only protects against
a single bit flip.

### Why Data CRC includes header CRC result?

This is to avoid unlikely case of header from one packet mixing with data
belonging to a different packet. By including the header CRC, the final data
CRC "links" the two together.
