chcp 1251

@echo off
setlocal

call settings.bat

echo -------------------------------
echo:
echo Project:%projectName%
echo Configuration:%configuration%
echo PackageName:%packageName%
echo PackageVersion:%packageVersion%
echo:
echo -------------------------------

rem Упаковка проекта в пакет
cd ..\src\%projectName%
call dotnet pack -c %configuration%

rem Отправка пакета в репозиторий
cd bin\%configuration%
call dotnet nuget push %packageName%.%packageVersion%.nupkg -s https://api.nuget.org/v3/index.json -k {your_api_key}
pause