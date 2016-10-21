@echo off
echo.
echo SAUVEGARDE VERS svr vm-web-qualif

time /T

SET HOME=c:\ENI\rsync
cd %HOME%

C:/ENI/rsync/rsync.exe -rltgodzv --chmod=u+rwx,g+r,Dg+x,F-x --delete-after -e "C:/ENI/rsync/ssh.exe" "/cygdrive/c/wamp/www/eni" jnagy01@vm-web-qualif.pun.ens-lyon.fr:/home/jnagy01/wamp/
C:/ENI/rsync/rsync.exe -rltgodzv --chmod=u+rwx,g+r,Dg+x,F-x --delete-after --exclude=Library -e "C:/ENI/rsync/ssh.exe" "/cygdrive/c/ENI" jnagy01@vm-web-qualif.pun.ens-lyon.fr:/home/jnagy01/ENI-UNITY/
time /T
