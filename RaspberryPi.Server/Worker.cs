using Microsoft.AspNetCore.SignalR.Client;
using System.Device.Gpio;

namespace RaspberryPi.Server
{
    public class Worker : BackgroundService
    {
        private readonly object _lock = new object();
        private readonly ILogger<Worker> _logger;
        private readonly GpioController _controller = new();

        public Worker(ILogger<Worker> logger) => _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            HubConnection connection;

            connection = new HubConnectionBuilder()
                .WithUrl("https://toomari.ir/RelayBoardHub")
                .WithAutomaticReconnect()
                .AddMessagePackProtocol()
                .Build();

            connection.Closed += (ex) =>
            {
                _logger.LogInformation($"Connection Closed error : {ex.Message};;{ex.InnerException?.Message} at: {DateTimeOffset.Now}");
                return Task.CompletedTask;
            };

            connection.Reconnecting += (ex) =>
            {
                _logger.LogInformation($"Reconnecting detail : {ex.Message};;{ex.InnerException?.Message} at: {DateTimeOffset.Now}");
                return Task.CompletedTask;
            };

            connection.Reconnected += (ex) =>
            {
                _logger.LogInformation($"Reconnected detail : {ex} at: {DateTimeOffset.Now}");
                return Task.CompletedTask;
            };

            connection.On<int, bool>("ReceiveSwitchCommand", async (pin, high) =>
            {
                try
                {
                    _logger.LogInformation("ReceiveSwitchCommand at: {time}", DateTimeOffset.Now);

                    lock (_lock)
                    {
                        _controller.OpenPin(pin, PinMode.Output);
                        _controller.Write(pin, high ? PinValue.High : PinValue.Low);
                        _controller.ClosePin(pin);
                    }

                    await Task.Delay(1000);

                    await connection.SendAsync("SendGetSwitchValuesCommand");

                    _logger.LogInformation("ReceiveSwitchCommand Complete at: {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"ReceiveSwitchCommand error : {ex.Message};;{ex.InnerException?.Message} at: {DateTimeOffset.Now}");
                }

            });

            connection.On("ReceiveGetSwitchValuesCommand", async () =>
            {
                try
                {
                    _logger.LogInformation("ReceiveGetSwitchValuesCommand at: {time}", DateTimeOffset.Now);

                    var dictionary = new Dictionary<int, bool>(3);

                    lock (_lock)
                    {
                        _controller.OpenPin(26, PinMode.Output);
                        var value26 = _controller.Read(26);
                        _controller.ClosePin(26);

                        _controller.OpenPin(20, PinMode.Output);
                        var value20 = _controller.Read(20);
                        _controller.ClosePin(20);

                        _controller.OpenPin(21, PinMode.Output);
                        var value21 = _controller.Read(21);
                        _controller.ClosePin(21);

                        dictionary.Add(26, value26 == PinValue.High ? true : false);
                        dictionary.Add(20, value20 == PinValue.High ? true : false);
                        dictionary.Add(21, value21 == PinValue.High ? true : false);
                    }

                    await connection.SendAsync("SendSwitchValues", dictionary);

                    _logger.LogInformation("ReceiveGetSwitchValuesCommand Complete at: {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"ReceiveGetSwitchValuesCommand error : {ex.Message};;{ex.InnerException?.Message} at: {DateTimeOffset.Now}");
                }
            });

            await connection.StartAsync();

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            try
            {
                var dictionary = new Dictionary<int, bool>(3);

                lock (_lock)
                {
                    _controller.OpenPin(26, PinMode.Output);
                    var value26 = _controller.Read(26);
                    _controller.ClosePin(26);

                    _controller.OpenPin(20, PinMode.Output);
                    var value20 = _controller.Read(20);
                    _controller.ClosePin(20);

                    _controller.OpenPin(21, PinMode.Output);
                    var value21 = _controller.Read(21);
                    _controller.ClosePin(21);

                    dictionary.Add(26, value26 == PinValue.High ? true : false);
                    dictionary.Add(20, value20 == PinValue.High ? true : false);
                    dictionary.Add(21, value21 == PinValue.High ? true : false);
                }

                await connection.SendAsync("SendSwitchValues", dictionary);

                _logger.LogInformation("SendSwitchValues at: {time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"SendSwitchValues error : {ex.Message};;{ex.InnerException?.Message} at: {DateTimeOffset.Now}");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogInformation("Worker stopping at: {time}", DateTimeOffset.Now);

            _controller.Dispose();

            await connection.StopAsync();

            await connection.DisposeAsync();
        }
    }
}