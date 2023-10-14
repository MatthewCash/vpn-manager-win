using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

static class RoutingManager {
    struct Route {
        public uint dwForwardDest;
        public uint dwForwardMask;
        public uint dwForwardPolicy;
        public uint dwForwardNextHop;
        public uint dwForwardIfIndex;
        public uint dwForwardType;
        public uint dwForwardProto;
        public uint dwForwardAge;
        public uint dwForwardNextHopAS;
        public uint dwForwardMetric1;
        public uint dwForwardMetric2;
        public uint dwForwardMetric3;
        public uint dwForwardMetric4;
        public uint dwForwardMetric5;
    };

    [DllImport("iphlpapi", CharSet = CharSet.Auto)]
    static extern int CreateIpForwardEntry(ref Route route);

    [DllImport("iphlpapi", CharSet = CharSet.Auto)]
    static extern int DeleteIpForwardEntry(ref Route route);

    public static uint GetInterfaceIndex(string name) {
        var interfaces = NetworkInterface.GetAllNetworkInterfaces();

        return (uint) interfaces.Single(iface => iface.Name == name)
            .GetIPProperties().GetIPv4Properties().Index;
    }

    public static bool AddRoute(IPAddress dest, IPAddress mask, IPAddress nextHop, uint ifIndex, uint metric) {
        return ModifyRoute(dest, mask, nextHop, ifIndex, metric, true);
    }

    public static bool DeleteRoute(IPAddress dest, IPAddress mask, IPAddress nextHop, uint ifIndex, uint metric) {
        return ModifyRoute(dest, mask, nextHop, ifIndex, metric, false);
    }

    static bool ModifyRoute(IPAddress dest, IPAddress mask, IPAddress nextHop, uint ifIndex, uint metric, bool add = true) {
        Route route = new() {
            dwForwardDest = BitConverter.ToUInt32(dest.GetAddressBytes(), 0),
            dwForwardMask = BitConverter.ToUInt32(mask.GetAddressBytes(), 0),
            dwForwardNextHop = BitConverter.ToUInt32(nextHop.GetAddressBytes(), 0),
            dwForwardMetric1 = metric,
            dwForwardType = 3,
            dwForwardProto = 3,
            dwForwardAge = 0,
            dwForwardIfIndex = ifIndex
        };

        return (add ? CreateIpForwardEntry(ref route) : DeleteIpForwardEntry(ref route)) == 0;
    }
}
