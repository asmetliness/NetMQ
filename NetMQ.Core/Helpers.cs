namespace NetMQ.Core
{
    public static class Helpers
    {
        private const string TcpPrefix = "tcp://";
        
        public static string ConvertIp(string ip)
        {
            return $@"{TcpPrefix}{ip}";
        }
    }
}