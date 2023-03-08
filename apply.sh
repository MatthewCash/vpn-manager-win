#! /usr/bin/env nix-shell
#! nix-shell -i bash -p bash openssh coreutils rsync

# Stop current DesktopManager task (so files can be copied)
ssh host "schtasks /End /TN \"Vpn Manager\""
sleep 1

# Copy changed files
rsync -a ./bin/win-x64/publish/* /mnt/win/Users/Matthew/bin/VpnManager/

# Start task
ssh host "schtasks /Run /TN \"Vpn Manager\""
