#! /usr/bin/env nix-shell
#! nix-shell -i bash -p bash dotnet-sdk

dotnet publish -c Release

# Patch the binary to make it WinExe instead of Console
printf '\x02\x00' | dd of=./bin/win-x64/publish/DesktopManager.exe bs=1 seek=316 count=2 conv=notrunc
