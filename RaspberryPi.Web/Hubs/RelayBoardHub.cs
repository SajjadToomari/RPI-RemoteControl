using Microsoft.AspNetCore.SignalR;

namespace RaspberryPi.Web.Hubs
{
    public class RelayBoardHub : Hub
    {
        public async Task SendSwitchCommand(int pin, bool high)
        {
            await Clients.All.SendAsync("ReceiveSwitchCommand", pin, high);
        }

        public async Task SendGetSwitchValuesCommand()
        {
            await Clients.All.SendAsync("ReceiveGetSwitchValuesCommand");
        }

        public async Task SendSwitchValues(Dictionary<int, bool> switchValues)
        {
            await Clients.All.SendAsync("ReceiveSwitchValues", switchValues);
        }
    }
}
