@ECHO off

:: Set those lines to fit your setup. 
:: This is where glife.qsp will be copied. If you don't want to move it just comment (::) the line below.
:: set CP_TO=..\GL_ECV

:: This is the program used to open the QSPFILE. If you comment this line windows will launch the default app for the ".qsp" extension.
set QSPGUI=QSP\Player-video\qspgui.exe
set QGEN=QSP\QGen5\QGen.exe

:: The file that will be generated or open
set QSPFILE=glife.qsp

::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

:menu
cls
echo.
echo :: QSP Compiler and Launcher
echo.

if defined QGEN (
	if not exist "%QGEN%" ( 
		echo QGEN     : [ERROR] - %QGEN%  not found. Using DEFAULT application.
		set QGEN=
	) else ( echo QGEN     : [OK] - "%QGEN%")
) else echo QGEN     : [NOT DEFINED] - Using DEFAULT application.

if defined QSPGUI (
	if not exist "%QSPGUI%" ( 
		echo QSP EXEC : [ERROR] - %QSPGUI% not found.
		set QSPGUI=
	) else ( echo QSP EXEC : [OK] - "%QSPGUI%")
) else ( echo QSP EXEC : [NOT DEFINED] - Using Windows DEFAULT.)

if defined QSPFILE (
	if not exist "%QSPFILE%" ( 
		echo QSP FILE : [WARNING] - %QSPFILE% not found.
	) else ( echo QSP FILE : [OK] - "%QSPFILE%")
) else ( echo QSP FILE : [NOT DEFINED] - ERROR: CAN'T CONTINUE.)

if defined CP_TO (
	if not exist "%CP_TO%" (
		echo COPY     : [ERROR] - Destination "%CP_TO%" not found. Copy DISABLED.
		set CP_TO=
	) else ( echo COPY     : [OK] - "%CP_TO%")
) else (  echo COPY     : [DISABLED] )

echo.

if defined NOT_FOUND (
	echo ERROR: Option '%action%' wasn't recognized. Is it lowercase? 
	set NOT_FOUND=
)

echo.
echo ACTIONS: (B)uild  (R)un  (F)ull  (Q)Gen  (E)xit
echo.
set /p action=Choose an action:

if defined QSPFILE (
	if %action% == b goto build
	if %action% == r goto run
	if %action% == f goto build
	if %action% == q goto qgen
)

if %action% == e goto exit

set NOT_FOUND=1
goto menu

:build
echo.
echo Building ...

@ECHO ON
python txtmerge.py locations glife.txt
txt2gam.exe glife.txt %QSPFILE% > nul
@ECHO OFF

echo.
if defined CP_TO ( echo Copying %QSPFILE% to "%CP_TO%" ... & copy %QSPFILE% %CP_TO% > nul )

echo.
echo Done.
pause
if %action% == f ( goto run ) else ( goto menu )

:qgen
echo.
echo Running ...
if defined CP_TO ( start %QGEN% %CP_TO%\%QSPFILE% ) else ( start %QGEN% %QSPFILE% )
goto exit

:run
echo.
echo Running ...

if defined CP_TO ( start %QSPGUI% %CP_TO%\%QSPFILE% ) else ( start %QSPGUI% %QSPFILE% )

:exit