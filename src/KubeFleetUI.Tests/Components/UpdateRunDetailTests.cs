using KubeFleetUI.Models;

namespace KubeFleetUI.Tests.Components;

public class UpdateRunDetailTests
{
    [Fact]
    public void StagedUpdateRun_DefaultValues_AreCorrect()
    {
        var run = new StagedUpdateRun();

        Assert.Equal("placement.kubernetes-fleet.io/v1beta1", run.ApiVersion);
        Assert.Equal("StagedUpdateRun", run.Kind);
        Assert.NotNull(run.Metadata);
        Assert.NotNull(run.Spec);
        Assert.Null(run.Status);
    }

    [Fact]
    public void UpdateRunSpec_DefaultState_IsInitialize()
    {
        var spec = new UpdateRunSpec();
        Assert.Equal(UpdateRunState.Initialize, spec.State);
    }

    [Fact]
    public void Condition_CanRepresentAllStates()
    {
        var trueCondition = new Condition { Type = "Initialized", Status = "True" };
        var falseCondition = new Condition { Type = "Succeeded", Status = "False" };
        var unknownCondition = new Condition { Type = "Progressing", Status = "Unknown" };

        Assert.Equal("True", trueCondition.Status);
        Assert.Equal("False", falseCondition.Status);
        Assert.Equal("Unknown", unknownCondition.Status);
    }

    [Fact]
    public void StageUpdatingStatus_ClustersDefaultEmpty()
    {
        var status = new StageUpdatingStatus();
        Assert.NotNull(status.Clusters);
        Assert.Empty(status.Clusters);
    }

    [Fact]
    public void ClusterUpdatingStatus_HasName()
    {
        var cluster = new ClusterUpdatingStatus { ClusterName = "cluster-1" };
        Assert.Equal("cluster-1", cluster.ClusterName);
    }

    [Fact]
    public void StageTaskStatus_TypesAreCorrect()
    {
        var timedWait = new StageTaskStatus { Type = StageTaskType.TimedWait };
        var approval = new StageTaskStatus { Type = StageTaskType.Approval, ApprovalRequestName = "req-1" };

        Assert.Equal(StageTaskType.TimedWait, timedWait.Type);
        Assert.Equal(StageTaskType.Approval, approval.Type);
        Assert.Equal("req-1", approval.ApprovalRequestName);
    }

    [Fact]
    public void UpdateRunStatus_CanHaveMultipleStages()
    {
        var status = new UpdateRunStatus
        {
            StagesStatus = new List<StageUpdatingStatus>
            {
                new() { StageName = "stage1" },
                new() { StageName = "stage2" },
                new() { StageName = "stage3" }
            }
        };

        Assert.Equal(3, status.StagesStatus.Count);
    }

    [Fact]
    public void ApprovalRequest_HasCorrectDefaults()
    {
        var ar = new ApprovalRequest();
        Assert.Equal("placement.kubernetes-fleet.io/v1beta1", ar.ApiVersion);
        Assert.Equal("ApprovalRequest", ar.Kind);
    }

    [Fact]
    public void WatchEventTypes_HasCorrectValues()
    {
        Assert.Equal("ADDED", WatchEventTypes.Added);
        Assert.Equal("MODIFIED", WatchEventTypes.Modified);
        Assert.Equal("DELETED", WatchEventTypes.Deleted);
        Assert.Equal("ERROR", WatchEventTypes.Error);
    }
}
