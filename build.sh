#! /usr/bin/env nix-shell
#! nix-shell -i bash -p bash dotnet-sdk

dotnet publish -c Release

# Patch the binary to make it WinExe instead of Console
printf '\x67\x64\x02\x00\x02' | dd of=./bin/win-x64/publish/VpnManager.exe bs=1 seek=320 count=5 conv=notrunc
