@echo off
echo.
echo SAUVEGARDE VERS svr vm-web-qualif

time /T

SET HOME=c:\Automatismes
cd %HOME%

C:/Automatismes/rsync -rltgodzv --chmod=u+rwx,g+r,Dg+x,F-x --delete-after -e "C:/Automatismes/ssh.exe -i id_rsa" "/cygdrive/c/WAMP/mon-appli/" tguigard@vm-web-qualif.pun.ens-lyon.fr:/home/tguigard/mon-appli/

time /T
