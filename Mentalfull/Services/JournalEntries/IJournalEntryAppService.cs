using Mentalfull.Services.Dtos.JournalEntries;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using Volo.Abp.Content;

namespace Mentalfull.Services.JournalEntries;

public interface IJournalEntryAppService : IApplicationService
{
    Task<JournalEntryDto> GetAsync(Guid id);
    Task<PagedResultDto<JournalEntryDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    Task<List<JournalEntryDto>> GetMyJournalEntriesAsync();
    Task<JournalEntryDto> CreateAsync(CreateUpdateJournalEntryDto input);
    Task<JournalEntryDto> UpdateAsync(Guid id, CreateUpdateJournalEntryDto input);
    Task DeleteAsync(Guid id);
    Task<JournalEntryDto> TogglePinAsync(Guid id);
    Task<string> UploadVoiceAsync(IRemoteStreamContent input);
}
