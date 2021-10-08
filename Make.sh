#!/bin/bash

if [ -t 1 ]; then
    ANSI_RESET="$(tput sgr0)"
    ANSI_UNDERLINE="$(tput smul)"
    ANSI_RED="`[ $(tput colors) -ge 16 ] && tput setaf 9 || tput setaf 1 bold`"
    ANSI_YELLOW="`[ $(tput colors) -ge 16 ] && tput setaf 11 || tput setaf 3 bold`"
    ANSI_CYAN="`[ $(tput colors) -ge 16 ] && tput setaf 14 || tput setaf 6 bold`"
fi

while getopts ":h" OPT; do
    case $OPT in
        h)
            echo
            echo    "  SYNOPSIS"
            echo -e "  $(basename "$0") [${ANSI_UNDERLINE}operation${ANSI_RESET}]"
            echo
            echo -e "    ${ANSI_UNDERLINE}operation${ANSI_RESET}"
            echo    "    Operation to perform (all, clean, debug, release, test)."
            echo
            echo    "  DESCRIPTION"
            echo    "  Make script compatible with both Windows and Linux."
            echo
            echo    "  SAMPLES"
            echo    "  $(basename "$0")"
            echo    "  $(basename "$0") all"
            echo
            exit 0
        ;;

        \?) echo "${ANSI_RED}Invalid option: -$OPTARG!${ANSI_RESET}" >&2 ; exit 1 ;;
        :)  echo "${ANSI_RED}Option -$OPTARG requires an argument!${ANSI_RESET}" >&2 ; exit 1 ;;
    esac
done

if ! command -v dotnet >/dev/null; then
    echo "${ANSI_RED}No dotnet found!${ANSI_RESET}" >&2
    exit 1
fi

trap "exit 255" SIGHUP SIGINT SIGQUIT SIGPIPE SIGTERM
trap "echo -n \"$ANSI_RESET\"" EXIT

BASE_DIRECTORY="$( cd "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"

SOLUTION_FILES="Medo.sln"
OUTPUT_FILES="Medo.dll Medo.pdb"

if [[ `uname -o` == "Msys" ]]; then  # assume Windows
    SOLUTION_FILES="$SOLUTION_FILES Medo.Windows.Forms.sln"
    OUTPUT_FILES="$OUTPUT_FILES Medo.Windows.Forms.dll Medo.Windows.Forms.pdb"
fi


function clean() {
    rm -r "$BASE_DIRECTORY/bin/" 2>/dev/null
    rm -r "$BASE_DIRECTORY/build/" 2>/dev/null
    rm -r "$BASE_DIRECTORY/src/**/bin/" 2>/dev/null
    rm -r "$BASE_DIRECTORY/src/**/obj/" 2>/dev/null
    return 0
}

function debug() {
    echo ".NET `dotnet --version`"
    mkdir -p "$BASE_DIRECTORY/bin/"
    mkdir -p "$BASE_DIRECTORY/build/debug/"
    for SOLUTION_FILE in $SOLUTION_FILES; do
        dotnet build "$BASE_DIRECTORY/src/$SOLUTION_FILE" \
                    --configuration "Debug" \
                    --output "$BASE_DIRECTORY/build/debug/" \
                    --verbosity "minimal" \
                    || return 1
        echo
    done
    for FILE in $OUTPUT_FILES; do
        cp "$BASE_DIRECTORY/build/debug/$FILE" "$BASE_DIRECTORY/bin/$FILE" || return 1
    done
    echo "${ANSI_CYAN}Output in 'bin/'${ANSI_RESET}"
}

function release() {
    if [[ `shell git status -s 2>/dev/null | wc -l` -gt 0 ]]; then
        echo "${ANSI_YELLOW}Uncommited changes present.${ANSI_RESET}" >&2
    fi
    echo ".NET `dotnet --version`"
    mkdir -p "$BASE_DIRECTORY/bin/"
    mkdir -p "$BASE_DIRECTORY/build/release/"
    for SOLUTION_FILE in $SOLUTION_FILES; do
        dotnet build "$BASE_DIRECTORY/src/$SOLUTION_FILE" \
                    --configuration "Release" \
                    --output "$BASE_DIRECTORY/build/release/" \
                    --verbosity "minimal" \
                    || return 1
        echo
    done
    for FILE in $OUTPUT_FILES; do
        cp "$BASE_DIRECTORY/build/release/$FILE" "$BASE_DIRECTORY/bin/$FILE" || return 1
    done
    echo "${ANSI_CYAN}Output in 'bin/'${ANSI_RESET}"
}

function test() {
    echo ".NET `dotnet --version`"
    mkdir -p "$BASE_DIRECTORY/build/tests/"
    for SOLUTION_FILE in $SOLUTION_FILES; do
        dotnet test "$BASE_DIRECTORY/src/$SOLUTION_FILE" \
                    --configuration "Debug" \
                    --output "$BASE_DIRECTORY/build/tests/" \
                    --verbosity "minimal" \
                    || return 1
        echo
    done
}


if [ $# -gt 1 ]; then
    echo "${ANSI_RED}Too many arguments!${ANSI_RESET}" >&2
    exit 1
fi

OK=0
OPERATION="$1"
if [[ "$OPERATION" == "" ]]; then OPERATION="all"; fi

case "$OPERATION" in
    all)        clean && OK=1 ; release && OK=1 ;;
    clean)      clean && OK=1 ;;
    debug)      debug && OK=1 ;;
    release)    release && OK=1 ;;
    test)       test && OK=1 ;;

    *)  echo "${ANSI_RED}Unknown operation '$OPERATION'!${ANSI_RESET}" >&2 ; exit 1 ;;
esac

if ! [[ $OK  ]]; then
    echo "${ANSI_RED}Error performing '$OPERATION' operation!${ANSI_RESET}" >&2
    exit 1
fi
