using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

HubConnection connection;

connection = new HubConnectionBuilder()
              .WithUrl("https://localhost:7247/RelayBoardHub")
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

var dictionary = new Dictionary<int, bool>(3);

dictionary.Add(26, false);
dictionary.Add(20, false);
dictionary.Add(21, true);

await connection.SendAsync("SendSwitchValues", dictionary);


Console.ReadLine();

//connection.SendAsync("SendGetSwitchValuesCommand");