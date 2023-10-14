# vpn-router-win

I have a persistent WireGuard VPN between my computer and an internet server. This applications allows traffic to be routed through the VPN by adjusting the Windows routing table.

## Config

On first run a config file should be created at `%APPDATA%\VpnManager\config.toml`

```toml
vpn_expected_address = "x.x.x.x" # The public ip address when routing through VPN
vpn_if_name = "VpnInterface" # The name of the VPN's network device
vpn_next_hop = "10.0.0.5" # IP address for route's next hop
route_to_change = "0.0.0.0" # IP Address of route
route_mask = "0.0.0.0" # Network Mask for route
route_metric_off = 100 # Cost of using route when VPN routing is off, should be higher than all others
route_metric_on = 10 # Cost of using route when VPN routing is on, should be lower than all others
```

`route_to_change` and `route_mask` should be set to `"0.0.0.0"` to route all IP traffic through the VPN
`route_metric_off` should be higher than your default gateway, `100` should be high enough
`route_metric_on` should be lower than your default gateway, `10` should be low enough

## Errors

**Run as Administrator** otherwise routing will fail

If the config is incorrectly formatted or missing entries, the program will crash. I plan to add more meaningful error messages for invalid configs and other failures in the future.

### Routing Errors

> VPN (Un-)Routing Unsuccessful, failed to apply routing changes!

The Windows API rejected the routing adjustment. There are many reasons for this, incorrect IP address format, invalid network device name, route already exists/does not exist, etc.

> VPN Routing Unsuccessful, new public IP address is incorrect!

Routing has been changed and traffic should be routed through the VPN, but the public IP address is not matching the one specified in the config. You can check your current IP address with `curl ifconfig.me`

> PN Un-Routing Unsuccessful, public IP address remains changed!

Routing has been reverted and traffic should _no longer_ be routed through the VPN, but the public IP address _is still_ matching the one specified in the config. You can check your current IP address with `curl ifconfig.me`
