SetCompressor lzma

!include utils.nsi

!include ..\.ver

;--------------------------------

Function .onInit
  MessageBox MB_YESNO|MB_ICONQUESTION "Would you like to install/update Gothic Mod Build Tool v${GMBT_VERSION}?" \
    /SD IDYES IDNO no IDYES yes

  yes:
    SetSilent silent
    Goto done
  no:
    Abort
  done:
FunctionEnd

;--------------------------------

Name "Gothic Mod Build Tool"
OutFile "gmbt-${GMBT_VERSION}.exe"
InstallDir $APPDATA\GMBT
InstallDirRegKey HKCU "Software\GMBT" "Install_Dir"
RequestExecutionLevel admin

;--------------------------------
; Pages
;--------------------------------

Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------
; Installer
;--------------------------------

!define APP_NAME "Gothic Mod Build Tool" 
!define APP_COPY "Copyright © 2018 Szymon 'Szmyk' Zak" 
!define APP_COMP "Szymon 'Szmyk' Zak"                 

VIProductVersion ${GMBT_VERSION_FULL}
VIAddVersionKey "CompanyName"      "${APP_COMP}"
VIAddVersionKey "FileVersion"      "${VER_TEXT}"
VIAddVersionKey "LegalCopyright"   "${APP_COPY}"
VIAddVersionKey "FileDescription"  "${APP_NAME}"

Section "" 

  SetOutPath "$INSTDIR\bin"
  File "..\src\gmbt\bin\Release\*.exe"
  File "..\src\gmbt\bin\Release\*.dll"
  
  SetOutPath "$INSTDIR\tools"
  File "..\src\gmbt\Resources\DDS2ZTEX Converter\dds2ztex.exe"
  File "..\src\gmbt\Resources\GothicVDFS 2.6\GothicVDFS.exe"
  File "..\src\gmbt\Resources\nvDXT\nvdxt.exe"
  File "..\src\gmbt\Resources\zSpy 2.05\zSpy.exe"
  File "..\src\gmbt\Resources\zSpy 2.05\zSPYdefault.cfg"
  
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
     
  Call IsSilent
  Pop $0
  StrCmp $0 1 0 +2
    Abort
	
  MessageBox MB_OK "Done."
  
  ${OpenURL} "https://github.com/Szmyk/gmbt#gothic-mod-build-tool"
	
SectionEnd

;--------------------------------
; Uninstaller
;--------------------------------

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GMBT"
  DeleteRegKey HKCU "Software\GMBT"

  Delete $INSTDIR\uninstall.exe
  
  Push "$INSTDIR\bin"
  Call un.RemoveFromPath

  Delete $INSTDIR\bin\*
  Delete $INSTDIR\tools\*
  
  RMDir /R "$INSTDIR\bin"
  RMDir /R "$INSTDIR\tools"

SectionEnd
