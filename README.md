# FilterChili
[![License: LGPL v3](https://img.shields.io/badge/License-LGPL%20v3-blue.svg)](https://www.gnu.org/licenses/lgpl-3.0)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/FilterChili.svg)](https://www.nuget.org/packages/FilterChili)
[<img src="https://gravityctrl.visualstudio.com/_apis/public/build/definitions/9e2dfce5-384e-48bb-94c8-08a393f23f51/2/badge"/>](https://gravityctrl.visualstudio.com/FilterChili/_build/index?definitionId=2)
[![Twitter URL](https://img.shields.io/twitter/url/http/shields.io.svg?style=social)](https://twitter.com/FilterChili)

Aims to provide an easy way to create a **collection of filters** inside **.NET Core** for a set of entities, and to provide **context-sensitive** information when **data-constraints** are being applied.

## Getting Started

FilterChili is available as a NuGet package. You can install it using the NuGet Package Console window:

```
PM> Install-Package FilterChili -Version 1.0.0-rc1
```

Alternatively you can install it using the .Net CLI using this command:

```
> dotnet add package FilterChili --version 1.0.0-rc1
```

## Usage

This is a first list of features and we intend to provide further documentation later on.

### Creating a FilterContext

Just derive from `FilterContext` and specify the model, which will be used to define the filters. You then just have to implement the abstract `Configure` method.

```csharp
public class ProductFilterContext : FilterContext<Product>
{
    public ProductFilterContext(IQueryable<Product> queryable) : base(queryable) {}

    protected override void Configure(ContextOptions<Product> options)
    {
        options.Filter(product => product.Category)
               .With(domain => domain.List("Category"));

        options.Filter(product => product.Rating)
               .With(domain => domain.Range("Rating"));

        options.Filter(product => product.NumberOfReviews)
               .With(domain => domain.GreaterThanOrEqual("NumberOfReviews"));
    }
}
```

What does this do? The `Configure` method is used to define all the filters, that shall be available when filtering `Product` entities.

```csharp
options.Filter(product => product.Category)
       .With(domain => domain.List("Category"));
```

With the first statement inside the `Configure` method we define a filter based on the `Product.Category` property. Since the `Category` property is a string, we can use `List` as filter domain. The first parameter of `domain.List(...)` specifies the name, with which the filter can be identified later on.

```csharp
options.Filter(product => product.Rating)
       .With(domain => domain.Range("Rating"));
```

The second statement defines a filter for the `Product.Rating` property. Since it stores `int` values, we can use a variety of filters, that can compare numbers. In this case we decided to filter it by using a `Range` of values. The third statement therefore defines a `GreaterThanOrEqual` filter for the `NumberOfReviews` property.

### Using the FilterContext

The code snipped below shows, how easy it is to use the filters.

```csharp
// Initialize the DbContext.
using (var dataContext = new AppDbContext())
{
    // Create an instance of the FilterContext.
    var filterContext = new ProductFilterContext(dataContext.Products);

    // Set the product categories, that shall be found.
    filterContext.TrySet("Category", new[] { "Books", "Magazines", "Newspapers" });

    // Specify the which ratings shall be accepted.
    filterContext.TrySet("Rating", min: 3, max: 8);

    // Ensure that at least 10 user reviews were written for this product.
    filterContext.TrySet("NumberOfReviews", value: 10);

    // Find out how the filters have influenced each other.
    var domains = await filterContext.Domains();
    Debug.WriteLine(JsonConvert.SerializeObject(domains))

    // Retrieve the filtered Queryable.
    var result = filterContext.ApplyFilters();

    // Apply further LINQ statements to the result.
    return await result.OrderBy(product => product.Name).Take(20).ToListAsync();
}
```

### FilterContext Tip:

In the derived `FilterContext` class, feel encouraged to create properties for each filter.

```csharp
public StringListResolver<Product> CategoryFilter { get; private set; }

protected override void Configure(ContextOptions<Product> options)
{
    CategoryFilter = options
        .Filter(product => product.Category)
        .With(domain => domain.List("Category"));

    [...]
}
```

This allows for using the direct `Set` methods:

```csharp
var filterContext = new ProductFilterContext(dataContext.Products);

// Set the product categories, that shall be found.
filterContext.CategoryFilter.Set("Books", "Magazines", "Newspapers");
```

## Future Plans

We are currently creating a demo page which will demonstrate the capabilities of this project in more detail.

## Contributing

We'd like to invite you to contribute to this project. Especially the documentation is currently lacking behind.
You can also help us with features as well. We are looking forward to find a way, how to integrate proper `enum` support.

## Authors

See [Contributors](https://github.com/gravityctrl/FilterChili/contributors)

## License

Copyright © 2017 Sebastian Krogull.

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with this program.  If not, see [http://www.gnu.org/licenses/](http://www.gnu.org/licenses).

Legal
------

By submitting a Pull Request, you disavow any rights or claims to any changes submitted to the FilterChili project and assign the copyright of those changes to Sebastian Krogull.

If you cannot or do not want to reassign those rights (your employment contract for your employer may not allow this), you should not submit a PR. Open an issue and someone else can do the work.

This is a legal way of saying "If you submit a PR to us, that code becomes ours". 99.9% of the time that's what you intend anyways; we hope it doesn't scare you away from contributing.
