#!/usr/bin/env python
# usage: txtmerge.py <input_dir> <output_file_name> 
# does the exact opposite of txtsplit.py

import os
import sys
import re
import io 
import xml.etree.ElementTree as ET

assert len(sys.argv) == 3, "usage:\ntxtmerge.py <input_dir> <output_file_name>"
idir = str(sys.argv[1])
oname = str(sys.argv[2])

# read the project xml file first
# let's do this later in order to implement directory structure
tree = ET.parse('glife.qproj')
root = tree.getroot()


ofile = io.open(oname, 'w', encoding='utf-16', newline='\r\n')

for location in root.iter('Location'):
    iname = location.attrib['name']
    iname = iname.replace("$","_")

    try:
        ifile = io.open(os.path.join(idir,iname + '.qsrc'), 'rt', encoding='utf-8')
        text = ifile.read()

        # make sure there's a line at the end of file
        # (why wouldn't there be one? WINDOWS!
        if text[-1] != u'\n':
            text += u'\n\n'

        ofile.write(text)
        ifile.close()
    except IOError:
        print("WARNING: missing location %s" % iname)
        pass

ofile.close()
    
        
