using FC.Codeflix.Catalog.Domain.Validation;
using Bogus;
using FluentAssertions;
using Xunit;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Validation;
public class DomainValidationTest
{
    private Faker Faker { get; set; } = new Faker();

    [Fact(DisplayName = nameof(NotNullOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOk()
    {
        var value = Faker.Commerce.ProductName();
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
            () => DomainValidation.NotNull(value, fieldName);

        action.Should().NotThrow();
    }

    [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullThrowWhenNull()
    {
        string? value = null;
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
            () => DomainValidation.NotNull(value, fieldName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be null");
    }

    [Theory(DisplayName = nameof(NotNullOrEmptyThrowWhenEmpty))]
    [Trait("Domain", "DomainValidation - Validation")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void NotNullOrEmptyThrowWhenEmpty(string? target)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
            () => DomainValidation.NotNullOrEmpty(target, fieldName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be null or empty");
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyOk()
    {
        var target = Faker.Commerce.ProductName();
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
            () => DomainValidation.NotNullOrEmpty(target, fieldName);

        action.Should().NotThrow();
    }

    [Theory(DisplayName = nameof(MinLengthThrowWhenLess))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValeusSmallerThanMin), parameters: 10)]
    public void MinLengthThrowWhenLess(string target, int minLength)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
            () => DomainValidation.MinLength(target, minLength, fieldName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be at leats {minLength} characetrs long");
    }

    public static IEnumerable<object[]> GetValeusSmallerThanMin(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 10 };

        var faker = new Faker();
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length + new Random(i).Next(1, 20);
            yield return new object[] { example, minLength };
        }
    }

    [Theory(DisplayName = nameof(MinLengthOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValeusGreaterThanMin), parameters: 10)]
    public void MinLengthOk(string target, int minLength)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
            () => DomainValidation.MinLength(target, minLength, fieldName);

        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValeusGreaterThanMin(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 6 };

        var faker = new Faker();
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length - new Random(i).Next(1, 5);
            yield return new object[] { example, minLength };
        }
    }

    [Theory(DisplayName = nameof(MaxLengthThrowWhenGreater))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValeusGreaterThanMax), parameters: 10)]
    public void MaxLengthThrowWhenGreater(string target, int maxLength)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
            () => DomainValidation.MaxLength(target, maxLength, fieldName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be less or equal {maxLength} characetrs long");
    }

    public static IEnumerable<object[]> GetValeusGreaterThanMax(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 5 };

        var faker = new Faker();
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLength = example.Length - new Random(i).Next(1, 5);
            yield return new object[] { example, maxLength };
        }
    }

    [Theory(DisplayName = nameof(MaxLengthOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValeusLessThanMax), parameters: 10)]
    public void MaxLengthOk(string target, int minLength)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
            () => DomainValidation.MaxLength(target, minLength, fieldName);

        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValeusLessThanMax(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 6 };

        var faker = new Faker();
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length + new Random(i).Next(0, 5);
            yield return new object[] { example, minLength };
        }
    }
}
