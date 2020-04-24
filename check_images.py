#!/usr/bin/env python

import io
import os
import re

#path = os.getcwd()
#print path

startpattern = """images/"""
imgFormats = ['jpg','gif']

infile = io.open('glife.txt',mode='r',encoding='utf-16')
lines = infile.readlines()

images = []

for name in os.listdir("locations"):
    ifile = io.open(
        os.path.join("locations", name),
        mode='rt',
        encoding='utf-8'
    )
    text = ifile.read()
    for match in re.finditer(r"images.+?[.](gif|jpg|png)", text, flags=re.U):
        imgfile = match.group().encode("utf-8")
        randmatch = re.search(r"'\s*[+]\s*rand\s*[(]\s*(\d+)\s*[,]\s*(\d+)\s*[)]\s*[+]\s*'", imgfile)
        if randmatch != None:
            for i in range(int(randmatch.group(1)), 1+int(randmatch.group(2))):
                images.append(re.sub(r"'\s*[+]\s*rand\s*[(].*?[)]\s*[+]\s*'", str(i), imgfile))
        else:
            images.append(imgfile)
    
    ifile.close()


for image in images:
    if not re.search(r"[<$]", image) and not os.path.isfile(image):
        print "Image not found:", image
