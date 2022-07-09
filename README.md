# vpn-router-win

I have a persistent WireGuard VPN between my computer and an internet server. This applications allows traffic to be routed through the VPN by adjusting the Windows routing table.

## Config 

A config file should be created at `%APPDATA%\VpnManager\config.toml`

```toml
vpn_expected_address = "x.x.x.x" # The public ip address when routing through VPN
vpn_net_device_name = "VpnDevice" # The name of the VPN's network device
routes_to_change = ["0.0.0.0/1", "128.0.0.0/1"] # Network ranges to override
```

This example config would generate and run two commands:

```
netsh.exe interface ipv4 {add/delete} route 0.0.0.0/1 VpnDevice 0.0.0.0
netsh.exe interface ipv4 {add/delete} route 128.0.0.0/1 VpnDevice 0.0.0.0
```

## Errors

**Run as Administrator** otherwise routing will fail

If the config is incorrectly formatted or missing entries, the program will crash. In the future I will add more meaningful error messages for invalid configs.

### Routing Errors

> VPN (Un-)Routing Unsuccessful, failed to apply routing changes!

One or more of the `netsh.exe` commands (example above) failed (exit code != 0). There are many reasons for this, incorrect IP address format, invalid network device name, etc.

> VPN Routing Unsuccessful, new public IP address is incorrect!

All applicable `netsh.exe` commands exited successfully and traffic should be routed through the VPN, but the public IP address is not matching the one specified in the config. You can check your current IP address with `curl ifconfig.me`

> PN Un-Routing Unsuccessful, public IP address remains changed!

All applicable `netsh.exe` commands exited successfully and traffic should *no longer* be routed through the VPN, but the public IP address *is still* matching the one specified in the config. You can check your current IP address with `curl ifconfig.me`