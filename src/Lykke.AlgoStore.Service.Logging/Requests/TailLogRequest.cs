namespace Lykke.AlgoStore.Service.Logging.Requests
{
    public class TailLogRequest
    {
        public string InstanceId { get; set; }
        public int Tail { get; set; }
    }
}
