namespace KubeFleetUI.Models;

public static class KubeFleetConstants
{
    public const string Group = "placement.kubernetes-fleet.io";
    public const string Version = "v1beta1";
    public const string StagedUpdateRunPlural = "stagedupdateruns";
    public const string ClusterStagedUpdateRunPlural = "clusterstagedupdateruns";
    public const string StagedUpdateStrategyPlural = "stagedupdatestrategies";
    public const string ClusterStagedUpdateStrategyPlural = "clusterstagedupdatestrategies";
    public const string ApprovalRequestPlural = "approvalrequests";
    public const string ClusterApprovalRequestPlural = "clusterapprovalrequests";

    public const string LabelTargetUpdateRun = "fleet.kubernetes-fleet.io/targetUpdateRun";
    public const string LabelTargetUpdatingStage = "fleet.kubernetes-fleet.io/targetUpdatingStage";
    public const string LabelIsLatestApproval = "fleet.kubernetes-fleet.io/isLatestUpdateRunApproval";
    public const string LabelTaskType = "fleet.kubernetes-fleet.io/taskType";
}
