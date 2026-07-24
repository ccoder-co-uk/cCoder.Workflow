// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using Moq;
using Xunit;
using DataUser = cCoder.Data.Models.Security.User;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowInstanceDataProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseDataContextWhenUserCanUpdateFlowInstanceDataForUpdateAsync()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        entity.FlowDefinition = null!;
        FlowInstanceData dbVersion = CreateRandomFlowInstanceData();
        dbVersion.Id = entity.Id;
        dbVersion.FlowDefinitionId = entity.FlowDefinitionId;
        dbVersion.FlowDefinition = new FlowDefinition
        {
            Id = entity.FlowDefinitionId,
            AppId = 1,
            Name = "Flow",
            DefinitionJson = "{}",
            ConfigJson = "{}",
            App = null!,
            Instances = [],
        };
        DataUser user = TestUsers.WithPrivilege("flowinstancedata_update", 1);
        currentUser = user;
        flowInstanceDataServiceMock.Setup(x => x.Get(entity.Id)).Returns(dbVersion);

        flowInstanceDataServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync((FlowInstanceData updated) => updated);

        // When
        FlowInstanceData result = await flowInstanceDataProcessingService.UpdateAsync(entity);

        // Then
        Assert.Equal(entity.Name, result.Name);
        flowInstanceDataServiceMock.Verify(
            x => x.UpdateAsync(It.Is<FlowInstanceData>(item => item.Id == entity.Id)),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldUpdateWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        entity.FlowDefinition = null!;
        FlowInstanceData dbVersion = CreateRandomFlowInstanceData();
        dbVersion.Id = entity.Id;
        dbVersion.FlowDefinitionId = entity.FlowDefinitionId;
        dbVersion.FlowDefinition = new FlowDefinition
        {
            Id = entity.FlowDefinitionId,
            AppId = 1,
            Name = "Flow",
            DefinitionJson = "{}",
            ConfigJson = "{}",
            App = null!,
            Instances = [],
        };
        flowInstanceDataServiceMock.Setup(x => x.Get(entity.Id)).Returns(dbVersion);
        flowInstanceDataServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync((FlowInstanceData updated) => updated);

        // When
        FlowInstanceData actualFlowInstanceData =
            await flowInstanceDataProcessingService.UpdateAsync(entity);

        // Then
        Assert.Equal(entity.Name, actualFlowInstanceData.Name);
        flowInstanceDataServiceMock.Verify(
            x => x.UpdateAsync(It.Is<FlowInstanceData>(item => item.Id == entity.Id)),
            Times.Once
        );
    }

}