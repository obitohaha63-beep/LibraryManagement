using Library.Core;
namespace Library.Tests;
public class LoanRulesTests
{
    [Theory]
    [InlineData(0, false, 0)] [InlineData(1, false, 1)] [InlineData(-1, false, 0)] [InlineData(5, true, 0)]
    public void OverdueUsesCalendarDate(int days, bool returned, int expected) => Assert.Equal(expected, LoanRules.DaysOverdue(new DateOnly(2026, 10, 23).AddDays(-days), new(2026, 10, 23), returned));
    [Fact] public void NormalizesCopyCodes() => Assert.Equal(["CP001", "CP002"], LoanRules.NormalizeCodes(["cp002 ", " CP001"]));
    [Theory] [InlineData("CP001", " cp001 ")] [InlineData("", "CP001")]
    public void InvalidCopyListsFail(string a, string b) => Assert.Throws<BusinessException>(() => LoanRules.NormalizeCodes([a, b]));
    [Theory] [InlineData(false, 0, 1, false, "READER_BLOCKED")] [InlineData(true, 3, 1, false, "LOAN_LIMIT")] [InlineData(true, 0, 1, true, "OVERDUE")]
    public void EligibilityRejectsInvalidReader(bool active, int current, int count, bool overdue, string code) => Assert.Equal(code, Assert.Throws<BusinessException>(() => LoanRules.CheckEligibility(active, current, count, overdue, new())).Code);
    [Fact] public void BoundaryAllowsThirdCopy() => LoanRules.CheckEligibility(true, 2, 1, false, new());
    [Fact] public void PolicyCanAllowBorrowingWhenOverdue() => LoanRules.CheckEligibility(true, 0, 1, true, new() { BlockIfOverdue = false });
    [Theory] [InlineData("images/books/a.jpg", true)] [InlineData("C:\\books\\a.png", false)] [InlineData("/images/books/a.jpg", false)] [InlineData("images/books/../a.png", false)] [InlineData("https://example.com/a.jpg", false)] [InlineData("images/books/a.svg", true)]
    public void OnlyRelativeBookPathsAreAllowed(string path, bool valid) => Assert.Equal(valid, LoanRules.IsRelativeBookImage(path));
}
