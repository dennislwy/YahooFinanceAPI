@echo off
SET COMPILER_VER=2.3.2
SET CSCPATH=%TEMP%\Microsoft.Net.Compilers.%COMPILER_VER%\tools
if not exist "%TEMP%\nuget.exe" powershell -Command "(new-object System.Net.WebClient).DownloadFile('https://dist.nuget.org/win-x86-commandline/latest/nuget.exe', '%TEMP%\nuget.exe')"
%TEMP%\nuget.exe install YahooFinanceAPI\packages.config -o packages
%TEMP%\nuget.exe install Microsoft.Net.Compilers -version %COMPILER_VER% -o %TEMP%
if not exist ".\bin" mkdir bin
%CSCPATH%\csc /target:library /out:bin\YahooFinanceAPI.dll /recurse:YahooFinanceAPI\*.cs /doc:bin\YahooFinanceAPI.xml
pause
