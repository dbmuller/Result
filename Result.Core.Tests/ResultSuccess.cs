namespace MildlySublime.Result.Core.Tests;

[TestClass]
public sealed class ResultSuccess
{
    private TestService _testService = new();

    [TestInitialize]
    public void TestSetup()
    {
        _testService = new();
    }

    [TestMethod]
    public void Void_Sync()
    {
        var result = _testService.Success();
        
        result.HandleResult(onSuccess: () => Result.Successful,
            onError: e =>
            {
                Assert.Fail("Should not fail");

                return e;
            });
    }
    
    [TestMethod]
    public async Task Void_Async()
    {
        var result = _testService.Success();
        
        await result.HandleResultAsync(onSuccess: async () =>
            {
                await Task.FromResult(false);
                return Result.Successful;
            },
            onError: async e =>
            {
                await Task.FromResult(false);
                Assert.Fail("Should not fail");

                return e;
            });
    }
    
    [TestMethod]
    public void Typed_Return_Sync()
    {
        var result = _testService.AddNumber(1, 2);

        result.HandleReturnResult(onSuccess: s =>
            {
                Assert.AreEqual(3, s);
                return s;
            },
            onError: e =>
            {
                Assert.Fail("Should not fail");
                return e;
            });
    }
    
    [TestMethod]
    public async Task Typed_Return_Async()
    {
        var result = _testService.AddNumber(1, 2);

        await result.HandleReturnResultAsync(onSuccess: async s =>
            {
                await Task.FromResult(false);
                Assert.AreEqual(3, s);
                return s;
            },
            onError: async e =>
            {
                await Task.FromResult(false);
                Assert.Fail("Should not fail");

                return e;
            });
    }

    private sealed class TestService
    {
        public Result<int> AddNumber(int number1, int number2)
            => number1 + number2;

        public Result Success()
        {
            return Result.Successful;
        }
    }
}
