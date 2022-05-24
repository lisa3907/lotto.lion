using Lion.Share;
using Lion.Share.Pipe;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Pipes;
using System.Runtime.Versioning;
using System.Security.AccessControl;

namespace Lion.Worker.Engine
{
    /// <summary>
    ///
    /// </summary>
    public class Receiver : BackgroundService
    {
        private readonly ILogger<Receiver> _logger;

        public Receiver(ILogger<Receiver> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[worker.receiver] starting assister...");

            var _pipe_secure = new PipeSecurity();
            _pipe_secure.SetAccessRule(new PipeAccessRule("Everyone", PipeAccessRights.FullControl, AccessControlType.Allow));

            var _pipe_server = NamedPipeServerStreamAcl.Create(
                                    PQueue.ReceiveServerPipeName, PipeDirection.InOut,
                                    20, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 512, 512,
                                    _pipe_secure
                                );

            var _reader = new StreamReader(_pipe_server);
            var _writer = new StreamWriter(_pipe_server);

            var _response = new VmsResponse<bool>();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _pipe_server.WaitForConnectionAsync();

                    var _read_line = await _reader.ReadLineAsync();

                    var _request = JsonConvert.DeserializeObject<VmsRequest<JToken>>(_read_line);
                    {
                        _response.command = _request.command;
                        _response.message = "";
                        _response.data = true;

                        if (_request.command == QCommand.AnalystQ)
                            PQueue.QAnalyst.Enqueue(_read_line);
                        else if (_request.command == QCommand.ChoicerQ)
                            PQueue.QChoicer.Enqueue(_read_line);
                        else if (_request.command == QCommand.SelectorQ)
                            PQueue.QSelector.Enqueue(_read_line);
                        else if (_request.command == QCommand.WinnerQ)
                            PQueue.QWinner.Enqueue(_read_line);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "receiver.choicer");

                    _response.message = ex.InnerMessage();
                    _response.data = false;
                }
                finally
                {
                    if (_pipe_server.IsConnected)
                    {
                        var _write_line = JsonConvert.SerializeObject(_response);

                        await _writer.WriteLineAsync(_write_line);
                        await _writer.FlushAsync();

                        _pipe_server.Disconnect();
                    }
                }
            }

            _pipe_server.Close();
        }
    }
}