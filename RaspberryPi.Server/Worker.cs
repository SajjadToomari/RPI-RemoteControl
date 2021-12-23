using Microsoft.AspNetCore.SignalR.Client;
using System.Device.Gpio;

namespace RaspberryPi.Server
{
    public class Worker : BackgroundService
    {
        private Timer _timer;
        private object _lock = new object();
        private readonly ILogger<Worker> _logger;
        private GpioController _controller = new ();

    public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            HubConnection connection;

            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:53353/RelayBoardHub")
                .WithAutomaticReconnect()
                .AddMessagePackProtocol()
                .Build();

            connection.On<int, bool>("ReceiveSwitchCommand", (pin, high) =>
            {
                lock (_lock)
                {
                    _controller.OpenPin(pin, PinMode.Output);
                    _controller.Write(pin, high ? PinValue.High : PinValue.Low);
                    _controller.ClosePin(pin);
                }
            });

            connection.On("ReceiveGetSwitchValuesCommand", () =>
            {
                lock (_lock)
                {
                    var value37 = _controller.Read(37);
                    var value38 = _controller.Read(38);
                    var value40 = _controller.Read(40);

                    var dictionary = new Dictionary<int, bool>(3)
                    {
                        { 37, value37 == PinValue.High ? true : false },
                        { 38, value38 == PinValue.High ? true : false },
                        { 40, value40 == PinValue.High ? true : false }
                    };

                    connection.SendAsync("SendSwitchValues", dictionary);
                }
            });

            await connection.StartAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }

            _controller.Dispose();

            await connection.StopAsync();

            await connection.DisposeAsync();
        }
    }
}