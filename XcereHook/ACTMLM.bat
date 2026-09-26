:: =--------------------------------------------------=
:: | PUBLIC RELEASE | PUBLIC RELEASE | PUBLIC RELEASE |
:: =--------------------------------------------------=
:: ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
:: NOTICE: This Batch file is used to auto-copy the built DLL to your ML Mods folder.
:: Use this Batch file at your own risk. Verify directories below for your build env.

:: =------------------------------------------=
:: ACTMLM
:: (A)uto (C)opy (T)o (M)elon(L)oader (M)ods
:: ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
:: Developed by
:: n7fs (n0xcere) for XcereHook development.
:: =------------------------------------------=

@ECHO OFF
SETLOCAL ENABLEDELAYEDEXPANSION

TITLE XcereHook Post-Build Script [ACTMLM]

:: You must set the directory to the DLL here:
SET "XH=bin\x64\Release\net6.0\XcereHook.dll"

:: You must set the directory to the game's MelonLoader Mods folder here:
SET "STEAM=C:\Program Files (x86)\Steam\steamapps\common\Forward Assault\Mods"

:: Universal error variable to determine the value to return on exit.
SET "ERROR=0"

:: Does the directory location and file to XcereHook's DLL exist?
IF NOT EXIST "%XH%" (
	SET "ERROR=1"
	COLOR 4
	ECHO ERROR: XcereHook DLL Not Found!
	COLOR 3
	PAUSE
	GOTO Reset
)

:: Does the Steam folder containing Forward Assault and the MelonLoader Mods folder exist?
IF NOT EXIST "%STEAM%" (
	SET "ERROR=1"
	COLOR 4
	ECHO ERROR: Steam Game Directory Not Found!
	COLOR 3
	PAUSE
	GOTO Reset
)

:: Copy DLL to MelonLoader's Mods folder.
COPY /Y "%XH%" "%STEAM%\" >NUL

IF ERRORLEVEL 1 (
	SET "ERROR=1"
	COLOR 4
	ECHO ERROR: ACTMLM Failed!
	COLOR 3
	PAUSE
	GOTO Reset
)

COLOR 2
ECHO XH Mod DLL Copied Successfully!

:: Reset Command Prompt/Terminal back to its default style.
:Reset
COLOR
TITLE %COMSPEC%
EXIT /B %ERROR%