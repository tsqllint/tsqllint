npm version arg0 > temp.txt
set /p VER=<temp.txt
set VER=%VER:~1%
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& '.\scripts\ps\set-assembly-version.ps1' %VER"