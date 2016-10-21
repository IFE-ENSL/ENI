echo.
echo SAUVEGARDE VERS svr tactileo

time /T

rsync -rltgodzv --chmod=u+rwx,g+r,Dg+x,F-x --delete-after -e "ssh.exe -i ~/.ssh/id_rsa" "/Users/taimpaperez/doctorat/" tperez@vm-tactileo.ens-lyon.fr:/var/data/stockage/tperez/doctorat/

time /T
