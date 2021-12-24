using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

HubConnection connection;

connection = new HubConnectionBuilder()
              .WithUrl("https://toomari.ir/RelayBoardHub")
              .WithAutomaticReconnect()
              .AddMessagePackProtocol()
              .Build();

connection.On<Dictionary<int, bool>>("ReceiveSwitchValues", (data) =>
{
    Console.WriteLine(data);
});

connection.On<int, bool>("ReceiveSwitchCommand", (pin, high) =>
{
});

connection.StartAsync();

connection.SendAsync("SendSwitchCommand", 26, true);


Console.ReadLine();

//connection.SendAsync("SendGetSwitchValuesCommand");