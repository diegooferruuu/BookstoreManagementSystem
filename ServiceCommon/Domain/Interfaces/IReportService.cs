namespace ServiceCommon.Domain.Interfaces
{
    public interface IReportService
    {
        byte[] GenerateReport();
        string GetContentType();
        string GetFileExtension();
    }
}
