; Inno Setup script for a simple per-user ApneScan installer.
; Version and source folder are passed on the ISCC command line via /DAppVer and /DSourceDir.

[Setup]
AppId={{9E9C2B70-4E1E-4E2A-9E1B-2C7A5F0B1D34}
AppName=ApneScan
AppVersion={#AppVer}
AppPublisher=ApneScan
DefaultDirName={autopf}\ApneScan
DefaultGroupName=ApneScan
DisableProgramGroupPage=yes
DisableDirPage=auto
UninstallDisplayIcon={app}\ApneScan.exe
OutputBaseFilename=ApneScan-Setup
Compression=lzma2/max
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=lowest
WizardStyle=modern

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion

[Icons]
Name: "{group}\ApneScan"; Filename: "{app}\ApneScan.exe"
Name: "{group}\Uninstall ApneScan"; Filename: "{uninstallexe}"
Name: "{autodesktop}\ApneScan"; Filename: "{app}\ApneScan.exe"

[Run]
Filename: "{app}\ApneScan.exe"; Description: "Launch ApneScan"; Flags: nowait postinstall skipifsilent
