using System.Diagnostics;
using System.Text.RegularExpressions;
using TlcvExtensionsHost.Configs;
using TlcvExtensionsHost.Models;
using static System.Formats.Asn1.AsnWriter;

namespace TlcvExtensionsHost.Services
{
    public class Engine : IEngine
    {
        private readonly ILogger<Engine> _logger;
        private Process _process;
        private string _currentFen;

        public EngineConfig Config { get; private set; }
        public EngineInfo CurrentEngineInfo { get; private set; }

        public Engine(ILogger<Engine> logger)
        {
            _logger = logger;
        }

        public async Task RunAsync(EngineConfig config)
        {
            Config = config;
            _logger.LogInformation("Running {EngineName} at {EnginePath}", config.Name, config.Path);

            if (File.Exists(config.Path))
            {
                _logger.LogWarning("Engine at {EnginePath} doesn't exist.", config.Path);
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = config.Path,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _process = Process.Start(startInfo);
            await SendAsync("uci");
            if (config.Options != null)
            {
                foreach (var option in config.Options)
                {
                    await SendAsync($"setoption Name {option.Name} value {option.Value}");
                }
            }
            while (true)
            {
                var line = await _process.StandardOutput.ReadLineAsync();
                HandleOutput(line);
            }
        }

        private void HandleOutput(string line)
        {
            _logger.LogDebug("{EngineName} >> {EngineOutput}", Config.Name, line);

            var success = TryGetEngineInfo(line, out var info);
            if (success)
            {
                CurrentEngineInfo = info;
            }
        }

        private readonly Regex _cpRegex = new Regex(" score cp (-?\\d+)", RegexOptions.Compiled);
        private readonly Regex _depthRegex = new Regex(" depth (\\d+)", RegexOptions.Compiled);
        private readonly Regex _nodesRegex = new Regex(" nodes (\\d+)", RegexOptions.Compiled);
        private readonly Regex _npsRegex = new Regex(" nps (\\d+)", RegexOptions.Compiled);
        private readonly Regex _pvRegex = new Regex(" pv (.+)", RegexOptions.Compiled);

        private bool TryGetEngineInfo(string line, out EngineInfo info)
        {
            info = new EngineInfo();
            if (!line.Contains("info"))
            {
                return false;
            }

            if (line.Contains("lowerbound"))
            {
                return false;
            }

            if (line.Contains("upperbound"))
            {
                return false;
            }

            if (line.Contains("currmove"))
            {
                return false;
            }

            info.Name = Config.Name;

            var cpMatch = _cpRegex.Match(line);
            if (cpMatch.Success)
            {
                if (int.TryParse(cpMatch.Groups[1].Value, out var cp))
                {
                    if (_currentFen.Contains(" b "))
                    {
                        cp = -cp;
                    }
                    info.Score = cp.ToString();
                }
            }

            var depthMatch = _depthRegex.Match(line);
            if (depthMatch.Success)
            {
                if (int.TryParse(depthMatch.Groups[1].Value, out var depth))
                {
                    info.Depth = depth;
                }
            }

            var nodesMatch = _nodesRegex.Match(line);
            if (nodesMatch.Success)
            {
                if (long.TryParse(nodesMatch.Groups[1].Value, out var nodes))
                {
                    info.Nodes = nodes;
                }
            }

            var npsMatch = _npsRegex.Match(line);
            if (npsMatch.Success)
            {
                if (long.TryParse(npsMatch.Groups[1].Value, out var nps))
                {
                    info.Nps = nps;
                }
            }

            var pvMatch = _pvRegex.Match(line);
            if (pvMatch.Success)
            {
                info.Pv = pvMatch.Groups[1].Value;
            }

            return true;
        }

        private async Task SendAsync(string line)
        {
            _logger.LogDebug("{EngineName} << {EngineOutput}", Config.Name, line);
            await _process.StandardInput.WriteLineAsync(line);
        }

        public async Task SetFenAsync(string fen)
        {
            _currentFen = fen;
            await SendAsync("stop");
            if (fen == "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
            {
                await SendAsync("ucinewgame");
            }
            await SendAsync($"position fen {fen}");
            await SendAsync("go infinite");
        }
    }
}
