namespace PlanyApp.Service.Interfaces
{
    public interface IProvinceDetectionService
    {
        Task<int?> DetectProvinceFromNameAsync(string planName);
    }
} 