namespace MildlySublime.Result.Core.Tests;

[TestClass]
public sealed class ResultFailure
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
        var result = _testService.Error();

        result.HandleResult(onSuccess: () =>
            {
                Assert.Fail("Should not succeed");
                return Result.Successful;
            },
            onError: e =>
            {
                Assert.AreEqual(1, e.Errors.Count);

                var error = e.Errors.First();
                Assert.AreEqual(TestService.FailAddCode, error.Code);
                Assert.AreEqual(TestService.FailMessageJustError, error.Message);

                return e;
            });
    }
    
    [TestMethod]
    public async Task Void_Async()
    {
        var result = _testService.Error();

        await result.HandleResultAsync(onSuccess: async () =>
            {
                await Task.FromResult(false);
                Assert.Fail("Should not succeed");
                return Result.Successful;
            },
            onError: async e =>
            {
                await Task.FromResult(false);
                Assert.AreEqual(1, e.Errors.Count);

                var error = e.Errors.First();
                Assert.AreEqual(TestService.FailAddCode, error.Code);
                Assert.AreEqual(TestService.FailMessageJustError, error.Message);

                return e;
            });
    }
    
    [TestMethod]
    public void Typed_Return_Sync()
    {
        var result = _testService.AddNumberError(1, 2);

        result.HandleReturnResult(onSuccess: s =>
        {
            Assert.Fail("Should not succeed");
            return s;
        },
        onError: e =>
        {
            Assert.AreEqual(1, e.Errors.Count);

            var error = e.Errors.First();
            Assert.AreEqual(TestService.FailAddCode, error.Code);
            Assert.AreEqual(TestService.FailAddMessage, error.Message);

            return e;
        });
    }
    
    [TestMethod]
    public async Task Typed_Return_Async()
    {
        var result = _testService.AddNumberError(1, 2);

        await result.HandleReturnResultAsync(onSuccess: async s =>
            {
                await Task.FromResult(false);
                Assert.Fail("Should not succeed");
                return s;
            },
            onError: async e =>
            {
                await Task.FromResult(false);
                Assert.AreEqual(1, e.Errors.Count);

                var error = e.Errors.First();
                Assert.AreEqual(TestService.FailAddCode, error.Code);
                Assert.AreEqual(TestService.FailAddMessage, error.Message);

                return e;
            });
    }
    
    [TestMethod]
    public void Typed_Void_Sync()
    {
        var result = _testService.AddNumberError(1, 2);

        result.HandleResult(onSuccess: s =>
            {
                Assert.Fail("Should not succeed");
            },
            onError: e =>
            {
                Assert.AreEqual(1, e.Errors.Count);

                var error = e.Errors.First();
                Assert.AreEqual(TestService.FailAddCode, error.Code);
                Assert.AreEqual(TestService.FailAddMessage, error.Message);
            });
    }
    
    [TestMethod]
    public async Task Typed_Void_Async()
    {
        var result = _testService.AddNumberError(1, 2);

        await result.HandleResultAsync(onSuccess: async s =>
            {
                await Task.FromResult(false);
                Assert.Fail("Should not succeed");
            },
            onError: async e =>
            {
                await Task.FromResult(false);
                Assert.AreEqual(1, e.Errors.Count);

                var error = e.Errors.First();
                Assert.AreEqual(TestService.FailAddCode, error.Code);
                Assert.AreEqual(TestService.FailAddMessage, error.Message);
            });
    }

    private sealed class TestService
    {
        internal const string FailAddCode = "Failed to add";
        internal const string FailAddMessage = "Failed to add because reasons";

        internal const string FailMessageJustError = "Failed to things";

        public Result<int> AddNumberError(int number1, int number2)
        {
            return Result<int>.CreateError([new ResultError(FailAddCode, FailAddMessage)]);
        }

        public Result Error()
        {
            return Result.CreateError([new ResultError(FailAddCode, FailMessageJustError)]);
        }
    }
}
