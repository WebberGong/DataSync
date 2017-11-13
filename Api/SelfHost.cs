using System;
using Nancy.Hosting.Self;

namespace Api
{
    public class SelfHost
    {
        private static readonly NancyHost Nancy;

        static SelfHost()
        {
            var configuration = new HostConfiguration
            {
                UrlReservations = new UrlReservations {CreateAutomatically = true}
            };
            var uri = new Uri($"{Settings.ApiBaseUrl}");
            Nancy = new NancyHost(configuration, uri);
        }

        public static void Start()
        {
            Nancy.Start();
        }

        public static void Stop()
        {
            Nancy.Stop();
        }
    }
}