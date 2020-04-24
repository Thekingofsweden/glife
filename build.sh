#!/usr/bin/env bash
set -x

# Set those lines to fit your setup.
# This is where glife.qsp will be copied. If you don't want to move it just comment (#) the line below.
#DESTDIR=../GL_ECV

# The file that will be generated or open
QSPFILE=glife.qsp

#######################################################################

./txtmerge.py locations glife.txt
if [[ "$OSTYPE" == "linux-gnu" ]]; then
	./txt2gam.linux glife.txt "${QSPFILE}" 1> /dev/null
elif [[ "$OSTYPE" == "darwin"* ]]; then
	./txt2gam.mac glife.txt "${QSPFILE}" 1> /dev/null
elif [[ "$OSTYPE" == "msys" ]]; then
	if [[ "$MSYSTEM_CARCH" == "x86_64" ]]; then
		./txt2gam64.exe glife.txt "${QSPFILE}" 1> /dev/null
	else
		./txt2gam.exe glife.txt "${QSPFILE}" 1> /dev/null
	fi
elif [[ "$OSTYPE" == "cygwin" ]]; then
	# Unsure if non-AMD CPUs give this or AMD64, tested on an actual AMD brand chip
	# requires a python install visible to cygwin (for the txtmerge above)
	if [[ "$PROCESSOR_ARCHITECTURE" == "x86_64" ]]; then
		./txt2gam64.exe glife.txt "${QSPFILE}" 1> /dev/null
	elif [[ "$PROCESSOR_ARCHITECTURE" == "AMD64" ]]; then
		./txt2gam64.exe glife.txt "${QSPFILE}" 1> /dev/null
	else
		./txt2gam.exe glife.txt "${QSPFILE}" 1> /dev/null
	fi
fi
if [ -d "${DESTDIR}" ]; then
	cp --reflink=auto "${QSPFILE}" "${DESTDIR}"
fi

