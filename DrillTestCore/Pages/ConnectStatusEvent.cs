namespace DrillTestCore.Pages
{
    public class ConnectStatusEvent
    {
        public string _connectStatus1 { get; set; }
        public string _connectStatus2 { get; set; }
        public ConnectStatusEvent(string connectStatus1, string connectStatus2)
        {
            _connectStatus1 =connectStatus1;
            _connectStatus2 = connectStatus2;
        }
    }
}