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
        DataUser user = TestUsers.WithPrivilege(privilege:"flowinstancedata_update", appId:1);
        currentUser = user;
        flowInstanceDataServiceMock.Setup(expression:x => x.Get(entity.Id)).Returns(value:dbVersion);

        flowInstanceDataServiceMock
            .Setup(expression:x => x.UpdateAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync(valueFunction:(FlowInstanceData updated) => updated);

        // When
        FlowInstanceData result = await flowInstanceDataProcessingService.UpdateAsync(entity:entity);

        // Then
        Assert.Equal(expected:entity.Name, actual:result.Name);
        flowInstanceDataServiceMock.Verify(
expression:            x => x.UpdateAsync(It.Is<FlowInstanceData>(item => item.Id == entity.Id)),
times:            Times.Once
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
        flowInstanceDataServiceMock.Setup(expression:x => x.Get(entity.Id)).Returns(value:dbVersion);
        flowInstanceDataServiceMock
            .Setup(expression:x => x.UpdateAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync(valueFunction:(FlowInstanceData updated) => updated);

        // When
        FlowInstanceData actualFlowInstanceData =
            await flowInstanceDataProcessingService.UpdateAsync(entity:entity);

        // Then
        Assert.Equal(expected:entity.Name, actual:actualFlowInstanceData.Name);
        flowInstanceDataServiceMock.Verify(
expression:            x => x.UpdateAsync(It.Is<FlowInstanceData>(item => item.Id == entity.Id)),
times:            Times.Once
        );
    }

}