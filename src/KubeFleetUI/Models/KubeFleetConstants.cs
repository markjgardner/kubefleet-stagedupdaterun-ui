namespace KubeFleetUI.Models;

public static class KubeFleetConstants
{
    public const string Group = "placement.kubefleet.io";
    public const string Version = "v1beta1";
    public const string StagedUpdateRunPlural = "stagedupdateruns";
    public const string ClusterStagedUpdateRunPlural = "clusterstagedupdateruns";
    public const string StagedUpdateStrategyPlural = "stagedupdatestrategies";
    public const string ClusterStagedUpdateStrategyPlural = "clusterstagedupdatestrategies";
    public const string ApprovalRequestPlural = "approvalrequests";
    public const string ClusterApprovalRequestPlural = "clusterapprovalrequests";

    public const string LabelTargetUpdateRun = "fleet.kubefleet.io/targetUpdateRun";
    public const string LabelTargetUpdatingStage = "fleet.kubefleet.io/targetUpdatingStage";
    public const string LabelIsLatestApproval = "fleet.kubefleet.io/isLatestUpdateRunApproval";
    public const string LabelTaskType = "fleet.kubefleet.io/taskType";
}
