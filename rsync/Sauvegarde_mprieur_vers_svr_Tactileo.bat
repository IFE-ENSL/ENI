@echo off
echo.
echo SAUVEGARDE Skydrive VERS svr tactileo

time /T

SET HOME=c:\sauvegarde-tactileo
cd %HOME%

rsync -rltgodzv --chmod=u+rwx,g+r,Dg+x,F-x --delete-after -e "c:/sauvegarde-tactileo/ssh.exe -i id_rsa" "/cygdrive/c/Users/mprieur/SkyDrive/" mprieur@vm-tactileo.ens-lyon.fr:/var/data/stockage/mprieur/SkyDrive/

time /T
echo SAUVEGARDE Mes Documents VERS svr tactileo

rsync -rltgodzv --chmod=u+rwx,g+r,Dg+x,F-x --delete-after -e "c:/sauvegarde-tactileo/ssh.exe -i id_rsa" "/cygdrive/c/Users/mprieur/Documents/" mprieur@vm-tactileo.ens-lyon.fr:/var/data/stockage/mprieur/Documents/

time /T
