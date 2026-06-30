using cCoder.Workflow.Activities.Support;
using FluentAssertions;
using Xunit;

namespace cCoder.Workflow.Activities.Tests;

public sealed partial class CSVParserTests
{
    [Fact]
    public void Parse_WhenHeadersAreProvided_MapsTypedRows()
    {
        // Given
        const string csv = "Name,Active\r\nExample,true";
        CSVParseConfig options = new()
        {
            FieldNamesInHeader = true,
            Separator = ',',
        };

        // When
        TestRow actualRow = CSVParser<TestRow>.Parse(csv, options).Single();

        // Then
        actualRow.Name.Should().Be("Example");
        actualRow.Active.Should().BeTrue();
    }
}
