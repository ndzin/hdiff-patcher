@echo off
python -m PyInstaller --onefile main.py --name "hdiff-patcher" --icon icon-grab.exe --uac-admin

move "dist\hdiff-patcher.exe" .

rmdir /s /q "./dist"
rmdir /s /q "./build"
del "hdiff-patcher.spec"

pause