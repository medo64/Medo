# BearBus Protocol Specification

BearBus is a message protocol intended for use on UART and similar interfaces.
It allows for communication between a single host and up to 127 devices using
up to 60 command codes.

Messages can contain up to 240 bytes of extra data either CRC-8 (recommended
for up to 12 bytes of data) or CRC-16 verified (13-240 bytes). The total
message length is thus between 5 bytes (up to a single data byte) and 247 bytes
(5 byte header, 240 bytes data, and 2 bytes CRC-16).

All commands either originate from host or have host as their destination.
There is no inter-device communication.

There are three distinct versions of packet:
* Short (only 5-byte header with a single byte of embedded data)
* Basic (up to 12 bytes of data are supported; 18 bytes total)
* Extended (up to 240 bytes of data are supported; 247 bytes total)

Packet is defined as follows:

|       |                                  Bit                  |                                |
| Octet |        7       |       6      | 5 | 4 | 3 | 2 | 1 | 0 |                                |
|-------|----------------|--------------|---|---|---|---|---|---|--------------------------------|
|   0:1 | Header[7:0] (`0xBB`)                                  |                                |
|   1:1 | Origin[7]      | Address[6:0]                         |                                |
|   2:1 | Reply/Error[7] | EmbedData[6] | Command[5:0]          |                                |
|   3:1 | DataLength[7:0]                                       | {EmbedData=0}                  |
|   3:1 | Datum[7:0]                                            | {EmbedData=1}                  |
|   4:1 | HeaderCRC8[7:0]                                       |                                |
|   5:N | Data                                                  | {EmbedData=0}                  |
| 5+N:1 | DataCRC8[7:0]                                         | {EmbedData=0} {DataLength<=12} |
| 5+N:2 | DataCRC16[15:0]                      |                | {EmbedData=0} {DataLength>12}  |


## Fields

### Header

`Header` is 8-bit field that always contains value 0xBB.

### Origin

`Origin` is 1-bit field denoting the origin of a message.

If value is `0`, a message originates from a device and is intended for the
host. If value is `1`, a message originates from the host and is intended for
a device.

### Address

`Address` is a 7-bit field specifying device that is either sender or receiver
of the message (depending on the value of `Origin` field). Value must be
between `1` and `127`.

If host is sending (`Origin`=`1`) a message to device address `0` (i.e., full
octet is `1000 0000`), a message is to be considered as a broadcast.

Devices without address must not send any replies.

### Reply/Error

`Reply/Error` is a 1-bit field serving two different purposes.

When sent from the host, it denotes if reply to this message is requested
(value `1`) or not (value `0`).

When received from a device, it denotes if message was a success (value `0`)
or an error (value `1`)

### EmbedData

This is a 1-bit field denoting if data is embedded within header (value `1`)
or is that field used as length of data (value `0`) and the actual data gets
added after the header.

### Command

`Command` is a 6-bit value specifying the command code of the message. Custom
codes are between `1` and `60`.

Requires codes are `0`, `61`, `62`, and `63`, based on the following table.
All required codes will use only Short packet format.

| Code   | Command     | Datum      | Origin  | Description                                                                                            |
|--------|-------------|------------|---------|--------------------------------------------------------------------------------------------------------|
| `0x00` | System      | (Action)   | Host    | Request system action.                                                                                 |
| `0x00` | System      | (Status)   | Device  | Unsolicited device update.                                                                             |
| `0x3D` | Ping        | (random)   | Host    | Checks connection to device.                                                                           |
| `0x3D` | Ping        | (random)   | (reply) | Response is sent with the same `Datum`.                                                                |
| `0x3E` | Status      | NewStatus  | Host    | Requests current status (or change to) of a device.                                                    |
| `0x3E` | Status      | CurrStatus | (reply) | Responds with the current status.                                                                      |
| `0x3F` | Address     | NewAddress | Host    | Changes address of node receiving the message.                                                         |
| `0x3F` | Address     | NewAddress | (reply) | After address change, reply is sent from the old address.                                              |

The following bits are defined for Status:

| Offset | Status       | Description                                                           |
|--------|--------------|-----------------------------------------------------------------------|
|      7 | Blink-Status | Status of LED blinking.                                               |
|    6:5 | Mode-Status  | Device mode (`00`: Normal, `01`: Config, `10`: Test, `11`: Program).  |
|      4 | Blink-Change | Whether LED blinking change is requested.                             |
|      3 | Mode-Change  | Whether mode change is requested.                                     |
|    2:0 | Device-Error | Error code. If no error is detected, value is `0x0`.                  |

### DataLength

This is the length of extra data after the header. Valid values are `1`-`240`.
Longer value will result in the invalid message.

Field exists only if `EmbedData`=`0`. Otherwise `Datum` occupies the same
space.

### Datum

This is a single byte of data built-in the header.

Field exists only if `EmbedData`=`1`. Otherwise `DataLength` occupies the same
space.

### HeaderCRC8

This is a CRC-8 (polynomial `0x2F`) value calculated over all preceding header
bytes. No CRC input XOR, output XOR or reflection is to be applied.

### Data

These are all extra bytes. Exists only if `EmbedData`=`0`.

### DataCRC8

This is a CRC-8 (polynomial `0x2F`) value calculated over HeaderCRC8 and all
preceding data bytes. No CRC input XOR, output XOR, or reflection is to be
applied.

Field exists only if `EmbedData`=`0` and `DataLength` is between `1` and `12`.

### DataCRC16

This is a CRC-16 (polynomial `0x755B`) value calculated over HeaderCRC8 and
all preceding data bytes. No CRC input XOR, output XOR or reflection is to be
applied.

Field is 2 octets in length.

Field exists only if `EmbedData`=`0` and `DataLength` is between `13` and
`240`.


## Example Messages

### Short Packet

This is an example of a short packet with a single data byte.

|             | Short Packet Example                   |
|-------------|----------------------------------------|
| Packet      | `BB 85 5D 42 DB`                       |
| Header      | `0xBB`                                 |
| Origin      | `1` (from host)                        |
| Address     | `0x05` (5)                             |
| Reply/Error | `0` (don't reply)                      |
| EmbedData   | `1` (using embedded data)              |
| Command     | `0x1D` (29)                            |
| Datum       | `0x42`                                 |
| HeaderCRC8  | `0xDB`                                 |

Total length of this packet is always 5 bytes and it contains 1 data byte.

### Basic Packet

This is an example of a packet with 3 bytes of data.

|             | Basic Packet Example                   |
|-------------|----------------------------------------|
| Packet      | `BB 93 1A 03 83 42 43 44 06`           |
| Header      | `0xBB`                                 |
| Origin      | `1` (from host)                        |
| Address     | `0x13` (19)                            |
| Reply/Error | `0` (don't reply)                      |
| EmbedData   | `0` (using extra data)                 |
| Command     | `0x1A` (26)                            |
| DataLength  | `0x03` (3)                             |
| HeaderCRC8  | `0x83`                                 |
| Data        | `0x424344`                             |
| DataCRC8    | `0x06`                                 |

Total length of this packet is between 8 bytes (2 data bytes) and 19 bytes (13
data bytes).

### Long Packet

This is an example of a long packet with 13 bytes of data.

|             | Long Packet Example                                           |
|-------------|---------------------------------------------------------------|
| Packet      | `BB 81 01 0D 7E 42 43 44 45 46 47 48 49 4A 4B 4C 4D 4E D1 69` |
| Header      | `0xBB`                                                        |
| Origin      | `1` (from host)                                               |
| Address     | `0x01` (1)                                                    |
| Reply/Error | `0` (don't reply)                                             |
| EmbedData   | `0` (using extra data)                                        |
| Command     | `0x01` (1)                                                    |
| DataLength  | `0x0D` (13)                                                   |
| HeaderCRC8  | `0x7E`                                                        |
| Data        | `0x42434445464748494A4B4C4D4E`                                |
| DataCRC16   | `0xD169` (since more than 12 data bytes are used)             |

Total length of this packet is between 21 bytes (14 data bytes) and 247 bytes
(240 data bytes). Most microcontroller devices are not expected to support
this packet type.


## Procedures

### System

#### Device Reset

Host can request reset of a single device by using 0x06 as a data.

|            | Device Reset                           |
|------------|----------------------------------------|
| Packet     | `BB A0 40 06 C4`                       |
| Header     | `0xBB`                                 |
| Origin     | `1` (from host)                        |
| Address    | `0x20` (32)                            |
| Reply      | `0` (no reply)                         |
| EmbedData  | `1` (data embedded)                    |
| Command    | `0x00` (`System`)                      |
| Datum      | `0x06` (reboot)                        |
| HeaderCRC8 | `0xC4`                                 |

Reply is not supported.

#### Global Reset

Host can also request reset of all devices.

|            | Broadcasted Device Reset               |
|------------|----------------------------------------|
| Packet     | `BB 80 40 06 2B`                       |
| Header     | `0xBB`                                 |
| Origin     | `1` (from host)                        |
| Address    | `0x00` (0)                             |
| Reply      | `0` (no reply)                         |
| EmbedData  | `1` (data embedded)                    |
| Command    | `0x00` (`System`)                      |
| Datum      | `0x06` (reboot)                        |
| HeaderCRC8 | `0x2B`                                 |

Reply is not supported.

#### Status Update

When device is powered on or after it changes its address, it must send an
unsolicited packet to the host.

|            | Unsolicited Status Update              |
|------------|----------------------------------------|
| Packet     | `BB 22 40 00 F7`                       |
| Header     | `0xBB`                                 |
| Origin     | `0` (from device)                      |
| Address    | `0x22` (34)                            |
| Error      | `0` (no error)                         |
| EmbedData  | `1` (data embedded)                    |
| Command    | `0x00` (`System`)                      |
| Datum      | `0x00` (no light, normal mode)         |
| HeaderCRC8 | `0xF7`                                 |

#### Duplicate Address Detection

If device sees an unsolicited packet from another device with address matching
its address (i.e. a duplicate), it should send an unsolicited packet with
`Error` bit set.

|            | Duplicate Detected                     |
|------------|----------------------------------------|
| Packet     | `BB 4C C0 00 BA`                       |
| Header     | `0xBB`                                 |
| Origin     | `0` (sent from device)                 |
| Address    | `0x4C` (76)                            |
| Error      | `1` (error detected)                   |
| EmbedData  | `1` (data embedded)                    |
| Command    | `0x00` (`System`)                      |
| Datum      | `0x00` (no light, normal mode)         |
| HeaderCRC8 | `0xBA`                                 |

Packet must NOT be sent for `System` error packets (to avoid loops).

#### Mode Update

If device uses a physical button to change a mode or it goes into a permanent
error, it can send an unsolicited update to the host. In case of the physical
button, update should be sent every time button is clicked (even if click
didn't result in mode change).

|            | Unsolicited Mode/Error Update          |
|------------|----------------------------------------|
| Packet     | `BB 2F 7E 22 5E`                       |
| Header     | `0xBB`                                 |
| Origin     | `0` (sent from device)                 |
| Address    | `0x2F` (47)                            |
| Error      | `0` (not error reply)                  |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x00` (`System`)                      |
| Datum      | `0x22` (Config mode, device error 2)   |
| HeaderCRC8 | `0x5E`                                 |

### Ping

#### Ping Check

To verify if device is alive on the bus, host can request a ping.

|            | Ping Request                           |
|------------|----------------------------------------|
| Packet     | `BB 8F 7D 42 XX`                       |
| Header     | `0xBB`                                 |
| Origin     | `1` (from host)                        |
| Address    | `0x0F` (15)                            |
| Reply      | `1` (do reply)                         |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x3D` (`Ping`)                        |
| Datum      | `0x42` (random)                        |
| HeaderCRC8 | `0xFD`                                 |

If device recognizes its Address, it must respond with the same random number.

|            | Ping Response                          |
|------------|----------------------------------------|
| Packet     | `BB 0F 7D 42 30`                       |
| Header     | `0xBB`                                 |
| Origin     | `0` (reply from device)                |
| Address    | `0x0F` (15)                            |
| Error      | `0` (no error)                         |
| EmbedData  | `0` (no embedded data)                 |
| Command    | `0x3D` (`Ping`)                        |
| Datum      | `0x30` (random)                        |
| HeaderCRC8 | `0xA4`                                 |

#### Check Status

To check current status, host can request the same.

|            | Check Current Status                   |
|------------|----------------------------------------|
| Packet     | `BB AF BE 00 2D`                       |
| Header     | `0xBB`                                 |
| Origin     | `1` (from host)                        |
| Address    | `0x2F` (47)                            |
| Reply      | `1` (do reply)                         |
| EmbedData  | `0` (no embedded data)                 |
| Command    | `0x3E` (`Status`)                      |
| Datum      | `0x00` (no data)                       |
| HeaderCRC8 | `0x2D`                                 |

Normal status reply would be as follows.

|            | Reply with Current Status              |
|------------|----------------------------------------|
| Packet     | `BB 2F 7E 00 73`                       |
| Header     | `0xBB`                                 |
| Origin     | `0` (reply from device)                |
| Address    | `0x2F` (47)                            |
| Error      | `0` (no error)                         |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x3E` (`Status`)                      |
| Datum      | `0x00` (no special mode, no error)     |
| HeaderCRC8 | `0x73`                                 |

If there is a device error, response might look like this.

|            | Reply with Current Status (Device Error)   |
|------------|--------------------------------------------|
| Packet     | `BB 2F 7E 06 91`                           |
| Header     | `0xBB`                                     |
| Origin     | `0` (reply from device)                    |
| Address    | `0x2F` (47)                                |
| Error      | `0` (not error reply)                      |
| EmbedData  | `1` (using embedded data)                  |
| Command    | `0x3E` (`Status`)                          |
| Datum      | `0x06` (no special mode, device error 0x6) |
| HeaderCRC8 | `0x91`                                     |

### Status

#### Turn On Blink

For the purpose of troubleshooting, one could ask device to turn on its Blink
mode (usually LED flashing at regular intervals).

|            | Turn On Blink                          |
|------------|----------------------------------------|
| Packet     | `BB AF FE 90 F4`                       |
| Header     | `0xBB`                                 |
| Origin     | `1` (from host)                        |
| Address    | `0x2F` (47)                            |
| Reply      | `1` (do reply)                         |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x3E` (`Status`)                      |
| Datum      | `0x90` (Set Blink)                     |
| HeaderCRC8 | `0xBB`                                 |

Reply would be as follows (assuming Blink setup is successful).

|            | Reply with Current Status              |
|------------|----------------------------------------|
| Packet     | `BB 2F 7E 80 90`                       |
| Header     | `0xBB`                                 |
| Origin     | `0` (reply from device)                |
| Address    | `0x2F` (47)                            |
| Error      | `0` (no error)                         |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x3E` (`Status`)                      |
| Datum      | `0x80` (Blink on, no device error)     |
| HeaderCRC8 | `0x90`                                 |

If there is no Blink light available, reply would be just a current status.

|            | Reply with Current Status              |
|------------|----------------------------------------|
| Packet     | `BB 2F FE 00 74`                       |
| Header     | `0xBB`                                 |
| Origin     | `0` (reply from device)                |
| Address    | `0x2F` (47)                            |
| Error      | `1` (error reply)                      |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x3E` (`Status`)                      |
| Datum      | `0x00` (normal mode, no device error)  |
| HeaderCRC8 | `0x74`                                 |

#### Change Mode

If one wants switch to other mode (e.g. Config), packet would looks as
follows. Please note only UID and Mode are applicable.

|            | Switch Mode to Config                  |
|------------|----------------------------------------|
| Packet     | `BB AF FE 28 9D`                       |
| Header     | `0xBB`                                 |
| Origin     | `1` (from host)                        |
| Address    | `0x2F` (47)                            |
| Reply      | `1` (do reply)                         |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x3E` (`Status`)                      |
| Datum      | `0x28` (Update, Cofnig mode)           |
| HeaderCRC8 | `0x9D`                                 |

Reply would be as follows (assuming switch to Config mode is successful).

|            | Reply with Current Status              |
|------------|----------------------------------------|
| Packet     | `BB 2F 7E 20 00`                       |
| Header     | `0xBB`                                 |
| Origin     | `0` (reply from device)                |
| Address    | `0x2F` (47)                            |
| Error      | `0` (no error)                         |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x3E` (`Status`)                      |
| Datum      | `0x20` (Config mode, no device error)  |
| HeaderCRC8 | `0x00`                                 |

If switch to Config mode is not possible (e.g. not supported), reply would be
just a current status. Please note that support for mode change if fully
optional.

|            | Reply with Current Status              |
|------------|----------------------------------------|
| Packet     | `BB 2F FE 00 74`                       |
| Header     | `0xBB`                                 |
| Origin     | `0` (reply from device)                |
| Address    | `0x2F` (47)                            |
| Error      | `1` (error reply)                      |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x3E` (`Status`)                      |
| Datum      | `0x00` (normal mode, no device error)  |
| HeaderCRC8 | `0x74`                                 |

### Address

#### Address Setup

If a device has no address, it will allow setting an address without `Config`
mode being set.

|            | Initial Address setup                  |
|------------|----------------------------------------|
| Packet     | `BB 80 7F 4D C0`                       |
| Header     | `0xBB`                                 |
| Origin     | `1` (from host)                        |
| Address    | `0x00` (broadcast)                     |
| Reply      | `0` (don't reply)                      |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x3F` (`Address`)                     |
| Datum      | `0x4D` (new address is 77)             |
| HeaderCRC8 | `0xC0`                                 |

Since a reply is not requested, only an unsolicited ping will be sent (if a
reply was requested, first a reply would be sent and ping would follow after).

|            | Unsolicited Update For Address Change  |
|------------|----------------------------------------|
| Packet     | `BB 4D 40 80 50`                       |
| Header     | `0xBB`                                 |
| Origin     | `0` (reply from device)                |
| Address    | `0x4D` (77)                            |
| Error      | `0` (no error)                         |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x00` (`System`)                      |
| Datum      | `0x80` (Blink is on)                   |
| HeaderCRC8 | `0x50`                                 |

#### Address Change

If a device already has an address, to change it, a device has to be in the
`Config` mode.

|            | Address Change                         |
|------------|----------------------------------------|
| Packet     | `BB 83 FF 4D D5`                       |
| Header     | `0xBB`                                 |
| Origin     | `1` (from host)                        |
| Address    | `0x03` (current address is 3)          |
| Reply      | `1` (do reply)                         |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x3F` (`Address`)                     |
| Datum      | `0x4D` (new address is 77)             |
| HeaderCRC8 | `0xD5`                                 |

Reply would be as follows (assuming the address was successfuly set).

|            | Reply for Successful Address Change    |
|------------|----------------------------------------|
| Packet     | `BB 03 7F 4D 1F`                       |
| Header     | `0xBB`                                 |
| Origin     | `0` (reply from device)                |
| Address    | `0x03` (3)                             |
| Error      | `0` (no error)                         |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x3F` (`Address`)                     |
| Datum      | `0x4D` (new address)                   |
| HeaderCRC8 | `0x1F`                                 |

Unsolicited ping is sent from the device when address changes.

|            | Unsolicited Update For Address Change  |
|------------|----------------------------------------|
| Packet     | `BB 4D 40 80 50`                       |
| Header     | `0xBB`                                 |
| Origin     | `0` (reply from device)                |
| Address    | `0x4D` (77)                            |
| Error      | `0` (no error)                         |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x00` (`System`)                      |
| Datum      | `0x80` (Blink is on)                   |
| HeaderCRC8 | `0x50`                                 |

If setting the address fails for any reason, no unsolicited ping will be send
and reply will contain error.

|            | Unsuccessful Address Change Reply      |
|------------|----------------------------------------|
| Packet     | `BB 03 FF 4D 18`                       |
| Header     | `0xBB`                                 |
| Origin     | `0` (reply from device)                |
| Address    | `0x03` (3)                             |
| Error      | `1` (error changing address)           |
| EmbedData  | `1` (using embedded data)              |
| Command    | `0x3F` (`Address`)                     |
| Datum      | `0x4D` (failed address)                |
| HeaderCRC8 | `0x18`                                 |


## Design Notes

### Device LEDs

Each device is recommended to have two LEDs. One LED would be dedicated to
status as follows.

| State   | Light                                     |
|---------|-------------------------------------------|
| Normal  | LED fully turned on                       |
| Config  | LED is flashing slowly (e.g., 1 Hz)       |
| Test    | LED is flashing fast (e.g. 3 Hz           |
| Program | LED is flashing extra fast (e.g., 30 Hz)  |
| UID     | LED is flashing really fast (e.g., 10 Hz) |

At minimum, device should support `Normal` and `UID` states.

Other LED would be for other purposes device might have (e.g. showing traffic,
etc.)

### Device Input

For most devices, it is recommended to allow entering `Config` mode only using
a button. To enter `Config` mode, one should detect press of a button at least
3 seconds in length. Once device is in config mode, it can exit it via simple
button press or by clearing `Config` via packet.

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

This is so that one can avoid corruption of DataLength value causing excessive
skipping of bytes.

### Why is CRC-16 used for longer data?

The maximum protection CRC-8 offers for HD=4 (detects all 3-bit errors) is only
when data length is 119 bits (14 bytes, including CRC itself) or less. Chosen
CRC-16 variant offers the same for up to 2048 bits (256 bytes) of data. Since
for the majority (if not all) messages one can expect data length to be less
than 14 bytes (1 byte for header CRC, 1 byte for data CRC itself, and  maximum
of 12 bytes of data), it makes sense to allow for a faster and smaller CRC-8
algorithm to be used while going to a longer CRC only when absolutely needed.

This also allows for vast majority of devices to omit support for CRC-16
altogether.

### Why were polynomials 0x2F (CRC-8) and 0x755B (CRC-16) selected?

Selection of these polynomials is based on research done in
[Cyclic Redundancy Code (CRC) Polynomial Selection For Embedded Networks](https://users.ece.cmu.edu/~koopman/roses/dsn04/koopman04_crc_poly_embedded.pdf)
whitepaper. Goal was to detect all 3-bit errors (HD=4) at all data lengths
defined by protocol. Not using input/output XOR and reflection was decided in
order to simplify code implementation on microcontroller.

Please note that whitepaper lists polynomials in their reversed reciprocal
form (pretty much only used by Koopman): `0x97` and `0xBAAD`.

### Why is maximum data size limited to 240 bytes?

For easier parsing on microcontrollers, it was desired that the whole packet
length fits in uint8_t variable. Since header is 5 bytes and CRC-16 is
additional 2 bytes, that means that the maximum packet length would be 248
bytes. In order to avoid overflow, 1 extra byte was deducted thus leaving the
maximum length at 247 bytes which I find a bit ugly. Value 240 is the next
lower value divisible by 8.

### Why Data CRC includes header CRC result?

This is to avoid unlikely case of header from one packet mixing with data
belonging to a different packet. By including header CRC, the final data CRC
"links" the two together.
