@echo off
set "$py=0"
call:construct

for /f "delims=" %%a in ('python #.py ^| findstr "2"') do set "$py=2"
for /f "delims=" %%a in ('python #.py ^| findstr "3"') do set "$py=3"
del #.py
goto:%$py%

echo python is not installed or python's path Path is not in the %%$path%% env. var
exit/b

:2
python tools/check_images2.py > missing_pictures.txt

exit/b

:3
python tools/check_images3.py > missing_pictures.txt

exit/b

:construct
echo import sys; print('{0[0]}.{0[1]}'.format(sys.version_info^)^) >#.py


