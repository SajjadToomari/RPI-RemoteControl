using Microsoft.AspNetCore.SignalR;

namespace RaspberryPi.Web.Hubs
{
    public class RelayBoardHub : Hub
    {
        private readonly ILogger<RelayBoardHub> _logger;
        public RelayBoardHub(ILogger<RelayBoardHub> logger) => _logger = logger;
        public async Task SendSwitchCommand(int pin, bool high)
        {
            _logger.LogInformation("SendSwitchCommand at: {time}", DateTimeOffset.Now);
            await Clients.All.SendAsync("ReceiveSwitchCommand", pin, high);
            _logger.LogInformation("SendSwitchCommand end at: {time}", DateTimeOffset.Now);
        }

        public async Task SendGetSwitchValuesCommand()
        {
            _logger.LogInformation("SendGetSwitchValuesCommand at: {time}", DateTimeOffset.Now);
            await Clients.All.SendAsync("ReceiveGetSwitchValuesCommand");
            _logger.LogInformation("SendGetSwitchValuesCommand end at: {time}", DateTimeOffset.Now);
        }

        public async Task SendSwitchValues(Dictionary<int, bool> switchValues)
        {
            _logger.LogInformation("SendGetSwitchValuesCommand at: {time}", DateTimeOffset.Now);
            await Clients.All.SendAsync("ReceiveSwitchValues", switchValues);
            _logger.LogInformation("SendGetSwitchValuesCommand end at: {time}", DateTimeOffset.Now);
        }
    }
}
