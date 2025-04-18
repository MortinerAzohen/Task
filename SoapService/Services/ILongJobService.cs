using CoreWCF;

namespace SoapService.Services
{
    [ServiceContract]
    public interface ILongJobService
    {
        [OperationContract]
        string StartJob(string callerId);

        [OperationContract]
        string GetJobResult(string jobId);
    }
}
