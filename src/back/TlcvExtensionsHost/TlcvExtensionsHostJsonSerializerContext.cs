using System.Text.Json.Serialization;
using TlcvExtensionsHost.Models;

namespace TlcvExtensionsHost;

[JsonSerializable(typeof(QueryResponse))]
[JsonSerializable(typeof(EngineInfo))]
[JsonSerializable(typeof(FenRequest))]
[JsonSerializable(typeof(FenResponse))]
internal partial class TlcvExtensionsHostJsonSerializerContext : JsonSerializerContext;
