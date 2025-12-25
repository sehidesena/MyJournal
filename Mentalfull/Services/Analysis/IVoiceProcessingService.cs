using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Mentalfull.Services.Analysis;

public interface IVoiceProcessingService : IApplicationService
{
    Task<string> TranscribeAudioAsync(string audioUrlOrPath);
    // Future: Task<string> TranscribeAudioAsync(Stream audioStream);
}
