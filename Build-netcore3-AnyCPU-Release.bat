@echo off
@REM call %comspec% /k "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\vcvars64.bat"
CALL "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\vcvars64.bat"
dotnet build "ide\\ds2wav.sln" "/t:_netcore\ds2wav" "/p:Platform=Any CPU;Configuration=Release"
pause
