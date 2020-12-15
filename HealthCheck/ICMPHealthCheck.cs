using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheck
{
    public class ICMPHealthCheck : IHealthCheck
    {
        //private string Host = "www.does-not-exist.com";
        //private int Timeout = 300;

        private string Host { get; set; }
        private int Timeout { get; set; }

        public ICMPHealthCheck(string host, int timeout)
        {
            Host = host;
            Timeout = timeout;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using(var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync(Host);
                    switch(reply.Status)
                    {
                        case IPStatus.Success:
                            var msg = $"ICMP to {Host} took {Timeout} ms.";
                            return (reply.RoundtripTime > Timeout) ? HealthCheckResult.Degraded(msg) : HealthCheckResult.Healthy(msg);
                        default:
                            var error = $"ICMP to {Host} failed: {reply.Status}";
                            return HealthCheckResult.Unhealthy(error);
                    }
                }
            }
            catch(Exception e)
            {
                var error = $"ICMP to {Host} failed: {e.Message}";
                return HealthCheckResult.Unhealthy(error);
            }
        }
    }
}
