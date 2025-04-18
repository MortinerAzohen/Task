using System.Collections.Concurrent;

namespace SoapService.Services
{

    public class LongJobService : ILongJobService
    {
        private static ConcurrentDictionary<string, string> _jobResults = new();
        private static ConcurrentDictionary<string, DateTime> _jobStartTimes = new();

        public string StartJob(string callerId)
        {
            var jobId = Guid.NewGuid().ToString();

            Task.Run(async () =>
            {
                _jobStartTimes[jobId] = DateTime.UtcNow;
                await Task.Delay(TimeSpan.FromMinutes(2));

                var result = $"Result for {callerId} at {DateTime.UtcNow}";
                _jobResults[jobId] = result;
            });

            return jobId;
        }

        public string GetJobResult(string jobId)
        {
            return _jobResults.TryGetValue(jobId, out var result)
                ? result
                : "Processing";
        }
    }
}
