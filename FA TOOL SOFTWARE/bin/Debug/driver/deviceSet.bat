@echo off

devcon.exe remove "USB\VID_2047&PID_0301"
echo errorlevel = %errorlevel%
rem (errorlevel 返回值必須按照從大到小的順序排列)
rem (errorlevel 返回值大於等於 1 以上就算fail)
rem (errorlevel 返回值等於 0 : Pass)
IF ERRORLEVEL 1 goto rescan
IF ERRORLEVEL 0 goto rescan
goto rescan



rem ==============================================================================
:rescan
devcon.exe rescan
echo errorlevel = %errorlevel%


:end
pause