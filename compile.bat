@echo off
python -m PyInstaller --onefile main.py --name "hdiff-patcher" --icon GenshinImpact.exe --uac-admin
pause