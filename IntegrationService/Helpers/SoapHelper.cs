using System.Text;
using System.Xml.Linq;

namespace IntegrationService.Helpers;

public class SoapHelper
{
    private readonly HttpClient _client;

    public SoapHelper(HttpClient client)
    {
        _client = client;
    }

    public async Task<string> StartJobAsync(string userId)
    {
        var soapEnvelope = $@"
        <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
            <s:Body>
                <tem:StartJob>
                    <tem:callerId>{userId}</tem:callerId>
                </tem:StartJob>
            </s:Body>
        </s:Envelope>";

        var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", "http://tempuri.org/ILongJobService/StartJob");

        var response = await _client.PostAsync("http://localhost:5002/LongJobService", content);
        var xml = await response.Content.ReadAsStringAsync();

        var doc = XDocument.Parse(xml);
        return doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "StartJobResult")?.Value ?? "";
    }

    public async Task<string> PollJobResultAsync(string jobId)
    {
        for (int i = 0; i < 10; i++) // max 10 attempts
        {
            var envelope = $@"
            <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
                <s:Body>
                    <tem:GetJobResult>
                        <tem:jobId>{jobId}</tem:jobId>
                    </tem:GetJobResult>
                </s:Body>
            </s:Envelope>";

            var content = new StringContent(envelope, Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", "http://tempuri.org/ILongJobService/GetJobResult");

            var res = await _client.PostAsync("http://localhost:5002/LongJobService", content);
            var xml = await res.Content.ReadAsStringAsync();

            var doc = XDocument.Parse(xml);
            var result = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "GetJobResultResult")?.Value ?? "";

            if (result != "Processing")
                return result;

            await Task.Delay(3000); // wait before next attempt
        }

        return "Timed out waiting for SOAP result";
    }
}
