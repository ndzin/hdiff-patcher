@echo off
python -m PyInstaller --onefile main.py --name "hdiff-patcher" --icon icon-grab.exe --uac-admin
pause