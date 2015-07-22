; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "HSPEXP+"
#define MyAppVersion "1.0.2"
#define MyAppPublisher "AQUA TERRA Consultants"
#define MyAppURL "http://www.aquaterra.com/resources/downloads/HSPEXPplus.php"
#define MyAppExeName "HSPEXP+.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{07C3304A-C9A6-4D30-81C0-004F939FB8C1}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={reg:HKLM\SOFTWARE\AQUA TERRA Consultants\BASINS41,Base Directory|{pf}\HSPEXP+}
DefaultGroupName={#MyAppName}
OutputBaseFilename=HSPEXP+1.02SetUp
Compression=lzma
SolidCompression=yes
InfoBeforeFile=install.txt

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "C:\Dev\BASINS40\HSPEXP\bin\x86\Debug\HSPEXP+.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Dev\BASINS40\HSPEXP\bin\x86\Debug\WinHSPFLt\*"; DestDir: "{app}\WinHSPFLt"; Flags: ignoreversion
Source: "C:\Dev\BASINS40\HSPEXP\bin\x86\Debug\*.dll"; DestDir: "{app}"; Flags: ignoreversion
;Source: "C:\Dev\BASINS40\HSPEXP\bin\x86\Debug\*.eer"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Dev\BASINS40\HSPEXP\*.chm"; DestDir: "{app}"; Flags: ignoreversion
;Source: "C:\Dev\BASINS40\HSPEXP\bin\x86\Debug\hspfmsg.wdm"; DestDir: "{app}"; Flags: ignoreversion
;Source: "C:\Dev\BASINS40\HSPEXP\bin\x86\Debug\ATCoUnits.mdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Dev\BASINS40\HSPEXP\GraphColors.txt"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

