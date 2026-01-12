namespace VendorRisk.Domain.Common;

public class DocumentStatus : ValueObject
{
    public bool ContractValid { get; private set; }
    public bool PrivacyPolicyValid { get; private set; }
    public bool PentestReportValid { get; private set; }

    private DocumentStatus() { }

    public DocumentStatus(bool contractValid, bool privacyPolicyValid, bool pentestReportValid)
    {
        ContractValid = contractValid;
        PrivacyPolicyValid = privacyPolicyValid;
        PentestReportValid = pentestReportValid;
    }

    public int ValidDocumentsCount =>
        (ContractValid ? 1 : 0) +
        (PrivacyPolicyValid ? 1 : 0) +
        (PentestReportValid ? 1 : 0);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ContractValid;
        yield return PrivacyPolicyValid;
        yield return PentestReportValid;
    }
}
