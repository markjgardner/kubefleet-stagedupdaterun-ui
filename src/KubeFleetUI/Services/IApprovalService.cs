using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public interface IApprovalService
{
    Task<List<ApprovalRequest>> ListForRunAsync(string updateRunName, string? ns = null);
    Task ApproveAsync(string approvalRequestName, string? ns = null);
}
