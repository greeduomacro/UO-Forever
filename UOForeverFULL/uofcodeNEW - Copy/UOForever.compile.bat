@SET CURPATH=%~dp0
@SET CSCPATH=%windir%\Microsoft.NET\Framework\v4.0.30319\

@SET SRVPATH=%CURPATH%Core\
@SET SCRPATH=%CURPATH%Shard\Scripts\

@TITLE: UOForever - http://www.uoforever.com


::##########


@ECHO:
@ECHO: Step 1 - Compile UOForever
@ECHO:

@PAUSE

@DEL "%CURPATH%Shard\UOForever.exe"

@ECHO ON

%CSCPATH%csc.exe /win32icon:"%SRVPATH%runuo.ico" /target:exe /out:"%CURPATH%Shard\UOForever.exe" /recurse:"%SRVPATH%*.cs" /d:Framework_4_0 /nowarn:0618 /nologo /optimize /unsafe

@ECHO OFF

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS


::##########


@ECHO:
@ECHO: Step 2 - Compile Scripts
@ECHO:

@PAUSE

@DEL "%SCRPATH%Output\Scripts.CS.dll"

@ECHO ON

%CSCPATH%csc.exe /r:"%CURPATH%Shard\UOForever.exe" /target:library /out:"%SCRPATH%Output\Scripts.CS.dll" /recurse:"%SCRPATH%*.cs" /d:Framework_4_0 /nowarn:0618 /nologo /optimize /unsafe

@ECHO OFF

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS


::##########


@ECHO:
@ECHO: Step 3 - Launch UOForever
@ECHO:

@PAUSE

@CLS

@ECHO OFF

%CURPATH%Shard\UOForever.exe
