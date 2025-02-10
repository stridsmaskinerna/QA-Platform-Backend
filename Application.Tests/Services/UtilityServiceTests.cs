using Application.Services;

namespace Application.Tests.Services;

public class UtilityServiceTests
{
    private readonly UtilityService _utilityService;

    public UtilityServiceTests()
    {
        _utilityService = new UtilityService();
    }

    public class NormalizeText : UtilityServiceTests
    {
        [Theory]
        [InlineData(" hello ", "HELLO", "Trim spaces")]
        [InlineData("Hello World", "HELLO_WORLD", "Replace spaces with underscore")]
        [InlineData("  Trim  Me  ", "TRIM_ME", "Multiple spaces reduced to single underscore")]
        [InlineData("café", "CAFÉ", "Unicode normalization")]
        [InlineData("Über", "ÜBER", "Normalization of composed characters")]
        [InlineData("\nLine\nBreaks", "LINE_BREAKS", "Newline characters replaced")]
        [InlineData("multiple    spaces", "MULTIPLE_SPACES", "Extra spaces reduced to single underscore")]
        [InlineData("Special_chars!", "SPECIAL_CHARS!", "Does not remove special characters")]
        [InlineData("", "", "Empty string remains empty")]
        [InlineData("    ", "", "Only spaces should be trimmed to empty")]
        public void NormalizeText_ShouldReturnExpectedResult(
            string input,
            string expected,
            string explanation
        )
        {
            // Act
            var result = _utilityService.NormalizeText(input);

            var whyFailing = $"❌ {explanation} | Expected: '{expected}', but got: '{result}'";

            // Assert
            Assert.True(result == expected, whyFailing);
        }
    }
}
