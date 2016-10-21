@echo off
echo.
echo SAUVEGARDE VERS svr tactileo

time /T

SET HOME=c:\sauvegarde-tactileo
cd %HOME%

rsync -rltgodzv --chmod=u+rwx,g+r,Dg+x,F-x --delete-after -e "c:/sauvegarde-tactileo/ssh.exe -i id_rsa" "/cygdrive/c/Users/rmonodan/Documents/" rmonod@vm-tactileo.ens-lyon.fr:/var/data/stockage/rmonod/Documents/ > Sauvegarde.txt
rsync -rltgodzv --chmod=u+rwx,g+r,Dg+x,F-x --delete-after -e "c:/sauvegarde-tactileo/ssh.exe -i id_rsa" "/cygdrive/c/Users/rmonodan/Desktop/" rmonod@vm-tactileo.ens-lyon.fr:/var/data/stockage/rmonod/Desktop/ >> Sauvegarde.txt
rsync -rltgodzv --chmod=u+rwx,g+r,Dg+x,F-x --delete-after -e "c:/sauvegarde-tactileo/ssh.exe -i id_rsa" "/cygdrive/c/Users/rmonodan/Pictures/" rmonod@vm-tactileo.ens-lyon.fr:/var/data/stockage/rmonod/Pictures/ >> Sauvegarde.txt
rsync -rltgodzv --chmod=u+rwx,g+r,Dg+x,F-x --delete-after -e "c:/sauvegarde-tactileo/ssh.exe -i id_rsa" "/cygdrive/c/Users/rmonodan/Videos/" rmonod@vm-tactileo.ens-lyon.fr:/var/data/stockage/rmonod/Videos/ >> Sauvegarde.txt
rsync -rltgodzv --chmod=u+rwx,g+r,Dg+x,F-x --delete-after -e "c:/sauvegarde-tactileo/ssh.exe -i id_rsa" "/cygdrive/c/Users/rmonodan/Phylogene" rmonod@vm-tactileo.ens-lyon.fr:/var/data/stockage/rmonod/ >> Sauvegarde.txt
rsync -rltgodzv --chmod=u+rwx,g+r,Dg+x,F-x --delete-after -e "c:/sauvegarde-tactileo/ssh.exe -i id_rsa" "/cygdrive/c/Users/rmonodan/reaction*" rmonod@vm-tactileo.ens-lyon.fr:/var/data/stockage/rmonod/ >> Sauvegarde.txt

rsync -rltgodzv --chmod=u+rwx,g+r,Dg+x,F-x --delete-after -e "c:/sauvegarde-tactileo/ssh.exe -i id_rsa" "/cygdrive/c/Users/rmonodan/Dropbox/" rmonod@vm-tactileo.ens-lyon.fr:/var/data/stockage/rmonod/Dropbox/ >> Sauvegarde.txt
rsync -rltgodzv --chmod=u+rwx,g+r,Dg+x,F-x --delete-after -e "c:/sauvegarde-tactileo/ssh.exe -i id_rsa" "/cygdrive/c/Users/rmonodan/Google Drive/" rmonod@vm-tactileo.ens-lyon.fr:/var/data/stockage/rmonod/Google Drive/ >> Sauvegarde.txt


time /T
