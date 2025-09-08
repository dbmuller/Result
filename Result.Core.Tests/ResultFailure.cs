namespace MildlySublime.Result.Core.Tests;

[TestClass]
public sealed class ResultFailure
{
    private AddNumbers _addNumbers = new();

    [TestInitialize]
    public void TestSetup()
    {
        _addNumbers = new();
    }

    [TestMethod]
    public void Typed_Return_Sync()
    {
        var result = _addNumbers.AddNumber(1, 2);

        result.HandleReturnResult(onSuccess: s =>
        {
            Assert.Fail("Should not succeed");
            return s;
        },
        onError: e =>
        {
            Assert.AreEqual(1, e.Errors.Count);

            var error = e.Errors.First();
            Assert.AreEqual(AddNumbers.FailAddCode, error.Code);
            Assert.AreEqual(AddNumbers.FailAddMessage, error.Message);

            return e;
        });
    }

    private sealed class AddNumbers
    {
        internal const string FailAddCode = "Failed to add";
        internal const string FailAddMessage = "Failed to add because reasons";

        public Result<int> AddNumber(int number1, int number2)
        {
            return Result<int>.CreateError([new ResultError(FailAddCode, FailAddMessage)]);
        }
    }

}

