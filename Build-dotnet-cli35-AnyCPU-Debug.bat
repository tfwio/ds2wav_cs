@echo off
:: set msbuild_path=C:\Program Files (x86)\msbuild\14.0\bin
:: set msbuild_path=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin
set msbuild_path=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin
set PATH=%PATH%;%msbuild_path%
msbuild /m "ide\\ds2wav.sln" "/t:_netfx\ds2wav" "/p:Platform=Any CPU;Configuration=Debug"
pause
