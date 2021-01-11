#!/usr/bin/env python
# checks to make sure that glife.qproj is in sync with the location files

import os
import sys
import re
import io
import xml.etree.ElementTree as ET
import os.path

# read the project xml file first
tree = ET.parse('glife.qproj')
root = tree.getroot()


xml_locations = []
os_locations = os.listdir("locations")

print "Locations missing from ./locations/:"
for location in root.iter('Location'):
    name = location.attrib['name'].replace("$","_")
    xml_locations.append(name)
    
    if name not in os_locations:
        print name


print "Locations missing from .qproj:"
for name in os_locations:
    if name not in xml_locations:
        print name
