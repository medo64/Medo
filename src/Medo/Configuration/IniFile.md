# File Format

IniFile will read UTF-8 encoded INI configuration file. Such file will contain
nummber of sections, each consisting of key/value properties.


## Sections

Section is denoted by starting left square bracket (`[`) character followed by
section name and concluded by right square bracket (`]`) character or
end-of-line.

Section names are case-insensitive and multiple sections with the same name will
be considered as a same section for the purpose of lookup. Any starting or
ending white space will be removed.

If no section is defined, all properties will be assigned to empty (`""`)
section.


## Properties

Propery consists of key and value separated by colon (:) or equals (=)
character. Any line without a separator will be ignored.

Keys are case-insensitive and multiple keys with the same name will be
considered to be a part of array (albeit any API function that expects a single
value will use the last value defined).

Key will have their starting and ending whitespace removed. Key cannot contain
either equals (=) or colon (:) characters.

Anything after the key separator until either end-of-line or start of comment
is considered to be a value. Both starting and ending whitespace will be
removed. To preserve white space and/or allow value to containing otherwise
unsupported characters (e.g. `#`), it can be escaped or enclosed in quotes.

No line continuation is supported.


## Quoting

Value enclosed in single quote (`'`) characters will be used as is and the only
escape mechanism will be changing two consecutive single quotes (`''`) into a
single single quote character (e.g. `'That''s awesome'`).

Value enclosed in double quote (`"`) characters will allow for escaping.


## Escaping

The following escape characters are recognized:

* `\"`: double quote (`"`)
* `\'`: single quote (`'`)
* `\\`: backslash (`\`)
* `\0`: null character (NUL)
* `\a`: alert character (BEL)
* `\b`: backspace character (BS)
* `\f`: form feed character (FF)
* `\n`: new line character (LF)
* `\r`: carriage return character (CR)
* `\t`: tab character (HT)
* `\uHHHH`: unicode escape sequence (UTF-16, range: 0000-FFFF)
*  `\U00HHHHHH`: unicode escape sequence (UTF-32, range: 000000-10FFFF)
* `\v`: vertical tab character (VT)
* `\xH[H][H][H]`: variable length unicode escape sequence (range: 0-FFFF)

When using the `\x` escape sequence with less than 4 hex digits, if character
following escape sequence is a valid hex digit (i.e. 0-9, A-F, and a-f), it
will be included as part of escape sequence. If that's not desired, use full
4 hex characters (e.g. \x0041).

If any hexadecimal unicode sequence cannot be recognized, character will be
omitted (e.g. `\uGGGG`).

Any escaped value that's not defined will result in next character taken
verbatim (e.g. `\z` will result in `z`).


## Comments

Comment starts with a hash (#) or semicolon (;) character and any text coming
after it will be ignored until end-of-line is found. Both full line and in-line
comments are supported.
