for /f %%i in ('npm version %1 --no-git-tag-version') do set VERSION=%%i
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& '.\scripts\ps\set-assembly-version.ps1' %VERSION%"
git commit -a -m "Release %VERSION%" && git tag -a -m "Release %VERSION%" %VERSION%
