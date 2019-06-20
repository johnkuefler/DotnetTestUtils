## Dotnet Test Utils

[![Build status](https://ci.appveyor.com/api/projects/status/lqs7dw1wxior9dh1?svg=true)](https://ci.appveyor.com/project/johnkuefler/dotnettestutils)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)
[![NuGet](https://img.shields.io/nuget/v/DotnetTestUtils.svg?label=NuGet)]

A small collection of utilities, factories, and builders to make unit and integration testing real-world .Net Core applications easier.
These tools heavily utilize [AutoFixture](https://github.com/AutoFixture/AutoFixture) and [EF Core in memory database](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/). 
It provides factory and builder tools for AutoFixture and EntityFramework Core/DbContext
with the goal of making items that are often difficult to test (e.g. repositories/dbcontexts, controllers) easier to test by streamlining the arrange part of the test.

### Getting Started
Examples below are using xUnit.

The easiest way to get started is to create a new FixtureFactory in your tests, and use 
the fixture this creates to generate a sut to test your code. FixtureFactory automatically makes an instance of
fixture that works well with most testing scenarios:
```csharp
[Fact]
public async void GetHome_ReturnsViewResult()
{
    // arrange
    Fixture fixture = new FixtureFactory()
                .WithDefaults()
                .Create();

    // auto injects any dependencies for home controller with automocking
    var sut = _fixture.Create<HomeController>();

    // act
    var result = await sut.Home();

    // assert
    Assert.NotNull(result);
    Assert.IsType<ViewResult>(result);
}
```
You can easily add an in memory version of dbcontext to the fixture, either when building in the factory, or later:
```csharp
// An example setting some fake data into the context and then injecting it into the fixture
[Fact]
public async void GetAllProducts_ReturnsAllProducts()
{
    // arrange
    var context = new DbContextTestBuilder<ProductDbContext>()
        .WithRandomRecords<Product>(5)
        .Build();

    Fixture fixture = new FixtureFactory()
                // you can also pass empty which will initialize context with no data
                .WithDbContext(context)
                .WithDefaults()
                .Create();

    // if you wanted to maniuplate the context with random data/fixtures after creating initial fixture, 
    // could register it this way as well
    //_fixture.RegisterDbContext(context);

    // dbcontext will be auto injected
    var sut = _fixture.Create<ProductService>();

    // act
    var products = await sut.GetAll();

    // assert
    Assert.NotNull(products);
    Assert.True(products.Count() == 5);
}
```
There are some extension methods when bulding your dbcontext to make it easier to manipulate,
just be aware there are some limitatons with in memory dbcontext that can cause errors if you aren't careful
about duplicate key generation:
```csharp
var context = new DbContextTestBuilder<ProductDbContext>()
    .WithRandomRecords<Product>(5) // add x number of completely random records
    .WithRecords(new List<Data>()) // add your own list of random data
    .WithRecord(item) // add a single instance of your own data
    .Build();
```
There are also several other items that make it easier to work with typical testing scenarios within Asp.Net Core 
controllers, inlcuding an option to include UserManger and SigninManager stubs with fixtures built from FixtureFactory, 
and a configuration builder that simulates settings from an appsettings.json.



Pull requests welcome! Happy Testing!