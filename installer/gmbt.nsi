SetCompressor /solid lzma 

!include utils.nsi

!include ..\.ver

XPStyle on

RequestExecutionLevel user

PageEx license
  LicenseData "..\EULA"
  LicenseForceSelection checkbox
PageExEnd

Page license
Page instfiles

LoadLanguageFile "${NSISDIR}\Contrib\Language files\English.nlf"
LoadLanguageFile "${NSISDIR}\Contrib\Language files\Polish.nlf"

LicenseLangString myLicenseData ${LANG_ENGLISH} "..\LICENSE"
LicenseLangString myLicenseData ${LANG_POLISH} "..\LICENSE"

LicenseData $(myLicenseData)

LangString Name ${LANG_POLISH} "Polski"
LangString Name ${LANG_ENGLISH} "English"

Name "Gothic Mod Build Tool"
OutFile "gmbt-${GMBT_VERSION}.exe"
InstallDir $APPDATA\GMBT
InstallDirRegKey HKCU "Software\GMBT" "Install_Dir"

!define APP_NAME "Gothic Mod Build Tool" 
!define APP_COPY "Copyright © 2018 Szymon 'Szmyk' Zak" 
!define APP_COMP "Szymon 'Szmyk' Zak"                 

VIProductVersion ${GMBT_VERSION_FULL}
VIAddVersionKey "CompanyName"      "${APP_COMP}"
VIAddVersionKey "FileVersion"      "${VER_TEXT}"
VIAddVersionKey "LegalCopyright"   "${APP_COPY}"
VIAddVersionKey "FileDescription"  "${APP_NAME}"

Function .onInit
  Call IsSilent
  Pop $0
  StrCmp $0 1 0 +2
	goto end
	
	Push ""
	Push ${LANG_POLISH}
	Push Polski
	Push ${LANG_ENGLISH}
	Push English
	Push A
	LangDLL::LangDialog "Installer Language" "Please select the language of the installer"

	Pop $LANGUAGE
	StrCmp $LANGUAGE "cancel" 0 +2
		Abort
		
	end:
FunctionEnd

Section "" 
  SetOutPath "$INSTDIR"
  File "..\README.md"
  File "..\ThirdPartyNotices.md"
  File "..\CHANGELOG.md"
  File "..\LICENSE"
 
  SetOutPath "$INSTDIR\lang"
  File "..\lang\*.json"
  
  SetOutPath "$INSTDIR\bin"
  File "..\src\gmbt\bin\Release\*.exe"
  File "..\src\gmbt\bin\Release\*.dll"
  
  SetOutPath "$INSTDIR\tools"
  File "..\tools\GothicVDFS 2.6\GothicVDFS.exe"
  File  /oname=GothicVDFS_ReadMe.txt "..\tools\GothicVDFS 2.6\ReadMe.txt"
  
  WriteRegStr HKCU SOFTWARE\GMBT "Install_Dir" "$INSTDIR"
   
  Push "$INSTDIR\bin"
  Call AddToPath

  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GMBT" "DisplayName" "Gothic Mod Build Tool"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GMBT" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GMBT" "NoModify" 1
  WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GMBT" "NoRepair" 1
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GMBT" "Readme" "https://github.com/Szmyk/gmbt#gothic-mod-build-tool"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GMBT" "Publisher" "Szymon 'Szmyk' Zak"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GMBT" "DisplayVersion" "${GMBT_VERSION}"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GMBT" "Comments" "Gothic Mod Build Tool is a simple tool designed to help in testing and building Gothic and Gothic 2 Night of the Raven mods."
  WriteUninstaller "uninstall.exe"
	
SectionEnd

Section "Uninstall"
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GMBT"
  DeleteRegKey HKCU "Software\GMBT"

  Delete $INSTDIR\uninstall.exe
  
  Push "$INSTDIR\bin"
  Call un.RemoveFromPath

  Delete $INSTDIR\bin\*
  Delete $INSTDIR\tools\*
  Delete $INSTDIR\updates\*
  Delete $INSTDIR\lang\*
  Delete $INSTDIR\*
  
  RMDir /R "$INSTDIR\bin"
  RMDir /R "$INSTDIR\tools"
  RMDir /R "$INSTDIR\updates"
  RMDir /R "$INSTDIR\lang"

SectionEnd
