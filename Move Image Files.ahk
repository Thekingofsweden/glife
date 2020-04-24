/*
 *		-- Move Image Files using 'Image Files List.txt'--
 *
 *		Requires images subfolder to exist in current directory
 *		
 *		ImageFileList		- List of file names from glife.txt and Auto List
 *		UsedImageFolder		- Folder to move images in ImageFileList
 *		UnUsedImageFolder	- Folder to move images not in ImageFileList
 *		SourceFolder		- Folder with images
 *          
 *
 *      WD: Jul 2015
 *
 *
 */
  
  

#NoEnv  ; Recommended for performance and compatibility with future AutoHotkey releases.
#Warn  ; Enable warnings to assist with detecting common errors.
SendMode Input  ; Recommended for new scripts due to its superior speed and reliability.
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory.

ImageFileList		= Image List - Used.txt
UsedImageFolder		= Images, Used
UnUsedImageFolder	= Images, Un-Used
SourceFolder		= images

FileEncoding, UTF-16

;-- backup files  and delete --
IfNotExist %ImageFileList%
{
	MsgBox, 16, Move Image Files, Unable to locate "%ImageFileList%" in "%A_WorkingDir%", 10
	Exit
}

IfNotExist %UsedImageFolder%
{
	FileCreateDir, %UsedImageFolder%
}

IfNotExist %UnUsedImageFolder%
{
	FileCreateDir, %UnUsedImageFolder%
}

; -- load data from file --
FileRead, Images, %ImageFileList%
Images := StrReplace(Images, "/", "\")								;; Use correct Win path seperator

; -- Parse data one line at a time --
Loop, Parse, Images, `n, `r  			; Specifying `n prior to `r allows both Windows and Unix files to be parsed.
{
	if trim(A_LoopField) = ""										;; Skip blank line
		continue
	
	IfExist, %A_LoopField%
	{
		; -- Create Dir if id dosn't exist --
		SplitPath, A_LoopField, FileName, FileDir
		IfNotExist  %UsedImageFolder%\%FileDir%
		{
			FileCreateDir, %UsedImageFolder%\%FileDir%
			if (ErrorLevel)
			{
				MsgBox, 48, Move Image File, FileCreateDir Error:`n`nFailed to create '%UsedImageFolder%\%FileDir%'`n`nErrorLevel: '%ErrorLevel%'`nLastError: '%A_LastError%'	
			}
		}
		
		; -- move file --
		FileMove, %A_LoopField%, %UsedImageFolder%\%FileDir%
		if (ErrorLevel)
		{
			MsgBox, 48, Move Image File, FileMove Error:`n`nFailed to move '%A_LoopField%' to '%UsedImageFolder%\%FileDir%'`n`nErrorLevel: '%ErrorLevel%'`nLastError: '%A_LastError%'	
		}
	}
	else	; -- File dosn't exist --
	{
		MsgBox, 64, Move Image Files, Unable to locate file : '%A_LoopField%', 5
	}
}

; -- Rename Images folder to unused Images folder --
IfExist %SourceFolder%
{
	FileMoveDir, %SourceFolder%, %UnUsedImageFolder%\%SourceFolder%
	if (ErrorLevel)
	{
		MsgBox, 48, Move Image File, FileMoveDir Error:`n`nFailed to move '%SourceFolder%' to '%UnUsedImageFolder%\%SourceFolder%'`n`nErrorLevel: '%ErrorLevel%'`nLastError: '%A_LastError%'	
	}
	
	; -- Rename Used Images folder to Images folder --
	else IfNotExist %SourceFolder%
	{
		FileMoveDir, %UsedImageFolder%\%SourceFolder%, %SourceFolder%
		if (ErrorLevel)
		{
			MsgBox, 48, Move Image File, FileMoveDir Error:`n`nFailed to move '%UsedImageFolder%\%SourceFolder%' to '%SourceFolder%'`n`nErrorLevel: '%ErrorLevel%'`nLastError: '%A_LastError%'	
		}
		else
		{
			FileRemoveDir, %UsedImageFolder%, 0					;; Delete Empty Folder
		}
	}
}


