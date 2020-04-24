#!/usr/bin/env python
# usage: txtsplit.py <input_file_name> <output_dir>
# splits a txt2gam file into individual location files
# encoded in utf-8, for git to better handle

import os
import sys
import re
import io
import xml.etree.ElementTree as ET

assert len(sys.argv) == 3, "usage:\ntxtsplit.py <input_file_name> <output_dir>"
iname = str(sys.argv[1])
odir = str(sys.argv[2])

# read the project xml file first
# let's do this later in order to implement directory structure
"""
tree = ET.parse('glife.qproj')
root = tree.getroot()
"""

ifile = io.open(iname, 'rt', encoding='utf-16')

counter = 1

oname = None
firstline = ifile.readline().replace(u'\ufeff','')
match = re.search(ur'^#\s(\$?[_.\w]+)$', firstline)
if match:
    oname = os.path.join(odir, match.group(1).replace("$","_") + '.qsrc' )
    counter += 1
assert oname, "file is in the wrong format, must start with a location name"

ofile = io.open(oname, 'w', encoding='utf-8')
ofile.write(firstline)

for line in ifile:
    match = re.search(ur'^#\s(\$?[_.\w]+)$', line)
    if match:
        ofile.close()
        oname = os.path.join(odir, match.group(1).replace("$","_") + '.qsrc' )
        counter += 1
        ofile = io.open(oname, 'w', encoding='utf-8')
    ofile.write(line)
        
