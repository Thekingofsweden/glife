/*
 *		-- Get image paths from glife.txt  --
 *
 *		Requires images subfolder to exist in current directory
 *		
 *		OutAllFile 	- List of All unprocessed file names from glife.txt that are not marked missing in the code
 *		OutUsedFile - List of Found file names from glife.txt and Auto List
 *		OutMissFile	- List of Missing file names from glife.txt that are marked missing in the code.
 *		OutAutoFile	- List of file paths from glife.txt that were added to Output file using code in this script
 *		OutManFile	- List of file paths from glife.txt that will have to be added manualy checked either in glife code or added to OutUsedFile
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


SourceFile 		= glife.txt
OutAllFile		= Image List - All.txt
OutUsedFile		= Image List - Used.txt
OutMissFile		= Image List - Missing.txt
OutAutoFile		= Image Paths - Auto List.txt
OutManFile		= Image Paths - Manual List.txt
SearchRegEx		= i)<img\s+src\s*=\s*"(.*?)"
Search2RegEx	= i)\bview\s*'+(.*?)'+
CommentRegEx 	= ^\s*!
MarkMissRegEx	= i)!.*?:\s+IMAGE NEEDED\s*~\s*.*?<img\s+src\s*=\s*"(.*?)"

FileEncoding, UTF-16

;-- backup files  and delete --
IfNotExist %SourceFile%
{
	MsgBox, 16, Get Image Paths, Unable to locate "%SourceFile%" in "%A_WorkingDir%", 10
	Exit
}

ifExist %OutAllFile%					;; Backup File		
{
	FileMove, %OutAllFile%, %OutAllFile%.bak, 1	
	FileDelete, %OutAllFile%
}

ifExist %OutUsedFile%				;; Backup File		
{
	FileMove, %OutUsedFile%, %OutUsedFile%.bak, 1	
	FileDelete, %OutUsedFile%
}

ifExist %OutMissFile%				;; Backup File		
{
	FileMove, %OutMissFile%, %OutMissFile%.bak, 1	
	FileDelete, %OutMissFile%
}

ifExist %OutAutoFile%				;; Backup File 
{
	FileMove, %OutAutoFile%, %OutAutoFile%.bak, 1
	FileDelete, %OutAutoFile%
}

ifExist %OutManFile%				;; Backup File 
{
	FileMove, %OutManFile%, %OutManFile%.bak, 1
	FileDelete, %OutManFile%
}


; -- load data from file --
FileRead, Source, %SourceFile%

; -- vars need plenty of space to work with --
VarSetCapacity(OutAll, 4194304)				;; 4mb
VarSetCapacity(OutUsed, 4194304)			;; 4mb
VarSetCapacity(OutMiss, 4194304)			;; 4mb
VarSetCapacity(OutAuto, 1048576)			;; 1mb
VarSetCapacity(OutMan, 1048576)				;; 1mb


; -- Parse data one line at a time --
Loop, Parse, Source, `n, `r  			; Specifying `n prior to `r allows both Windows and Unix files to be parsed.
{
	if trim(A_LoopField) = ""													;; Skip blank line
		continue
	
	; -- Check is Comment line --
	FoundCmntPos := RegExMatch(A_LoopField, CommentRegEx)
	if (ErrorLevel)
	{
		MsgBox, 48, Get Image Paths, RegExMatch runtime error: %ErrorLevel%`n`nFound searching string: %A_LoopField%`n`nusing search: %CommentRegEx%
		break
	}
	
	if (FoundCmntPos)															;; comment found
	{
		FoundPos := RegExMatch(A_LoopField, MarkMissRegEx, Match)				;; Check for known missing file
		if (ErrorLevel)
		{
			MsgBox, 48, Get Image Paths, RegExMatch runtime error: %ErrorLevel%`n`nFound searching string: %A_LoopField%`n`nusing search: %MarkMissRegEx%
			break
		}	
		if (FoundPos)															;; Found File
		{	
			Match1 := StrReplace(Match1, "/", "\")								;; Use correct Win path seperator	
			IfExist %Match1%
			{
				OutMiss	.= Spacer(Match1) . "- Missing file exists`n"			;; Missing File found
			}
			else
			{
				OutMiss	.= Match1 . "`n"										;; Missing file Not found
			}		
		}
		Continue
	}
	
	LineNo := A_index															;; Save line no
	
	; -- HTML  images --
	FoundPos := 1
	Haystack := A_LoopField

	Loop, 200
	{
	
		; -- Find Image File Path --
		FoundPos := RegExMatch(Haystack, SearchRegEx, Match, FoundPos)				;; Search for image path in html
		if (ErrorLevel)
		{
			MsgBox, 48, Get Image Paths, RegExMatch runtime error: %ErrorLevel%`n`nFound searching string: %Haystack%`n`nusing search: %SearchRegEx%
			break
		}
		
		if (FoundPos = 0)															;; Not Found exit loop
		{
			break
		}
		else
		; if (FoundPos)																;; Found File
		{	
			IdxTxt := " (L:" . LineNo . ", P:" . FoundPos . ")" 
			Match1 := StrReplace(Match1, "/", "\")									;; Use correct Win path seperator
			OutAll .= Match1 . "`n"
			; -- Image Path Contains Code --
			if inStr(Match1, "<<")													;; String contains expression
			{
				
				if inStr(Match1, "FUNC(''$face_image''")							;; Hairstyle images Function - only use JPG files
				{
					if GetImageFiles("images\body\hairstyles", "hcol*.jpg", OutUsed, "", "FR")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\body\hairstyles\*\hcol*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\body\hairstyles\*\hcol*.jpg' " . IdxTxt . "`n"
					}
					
				}
				else if inStr(Match1, "FUNC(''$clothing_image''")					;; Clothing images Function - only use JPG files
				{
					if GetImageFiles("images\clothes", "vatnik.jpg", OutUsed)
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\clothes\vatnik.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\clothes\vatnik.jpg' " . IdxTxt . "`n"
					}
					; GetImageFiles("images\clothes\newclo", "131.jpg", OutUsed)	;; Dupe See below	
					
					if GetImageFiles("images\clothes", "jeans*.jpg", OutUsed,  "i)jeans\d+\.jpg")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\clothes\jeans*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\clothes\jeans*.jpg' " . IdxTxt . "`n"
					}

					if GetImageFiles("images\clothes", "yoga*.jpg", OutUsed, "i)yoga\d+\.jpg")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\clothes\yoga*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\clothes\yoga*.jpg' " . IdxTxt . "`n"
					}

					if GetImageFiles("images\clothes", "sarafan*.jpg", OutUsed, "i)sarafan\d+\.jpg")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\clothes\sarafan*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\clothes\sarafan*.jpg' " . IdxTxt . "`n"
					}

					if GetImageFiles("images\clothes", "short*.jpg", OutUsed, "i)short\d+\.jpg")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\clothes\short*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\clothes\short*.jpg' " . IdxTxt . "`n"
					}

					if GetImageFiles("images\clothes", "skirt*.jpg", OutUsed, "i)skirt\d+\.jpg")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\clothes\skirt*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\clothes\skirt*.jpg' " . IdxTxt . "`n"
					}

					if GetImageFiles("images\clothes", "dress*.jpg", OutUsed, "i)dress\d+\.jpg")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\clothes\dress*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\clothes\dress*.jpg' " . IdxTxt . "`n"
					}

					if GetImageFiles("images\clothes", "profi*.jpg", OutUsed, "i)profi\d+\.jpg")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\clothes\profi*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\clothes\profi*.jpg' " . IdxTxt . "`n"
					}

					if GetImageFiles("images\clothes", "pants*.jpg", OutUsed "i)pants\d+\.jpg")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\clothes\pants*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\clothes\pants*.jpg' " . IdxTxt . "`n"
					}

					if GetImageFiles("images\clothes", "latex*.jpg", OutUsed, "i)latex\d+\.jpg")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\clothes\latex*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\clothes\latex*.jpg' " . IdxTxt . "`n"
					}

					if GetImageFiles("images\clothes", "hooker*.jpg", OutUsed, "i)hooker\d+\.jpg")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\clothes\hooker*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\clothes\hooker*.jpg' " . IdxTxt . "`n"
					}

					if GetImageFiles("images\clothes", "k*.jpg", OutUsed, "i)k\d+\.jpg")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\clothes\k*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\clothes\k*.jpg' " . IdxTxt . "`n"
					}

					if GetImageFiles("images\clothes\newclo", "*.jpg", OutUsed, "i)\d+\.jpg")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\clothes\newclo\*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\clothes\newclo\*.jpg' " . IdxTxt . "`n"
					}

					if GetImageFiles("images\img\dress", "ero*.jpg", OutUsed, "i)ero\d+\.jpg")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\img\dress\ero*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\img\dress\ero*.jpg' " . IdxTxt . "`n"
					}
				}

				else if inStr(Match1, "images\qwest\card")							;; Playingcard Images  - only use JPG files
				{
					if GetImageFiles("images\qwest\card", "*.jpg", OutUsed, "", "FR")
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\qwest\card\*\*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- No files found in 'images\qwest\card\*\*.jpg' " . IdxTxt . "`n"
					}				
				}
				
				else if inStr(Match1, "images\BDSM_Club\<<$BDSMrole>>") ;; BDSM Club
				{
					BDSMRegEx := "i)(_\d+)\.jpg"
					BDSMPos := RegExMatch(Match1, BDSMRegEx , BDSMatch)
					if (ErrorLevel)
					{
						MsgBox, 48, Get Image Paths, RegExMatch runtime error: %ErrorLevel%`n`nFound searching string: %Match1%`n`nusing search: %BDSMRegEx%
						break
					}			
					
					BDSMRegEx := "i)[rg]?\d+" . BDSMatch1 . "\.jpg"
					
					if GetImageFiles("images\BDSM_Club", "*.jpg", OutUsed, BDSMRegEx)
					{
						OutAuto	.= Spacer(Match1) . "- Found 'images\BDSM_Club*.jpg'`n"
					}
					else
					{
						OutMan	.= Spacer(Match1) . "- no files found in 'images\BDSM_Club*.jpg' " . IdxTxt . "`n"
					}					

				}
				
				else if inStr(Match1, "FUNC")										;; Unknown Function - Manual
				{
					OutMan	.= Spacer(Match1) . "- Unknown Function " . IdxTxt . "`n"
				}
				
				else if inStr(Match1, "$")											;; Uses String variable - Manual
				{
					OutMan	.= Spacer(Match1) . "- Used String Variable " . IdxTxt . "`n"
				}
				
				else if GetImagefromPath(Match1, OutUsed)							;; Try to find images
				{
					OutAuto	.= Spacer(Match1) . "- Found image files`n"
				}
				else
				{
					OutMan	.= Spacer(Match1) . "- Files not found " . IdxTxt . "`n"
				}
			}
			; -- Image path is just a file name --
			else
			{
				IfExist %Match1%
				{
					OutUsed	.= Match1 . "`n"										;; Normal file found
				}
				else
				{
					OutMan	.= Spacer(Match1) . "- File not found " . IdxTxt . "`n"
				}
			}	
			FoundPos += 8 + StrLen(Match1)											;; Increment Search Position
		}
	}
	
	
	; -- VIEW  images --
	FoundPos := 1
	Haystack := A_LoopField

	Loop, 200
	{
	
		; -- Find Image File Path --
		FoundPos := RegExMatch(Haystack, Search2RegEx, Match, FoundPos)				;; Search for 'view' cmd path
		if (ErrorLevel)
		{
			MsgBox, 48, Get Image Paths, RegExMatch runtime error: %ErrorLevel%`n`nFound searching string: %Haystack%`n`nusing search: %Search2RegEx%
			break
		}
		
		if (FoundPos = 0)															;; Not Found exit loop
		{
			break
		}
		else
		; if (FoundPos)																;; Found File
		{	
			IdxTxt := " (L:" . LineNo . ", P:" . FoundPos . ")" 
			Match1 := StrReplace(Match1, "/", "\")									;; Use correct Win path seperator
			OutAll .= Match1 . "`n"
			; -- Image Path Contains Code --
			if inStr(Match1, "<<")													;; String contains expression
			{
				if inStr(Match1, "FUNC")											;; Unknown Function - Manual
				{
					OutMan	.= Spacer(Match1) . "- Unknown Function " . IdxTxt . "`n"
				}
				
				else if inStr(Match1, "$")											;; Uses String variable - Manual
				{
					OutMan	.= Spacer(Match1) . "- Used String Variable " . IdxTxt . "`n"
				}
				
				else if GetImagefromPath(Match1, OutUsed)							;; Try to find images
				{
					OutAuto	.= Spacer(Match1) . "- Found image files`n"
				}
				else
				{
					OutMan	.= Spacer(Match1) . "- Files not found " . IdxTxt . "`n"
				}
			}
			
			; -- Image path is just a file name --
			else
			{
				IfExist %Match1%
				{
					OutUsed	.= Match1 . "`n"										;; Normal file found
				}
				else
				{
					OutMan	.= Spacer(Match1) . "- File not found " . IdxTxt . "`n"
				}
			}
			FoundPos += 5 + StrLen(Match1)											;; Increment Search Position
		}
	}
}


; -- Set path to use / seperators --
OutAll	:= StrReplace(OutAll, "\", "/")
OutUsed	:= StrReplace(OutUsed, "\", "/")
OutMiss	:= StrReplace(OutMiss, "\", "/")
OutAuto	:= StrReplace(OutAuto, "\", "/")
OutMan	:= StrReplace(OutMan, "\", "/")


; -- sort filenames and remove dupes --
Sort, OutAll, U
Sort, OutUsed, U
Sort, OutMiss, U
Sort, OutAuto, U
Sort, OutMan, U


;-- save files --
FileAppend, %OutAll%, %OutAllFile%
FileAppend, %OutUsed%, %OutUsedFile%
FileAppend, %OutMiss%, %OutMissFile%
FileAppend, %OutAuto%, %OutAutoFile%
FileAppend, %OutMan%, %OutManFile%

Exit

; ########## Functions ##########
Spacer(TxtStr = "")
{
	Static SpaceFill := "                                                                 "
	
	;~ ps := strlen(TxtStr) + 1												;; allow empty string = 1 
	;~ ss := TxtStr . SubStr(SpaceFill, ps) . "`t"
	
	return TxtStr . SubStr(SpaceFill, strlen(TxtStr)+1) . "`t"
	
}

GetImageFiles(FilePath, FileName, ByRef Output, FileNameRegEx := "", FileLoopMode := "F")
{	;; Get image paths in filepath using filepattern FileName
	
	;~ msgbox 	GetImageFiles()`nFilePath := %FilePath%`nFileName := %FileName%`nFileNameRegEx := %FileNameRegEx%`nFileLoopMode := %FileLoopMode%
	
	IfNotExist, %FilePath%
	{
		MsgBox, 48, Get Image Paths, Error in GetImageFiles()`n`nUnable to locate "%FilePath%" folder, Filename '%FileName%'.`n`nCurrent working dir is "%A_WorkingDir%"
		return false		
	}
	
	SaveWorkDir := A_WorkingDir
	Setworkingdir, %FilePath%
	
	found := false
	
	; -- Loop with or Without recursion --
	Loop, Files, %FileName%, %FileLoopMode%										;; Find files in FilePath using Filepattern FileName
	{
		if(FileNameRegEx)														;; Can we test file name
		{
			FndPos := RegExMatch(A_LoopFileName, FileNameRegEx)					;; File Found matches
			if (ErrorLevel)
			{
				MsgBox, 48, Get Image Paths, RegExMatch Error in function GetImageFiles()`n`nruntime error: %ErrorLevel%`n`nFound searching string: %A_LoopFileName%`n`nusing search: %FileNameRegEx%
				break
			}
			if (not FndPos)														;; Filename not matches skip
			{
				continue
			}
		}
	fp := FilePath  . "\" . A_LoopFileFullPath
	Output .= fp . "`n"
	found := true
	}

	
	Setworkingdir, %SaveWorkDir%
	return found

}

GetImagefromPath(FilePath, ByRef Output)
{	;; Try to get Images from path 

	FindRegEx 	:= "<<.*?>>"
	ReplaceStr	:=	"*"

	fp			:= StrReplace(FilePath, "/", "\") 
	fp 			:= RegExReplace(fp, FindRegEx, ReplaceStr)
	
	splitpath, fp, FileName, Dir, Ext

	;; -- Manual if wildcard in path --
	if inStr(Dir, "*")
	{
		; MsgBox, 64, Get Image Path, GetImageFromPath() error: Path contains wildcard`n `nPath : '%fp%'`nDir :  '%Dir%'
		return false
	}

	FileNameRegEx := StrReplace(FileName, "*", "\d+") 					; set up test for filename wildcard to be numbers only
	FileNameRegEx := "i)" . StrReplace(FileNameRegEx, ".", "\.") 
	
	return GetImageFiles(Dir, FileName, Output, FileNameRegEx)
}