﻿using System.ComponentModel.DataAnnotations;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest
{
    private readonly CategoryTestFixture _categoryTestFixture;

    public CategoryTest(CategoryTestFixture categoryTestFixture)
        => _categoryTestFixture = categoryTestFixture;


    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        // arrange
        var validCategory = _categoryTestFixture.GetValidCategory();

        // act
        var datetimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(
            validCategory.Name,
            validCategory.Description);

        var datetimeAfter = DateTime.Now.AddSeconds(1);

        // assert
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        (category.CreatedAt >= datetimeBefore).Should().BeTrue();
        (category.CreatedAt <= datetimeAfter).Should().BeTrue();
        (category.IsActive).Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var datetimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(
            validCategory.Name,
            validCategory.Description,
            isActive);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(category.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        (category.CreatedAt >= datetimeBefore).Should().BeTrue();
        (category.CreatedAt <= datetimeAfter).Should().BeTrue();
        (category.IsActive).Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action =
            () => new DomainEntity.Category(name!, validCategory.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be null or empty");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action =
            () => new DomainEntity.Category(validCategory.Name, null!);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should not be null");
    }

    // nome deve ter no minimo 3 caracteres
    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("a")]
    [InlineData("ab")]
    public void InstantiateErrorWhenNameIsLessThan3Characters(string invalidName)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action =
            () => new DomainEntity.Category(invalidName, validCategory.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at leats 3 characetrs long");
    }

    // nome deve ter no maximo 255 caracteres
    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThan255Characters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());

        Action action =
            () => new DomainEntity.Category(invalidName, validCategory.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characetrs long");
    }

    // descrição deve ter no maximo 10_000 caracteres
    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var invalidDescription = String.Join(null, Enumerable.Range(1, 10_001).Select(_ => "a").ToArray());

        Action action =
            () => new DomainEntity.Category(validCategory.Name, invalidDescription);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10000 characetrs long");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(
            validCategory.Name,
            validCategory.Description,
            false);
        category.Activate();

        (category.IsActive).Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(
            validCategory.Name,
            validCategory.Description,
            true);
        category.Deactivate();

        (category.IsActive).Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        var category = _categoryTestFixture.GetValidCategory();

        // como agora os dados são randomicos, podemos apenas instânciar um novo objeto
        var categoryWithNewValues = _categoryTestFixture.GetValidCategory();

        category.Update(categoryWithNewValues.Name, categoryWithNewValues.Description);

        category.Name.Should().Be(categoryWithNewValues.Name);
        category.Description.Should().Be(categoryWithNewValues.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var newName = _categoryTestFixture.GetValidCategoryName();
        var currentDescription = category.Description;

        category.Update(newName);

        category.Name.Should().Be(newName);
        category.Description.Should().Be(currentDescription);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        var category = _categoryTestFixture.GetValidCategory();

        Action action =
            () => category.Update(name!);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be null or empty");
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [MemberData(nameof(GetWhenNameIsLessThan3Characters), parameters: 10)]
    public void UpdateErrorWhenNameIsLessThan3Characters(string invalidName)
    {
        var category = _categoryTestFixture.GetValidCategory();
        Action action =
            () => category.Update(invalidName!);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at leats 3 characetrs long");
    }

    // MemberData recebe a lista de dados que vai usar para o teste
    public static IEnumerable<object[]> GetWhenNameIsLessThan3Characters(int numberOfTests)
    {
        var fixture = new CategoryTestFixture();

        for (int i = 0; i < numberOfTests; i++)
        {
            var isOdd  = i % 2 == 0;
            yield return new object[] {
                fixture.GetValidCategoryName()[..(isOdd ? 1 : 2)]
            };
        }
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThan255Characters()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var invalidName = _categoryTestFixture.Faker.Lorem.Letter(256);

        Action action =
            () => category.Update(invalidName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characetrs long");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var invalidDescription =
            _categoryTestFixture.Faker.Commerce.ProductDescription();

        while (invalidDescription.Length <= 10_000)
            invalidDescription = $"{invalidDescription} {_categoryTestFixture.Faker.Commerce.ProductDescription()}";

        Action action =
            () => category.Update("Category new name", invalidDescription);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10000 characetrs long");
    }
}