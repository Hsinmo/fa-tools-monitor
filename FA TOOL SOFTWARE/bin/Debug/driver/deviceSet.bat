@echo off

devcon.exe remove "USB\VID_2047&PID_0301"
echo errorlevel = %errorlevel%
rem (errorlevel ��^�ȥ������ӱq�j��p�����ǱƦC)
rem (errorlevel ��^�Ȥj�󵥩� 1 �H�W�N��fail)
rem (errorlevel ��^�ȵ��� 0 : Pass)
IF ERRORLEVEL 1 goto rescan
IF ERRORLEVEL 0 goto rescan
goto rescan



rem ==============================================================================
:rescan
devcon.exe rescan
echo errorlevel = %errorlevel%


:end
pause