@echo off
c:
cd\
IF NOT EXIST C:\ptest md ptest 
cd ptest
echo "Anwendung wird aktualisiert - Bitte kurz warten!"
xcopy O:\UMWELT-PARADIGMA\div\deployxc\bin\debug\*.* c:\ptest\*.* /d /Y >NUL 

echo %USERNAME% > O:\UMWELT-PARADIGMA\div\deployxc\bin\debug\updates.txt
rem pause
start  c:\ptest\paradigma.exe
 