// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.FunctionalTests.TestUtilities.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Data.Entity.SqlServer.FunctionalTests
{
    public class IncludeSqlServerTest : IncludeTestBase<NorthwindQuerySqlServerFixture>
    {
        public IncludeSqlServerTest(NorthwindQuerySqlServerFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            //TestSqlLoggerFactory.CaptureOutput(testOutputHelper);
        }

        public override void Include_list()
        {
            base.Include_list();
            
            Assert.Equal(
                @"SELECT [c].[ProductID], [c].[Discontinued], [c].[ProductName], [c].[UnitsInStock]
FROM [Products] AS [c]
ORDER BY [c].[ProductID]

SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice], [o0].[OrderID], [o0].[CustomerID], [o0].[EmployeeID], [o0].[OrderDate]
FROM [Order Details] AS [o]
INNER JOIN (
    SELECT DISTINCT [c].[ProductID]
    FROM [Products] AS [c]
) AS [c] ON [o].[ProductID] = [c].[ProductID]
INNER JOIN [Orders] AS [o0] ON [o].[OrderID] = [o0].[OrderID]
ORDER BY [c].[ProductID]",
                Sql);
        }

        public override void Include_collection()
        {
            base.Include_collection();

            Assert.Equal(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
ORDER BY [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [c].[CustomerID]
    FROM [Customers] AS [c]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_reference_and_collection()
        {
            base.Include_reference_and_collection();

            Assert.Equal(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Orders] AS [o]
LEFT JOIN [Customers] AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [o].[OrderID]

SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
INNER JOIN (
    SELECT DISTINCT [o].[OrderID]
    FROM [Orders] AS [o]
) AS [o0] ON [o].[OrderID] = [o0].[OrderID]
ORDER BY [o0].[OrderID]",
                Sql);
        }

        public override void Include_references_multi_level()
        {
            base.Include_references_multi_level();

            Assert.Equal(
                @"SELECT [od].[OrderID], [od].[ProductID], [od].[Discount], [od].[Quantity], [od].[UnitPrice], [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Order Details] AS [od]
INNER JOIN [Orders] AS [o] ON [od].[OrderID] = [o].[OrderID]
LEFT JOIN [Customers] AS [c] ON [o].[CustomerID] = [c].[CustomerID]",
                Sql);
        }

        public override void Include_multiple_references_multi_level()
        {
            base.Include_multiple_references_multi_level();

            Assert.Equal(
                @"SELECT [od].[OrderID], [od].[ProductID], [od].[Discount], [od].[Quantity], [od].[UnitPrice], [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region], [p].[ProductID], [p].[Discontinued], [p].[ProductName], [p].[UnitsInStock]
FROM [Order Details] AS [od]
INNER JOIN [Orders] AS [o] ON [od].[OrderID] = [o].[OrderID]
LEFT JOIN [Customers] AS [c] ON [o].[CustomerID] = [c].[CustomerID]
INNER JOIN [Products] AS [p] ON [od].[ProductID] = [p].[ProductID]",
                Sql);
        }

        public override void Include_multiple_references_multi_level_reverse()
        {
            base.Include_multiple_references_multi_level_reverse();

            Assert.Equal(
                @"SELECT [od].[OrderID], [od].[ProductID], [od].[Discount], [od].[Quantity], [od].[UnitPrice], [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region], [p].[ProductID], [p].[Discontinued], [p].[ProductName], [p].[UnitsInStock]
FROM [Order Details] AS [od]
INNER JOIN [Orders] AS [o] ON [od].[OrderID] = [o].[OrderID]
LEFT JOIN [Customers] AS [c] ON [o].[CustomerID] = [c].[CustomerID]
INNER JOIN [Products] AS [p] ON [od].[ProductID] = [p].[ProductID]",
                Sql);
        }

        public override void Include_references_and_collection_multi_level()
        {
            base.Include_references_and_collection_multi_level();

            Assert.Equal(
                @"SELECT [od].[OrderID], [od].[ProductID], [od].[Discount], [od].[Quantity], [od].[UnitPrice], [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Order Details] AS [od]
INNER JOIN [Orders] AS [o] ON [od].[OrderID] = [o].[OrderID]
LEFT JOIN [Customers] AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [c].[CustomerID]
    FROM [Order Details] AS [od]
    INNER JOIN [Orders] AS [o] ON [od].[OrderID] = [o].[OrderID]
    LEFT JOIN [Customers] AS [c] ON [o].[CustomerID] = [c].[CustomerID]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_multi_level_reference_and_collection_predicate()
        {
            base.Include_multi_level_reference_and_collection_predicate();

            Assert.Equal(
                @"SELECT TOP(2) [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Orders] AS [o]
LEFT JOIN [Customers] AS [c] ON [o].[CustomerID] = [c].[CustomerID]
WHERE [o].[OrderID] = 10248
ORDER BY [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT TOP(2) [c].[CustomerID]
    FROM [Orders] AS [o]
    LEFT JOIN [Customers] AS [c] ON [o].[CustomerID] = [c].[CustomerID]
    WHERE [o].[OrderID] = 10248
    ORDER BY [c].[CustomerID]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_multi_level_collection_and_then_include_reference_predicate()
        {
            base.Include_multi_level_collection_and_then_include_reference_predicate();

            Assert.Equal(
                @"SELECT TOP(2) [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE [o].[OrderID] = 10248
ORDER BY [o].[OrderID]

SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice], [p].[ProductID], [p].[Discontinued], [p].[ProductName], [p].[UnitsInStock]
FROM [Order Details] AS [o]
INNER JOIN (
    SELECT DISTINCT TOP(2) [o].[OrderID]
    FROM [Orders] AS [o]
    WHERE [o].[OrderID] = 10248
    ORDER BY [o].[OrderID]
) AS [o0] ON [o].[OrderID] = [o0].[OrderID]
INNER JOIN [Products] AS [p] ON [o].[ProductID] = [p].[ProductID]
ORDER BY [o0].[OrderID]",
                Sql);
        }

        public override void Include_collection_alias_generation()
        {
            base.Include_collection_alias_generation();

            Assert.Equal(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
ORDER BY [o].[OrderID]

SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
INNER JOIN (
    SELECT DISTINCT [o].[OrderID]
    FROM [Orders] AS [o]
) AS [o0] ON [o].[OrderID] = [o0].[OrderID]
ORDER BY [o0].[OrderID]",
                Sql);
        }

        public override void Include_collection_order_by_key()
        {
            base.Include_collection_order_by_key();

            Assert.Equal(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
ORDER BY [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [c].[CustomerID]
    FROM [Customers] AS [c]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_collection_order_by_non_key()
        {
            base.Include_collection_order_by_non_key();

            Assert.Equal(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
ORDER BY [c].[City], [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [c].[City], [c].[CustomerID]
    FROM [Customers] AS [c]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[City], [c].[CustomerID]",
                Sql);
        }

        public override void Include_collection_as_no_tracking()
        {
            base.Include_collection_as_no_tracking();

            Assert.Equal(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
ORDER BY [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [c].[CustomerID]
    FROM [Customers] AS [c]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_collection_principal_already_tracked()
        {
            base.Include_collection_principal_already_tracked();

            Assert.Equal(
                @"SELECT TOP(2) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[CustomerID] = 'ALFKI'

SELECT TOP(2) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[CustomerID] = 'ALFKI'
ORDER BY [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT TOP(2) [c].[CustomerID]
    FROM [Customers] AS [c]
    WHERE [c].[CustomerID] = 'ALFKI'
    ORDER BY [c].[CustomerID]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_collection_principal_already_tracked_as_no_tracking()
        {
            base.Include_collection_principal_already_tracked_as_no_tracking();

            Assert.Equal(
                @"SELECT TOP(2) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[CustomerID] = 'ALFKI'

SELECT TOP(2) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[CustomerID] = 'ALFKI'
ORDER BY [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT TOP(2) [c].[CustomerID]
    FROM [Customers] AS [c]
    WHERE [c].[CustomerID] = 'ALFKI'
    ORDER BY [c].[CustomerID]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_collection_with_filter()
        {
            base.Include_collection_with_filter();

            Assert.Equal(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[CustomerID] = 'ALFKI'
ORDER BY [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [c].[CustomerID]
    FROM [Customers] AS [c]
    WHERE [c].[CustomerID] = 'ALFKI'
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_collection_with_filter_reordered()
        {
            base.Include_collection_with_filter_reordered();

            Assert.Equal(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[CustomerID] = 'ALFKI'
ORDER BY [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [c].[CustomerID]
    FROM [Customers] AS [c]
    WHERE [c].[CustomerID] = 'ALFKI'
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_collection_then_include_collection()
        {
            base.Include_collection_then_include_collection();

            Assert.Equal(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
ORDER BY [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [c].[CustomerID]
    FROM [Customers] AS [c]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID], [o].[OrderID]

SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
INNER JOIN (
    SELECT DISTINCT [c].[CustomerID], [o].[OrderID]
    FROM [Orders] AS [o]
    INNER JOIN (
        SELECT DISTINCT [c].[CustomerID]
        FROM [Customers] AS [c]
    ) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
) AS [o0] ON [o].[OrderID] = [o0].[OrderID]
ORDER BY [o0].[CustomerID], [o0].[OrderID]",
                Sql);
        }

        public override void Include_collection_when_projection()
        {
            base.Include_collection_when_projection();

            Assert.Equal(
                @"SELECT [c].[CustomerID]
FROM [Customers] AS [c]",
                Sql);
        }

        public override void Include_collection_on_join_clause_with_filter()
        {
            base.Include_collection_on_join_clause_with_filter();

            Assert.Equal(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
INNER JOIN [Orders] AS [o] ON [c].[CustomerID] = [o].[CustomerID]
WHERE [c].[CustomerID] = 'ALFKI'
ORDER BY [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [c].[CustomerID]
    FROM [Customers] AS [c]
    INNER JOIN [Orders] AS [o] ON [c].[CustomerID] = [o].[CustomerID]
    WHERE [c].[CustomerID] = 'ALFKI'
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_collection_on_additional_from_clause_with_filter()
        {
            base.Include_collection_on_additional_from_clause_with_filter();

            Assert.Equal(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c1]
CROSS JOIN [Customers] AS [c]
WHERE [c].[CustomerID] = 'ALFKI'
ORDER BY [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [c].[CustomerID]
    FROM [Customers] AS [c1]
    CROSS JOIN [Customers] AS [c]
    WHERE [c].[CustomerID] = 'ALFKI'
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_collection_on_additional_from_clause()
        {
            base.Include_collection_on_additional_from_clause();

            Assert.Equal(
                @"@__p_0: 5

SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM (
    SELECT TOP(@__p_0) [c].*
    FROM [Customers] AS [c]
    ORDER BY [c].[CustomerID]
) AS [t0]
CROSS JOIN [Customers] AS [c]
ORDER BY [c].[CustomerID]

@__p_0: 5

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [c].[CustomerID]
    FROM (
        SELECT TOP(@__p_0) [c].*
        FROM [Customers] AS [c]
        ORDER BY [c].[CustomerID]
    ) AS [t0]
    CROSS JOIN [Customers] AS [c]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_duplicate_collection()
        {
            base.Include_duplicate_collection();

            if (TestEnvironment.GetFlag(nameof(SqlServerCondition.SupportsOffset)) ?? true)
            {
                Assert.Equal(
                    @"@__p_0: 2

SELECT [t0].[CustomerID], [t0].[Address], [t0].[City], [t0].[CompanyName], [t0].[ContactName], [t0].[ContactTitle], [t0].[Country], [t0].[Fax], [t0].[Phone], [t0].[PostalCode], [t0].[Region], [t1].[CustomerID], [t1].[Address], [t1].[City], [t1].[CompanyName], [t1].[ContactName], [t1].[ContactTitle], [t1].[Country], [t1].[Fax], [t1].[Phone], [t1].[PostalCode], [t1].[Region]
FROM (
    SELECT TOP(@__p_0) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
    FROM [Customers] AS [c]
    ORDER BY [c].[CustomerID]
) AS [t0]
CROSS JOIN (
    SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
    FROM [Customers] AS [c]
    ORDER BY [c].[CustomerID]
    OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY
) AS [t1]
ORDER BY [t0].[CustomerID], [t1].[CustomerID]

@__p_0: 2

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [t0].[CustomerID], [t1].[CustomerID] AS [CustomerID0]
    FROM (
        SELECT TOP(@__p_0) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
        FROM [Customers] AS [c]
        ORDER BY [c].[CustomerID]
    ) AS [t0]
    CROSS JOIN (
        SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
        FROM [Customers] AS [c]
        ORDER BY [c].[CustomerID]
        OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY
    ) AS [t1]
) AS [t1] ON [o].[CustomerID] = [t1].[CustomerID0]
ORDER BY [t1].[CustomerID], [t1].[CustomerID0]

@__p_0: 2

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [t0].[CustomerID]
    FROM (
        SELECT TOP(@__p_0) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
        FROM [Customers] AS [c]
        ORDER BY [c].[CustomerID]
    ) AS [t0]
    CROSS JOIN (
        SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
        FROM [Customers] AS [c]
        ORDER BY [c].[CustomerID]
        OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY
    ) AS [t1]
) AS [t0] ON [o].[CustomerID] = [t0].[CustomerID]
ORDER BY [t0].[CustomerID]",
                    Sql);
            }
        }

        public override void Include_duplicate_collection_result_operator()
        {
            base.Include_duplicate_collection_result_operator();

            if (TestEnvironment.GetFlag(nameof(SqlServerCondition.SupportsOffset)) ?? true)
            {
                Assert.Equal(
                    @"@__p_1: 1
@__p_0: 2

SELECT TOP(@__p_1) [t0].[CustomerID], [t0].[Address], [t0].[City], [t0].[CompanyName], [t0].[ContactName], [t0].[ContactTitle], [t0].[Country], [t0].[Fax], [t0].[Phone], [t0].[PostalCode], [t0].[Region], [t1].[CustomerID], [t1].[Address], [t1].[City], [t1].[CompanyName], [t1].[ContactName], [t1].[ContactTitle], [t1].[Country], [t1].[Fax], [t1].[Phone], [t1].[PostalCode], [t1].[Region]
FROM (
    SELECT TOP(@__p_0) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
    FROM [Customers] AS [c]
    ORDER BY [c].[CustomerID]
) AS [t0]
CROSS JOIN (
    SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
    FROM [Customers] AS [c]
    ORDER BY [c].[CustomerID]
    OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY
) AS [t1]
ORDER BY [t0].[CustomerID], [t1].[CustomerID]

@__p_1: 1
@__p_0: 2

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT TOP(@__p_1) [t0].[CustomerID], [t1].[CustomerID] AS [CustomerID0]
    FROM (
        SELECT TOP(@__p_0) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
        FROM [Customers] AS [c]
        ORDER BY [c].[CustomerID]
    ) AS [t0]
    CROSS JOIN (
        SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
        FROM [Customers] AS [c]
        ORDER BY [c].[CustomerID]
        OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY
    ) AS [t1]
    ORDER BY [t0].[CustomerID], [t1].[CustomerID]
) AS [t1] ON [o].[CustomerID] = [t1].[CustomerID0]
ORDER BY [t1].[CustomerID], [t1].[CustomerID0]

@__p_1: 1
@__p_0: 2

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT TOP(@__p_1) [t0].[CustomerID]
    FROM (
        SELECT TOP(@__p_0) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
        FROM [Customers] AS [c]
        ORDER BY [c].[CustomerID]
    ) AS [t0]
    CROSS JOIN (
        SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
        FROM [Customers] AS [c]
        ORDER BY [c].[CustomerID]
        OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY
    ) AS [t1]
    ORDER BY [t0].[CustomerID]
) AS [t0] ON [o].[CustomerID] = [t0].[CustomerID]
ORDER BY [t0].[CustomerID]",
                    Sql);
            }
        }

        public override void Include_collection_on_join_clause_with_order_by_and_filter()
        {
            base.Include_collection_on_join_clause_with_order_by_and_filter();

            Assert.Equal(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
INNER JOIN [Orders] AS [o] ON [c].[CustomerID] = [o].[CustomerID]
WHERE [c].[CustomerID] = 'ALFKI'
ORDER BY [c].[City], [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [c].[City], [c].[CustomerID]
    FROM [Customers] AS [c]
    INNER JOIN [Orders] AS [o] ON [c].[CustomerID] = [o].[CustomerID]
    WHERE [c].[CustomerID] = 'ALFKI'
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[City], [c].[CustomerID]",
                Sql);
        }

        public override void Include_collection_on_additional_from_clause2()
        {
            base.Include_collection_on_additional_from_clause2();

            Assert.Equal(
                @"@__p_0: 5

SELECT [t0].[CustomerID], [t0].[Address], [t0].[City], [t0].[CompanyName], [t0].[ContactName], [t0].[ContactTitle], [t0].[Country], [t0].[Fax], [t0].[Phone], [t0].[PostalCode], [t0].[Region]
FROM (
    SELECT TOP(@__p_0) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
    FROM [Customers] AS [c]
    ORDER BY [c].[CustomerID]
) AS [t0]
CROSS JOIN [Customers] AS [c]",
                Sql);
        }

        public override void Include_duplicate_collection_result_operator2()
        {
            base.Include_duplicate_collection_result_operator2();
            if (TestEnvironment.GetFlag(nameof(SqlServerCondition.SupportsOffset)) ?? true)
            {
                Assert.Equal(
                    @"@__p_1: 1
@__p_0: 2

SELECT TOP(@__p_1) [t0].[CustomerID], [t0].[Address], [t0].[City], [t0].[CompanyName], [t0].[ContactName], [t0].[ContactTitle], [t0].[Country], [t0].[Fax], [t0].[Phone], [t0].[PostalCode], [t0].[Region], [t1].[CustomerID], [t1].[Address], [t1].[City], [t1].[CompanyName], [t1].[ContactName], [t1].[ContactTitle], [t1].[Country], [t1].[Fax], [t1].[Phone], [t1].[PostalCode], [t1].[Region]
FROM (
    SELECT TOP(@__p_0) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
    FROM [Customers] AS [c]
    ORDER BY [c].[CustomerID]
) AS [t0]
CROSS JOIN (
    SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
    FROM [Customers] AS [c]
    ORDER BY [c].[CustomerID]
    OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY
) AS [t1]
ORDER BY [t0].[CustomerID]

@__p_1: 1
@__p_0: 2

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT TOP(@__p_1) [t0].[CustomerID]
    FROM (
        SELECT TOP(@__p_0) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
        FROM [Customers] AS [c]
        ORDER BY [c].[CustomerID]
    ) AS [t0]
    CROSS JOIN (
        SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
        FROM [Customers] AS [c]
        ORDER BY [c].[CustomerID]
        OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY
    ) AS [t1]
    ORDER BY [t0].[CustomerID]
) AS [t0] ON [o].[CustomerID] = [t0].[CustomerID]
ORDER BY [t0].[CustomerID]",
                    Sql);
            }
        }

        public override void Include_reference()
        {
            base.Include_reference();

            Assert.Equal(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Orders] AS [o]
LEFT JOIN [Customers] AS [c] ON [o].[CustomerID] = [c].[CustomerID]",
                Sql);
        }

        public override void Include_multiple_references()
        {
            base.Include_multiple_references();

            Assert.Equal(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice], [o0].[OrderID], [o0].[CustomerID], [o0].[EmployeeID], [o0].[OrderDate], [p].[ProductID], [p].[Discontinued], [p].[ProductName], [p].[UnitsInStock]
FROM [Order Details] AS [o]
INNER JOIN [Orders] AS [o0] ON [o].[OrderID] = [o0].[OrderID]
INNER JOIN [Products] AS [p] ON [o].[ProductID] = [p].[ProductID]",
                Sql);
        }

        public override void Include_reference_alias_generation()
        {
            base.Include_reference_alias_generation();

            Assert.Equal(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice], [o0].[OrderID], [o0].[CustomerID], [o0].[EmployeeID], [o0].[OrderDate]
FROM [Order Details] AS [o]
INNER JOIN [Orders] AS [o0] ON [o].[OrderID] = [o0].[OrderID]",
                Sql);
        }

        public override void Include_duplicate_reference()
        {
            base.Include_duplicate_reference();

            if (TestEnvironment.GetFlag(nameof(SqlServerCondition.SupportsOffset)) ?? true)
            {
                Assert.Equal(
                    @"@__p_0: 2

SELECT [t0].[OrderID], [t0].[CustomerID], [t0].[EmployeeID], [t0].[OrderDate], [t1].[OrderID], [t1].[CustomerID], [t1].[EmployeeID], [t1].[OrderDate], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region], [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
FROM (
    SELECT TOP(@__p_0) [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
    FROM [Orders] AS [o]
    ORDER BY [o].[CustomerID]
) AS [t0]
CROSS JOIN (
    SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
    FROM [Orders] AS [o]
    ORDER BY [o].[CustomerID]
    OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY
) AS [t1]
LEFT JOIN [Customers] AS [c] ON [t0].[CustomerID] = [c].[CustomerID]
LEFT JOIN [Customers] AS [c0] ON [t1].[CustomerID] = [c0].[CustomerID]",
                    Sql);
            }
        }

        public override void Include_duplicate_reference2()
        {
            base.Include_duplicate_reference2();

            if (TestEnvironment.GetFlag(nameof(SqlServerCondition.SupportsOffset)) ?? true)
            {
                Assert.Equal(
                    @"@__p_0: 2

SELECT [t0].[OrderID], [t0].[CustomerID], [t0].[EmployeeID], [t0].[OrderDate], [t1].[OrderID], [t1].[CustomerID], [t1].[EmployeeID], [t1].[OrderDate], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM (
    SELECT TOP(@__p_0) [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
    FROM [Orders] AS [o]
    ORDER BY [o].[OrderID]
) AS [t0]
CROSS JOIN (
    SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
    FROM [Orders] AS [o]
    ORDER BY [o].[OrderID]
    OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY
) AS [t1]
LEFT JOIN [Customers] AS [c] ON [t0].[CustomerID] = [c].[CustomerID]",
                    Sql);
            }
        }

        public override void Include_duplicate_reference3()
        {
            base.Include_duplicate_reference3();

            if (TestEnvironment.GetFlag(nameof(SqlServerCondition.SupportsOffset)) ?? true)
            {
                Assert.Equal(
                    @"@__p_0: 2

SELECT [t0].[OrderID], [t0].[CustomerID], [t0].[EmployeeID], [t0].[OrderDate], [t1].[OrderID], [t1].[CustomerID], [t1].[EmployeeID], [t1].[OrderDate], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM (
    SELECT TOP(@__p_0) [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
    FROM [Orders] AS [o]
    ORDER BY [o].[OrderID]
) AS [t0]
CROSS JOIN (
    SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
    FROM [Orders] AS [o]
    ORDER BY [o].[OrderID]
    OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY
) AS [t1]
LEFT JOIN [Customers] AS [c] ON [t1].[CustomerID] = [c].[CustomerID]",
                    Sql);
            }
        }

        public override void Include_reference_when_projection()
        {
            base.Include_reference_when_projection();

            Assert.Equal(
                @"SELECT [o].[CustomerID]
FROM [Orders] AS [o]",
                Sql);
        }

        public override void Include_reference_with_filter_reordered()
        {
            base.Include_reference_with_filter_reordered();

            Assert.Equal(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Orders] AS [o]
LEFT JOIN [Customers] AS [c] ON [o].[CustomerID] = [c].[CustomerID]
WHERE [o].[CustomerID] = 'ALFKI'",
                Sql);
        }

        public override void Include_reference_with_filter()
        {
            base.Include_reference_with_filter();

            Assert.Equal(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Orders] AS [o]
LEFT JOIN [Customers] AS [c] ON [o].[CustomerID] = [c].[CustomerID]
WHERE [o].[CustomerID] = 'ALFKI'",
                Sql);
        }

        public override void Include_collection_dependent_already_tracked_as_no_tracking()
        {
            base.Include_collection_dependent_already_tracked_as_no_tracking();

            Assert.Equal(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE [o].[CustomerID] = 'ALFKI'

SELECT TOP(2) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[CustomerID] = 'ALFKI'
ORDER BY [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT TOP(2) [c].[CustomerID]
    FROM [Customers] AS [c]
    WHERE [c].[CustomerID] = 'ALFKI'
    ORDER BY [c].[CustomerID]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_collection_dependent_already_tracked()
        {
            base.Include_collection_dependent_already_tracked();

            Assert.Equal(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE [o].[CustomerID] = 'ALFKI'

SELECT TOP(2) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[CustomerID] = 'ALFKI'
ORDER BY [c].[CustomerID]

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT TOP(2) [c].[CustomerID]
    FROM [Customers] AS [c]
    WHERE [c].[CustomerID] = 'ALFKI'
    ORDER BY [c].[CustomerID]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_reference_dependent_already_tracked()
        {
            base.Include_reference_dependent_already_tracked();

            Assert.Equal(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE [o].[CustomerID] = 'ALFKI'

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Orders] AS [o]
LEFT JOIN [Customers] AS [c] ON [o].[CustomerID] = [c].[CustomerID]",
                Sql);
        }

        public override void Include_reference_as_no_tracking()
        {
            base.Include_reference_as_no_tracking();

            Assert.Equal(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Orders] AS [o]
LEFT JOIN [Customers] AS [c] ON [o].[CustomerID] = [c].[CustomerID]",
                Sql);
        }

        public override void Include_collection_as_no_tracking2()
        {
            base.Include_collection_as_no_tracking2();

            Assert.Equal(
                @"@__p_0: 5

SELECT TOP(@__p_0) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
ORDER BY [c].[CustomerID]

@__p_0: 5

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT TOP(@__p_0) [c].[CustomerID]
    FROM [Customers] AS [c]
    ORDER BY [c].[CustomerID]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[CustomerID]",
                Sql);
        }

        public override void Include_with_take()
        {
            base.Include_with_take();

            Assert.Equal(
                @"@__p_0: 10

SELECT TOP(@__p_0) [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
ORDER BY [c].[City] DESC, [c].[CustomerID]

@__p_0: 10

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT TOP(@__p_0) [c].[City], [c].[CustomerID]
    FROM [Customers] AS [c]
    ORDER BY [c].[City] DESC, [c].[CustomerID]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[City] DESC, [c].[CustomerID]",
                Sql);
        }

        [ConditionalFact]
        [SqlServerCondition(SqlServerCondition.SupportsOffset)]
        public override void Include_with_skip()
        {
            base.Include_with_skip();

            Assert.Equal(
                @"@__p_0: 80

SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
ORDER BY [c].[ContactName], [c].[CustomerID]
OFFSET @__p_0 ROWS

@__p_0: 80

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
INNER JOIN (
    SELECT DISTINCT [t0].*
    FROM (
        SELECT [c].[ContactName], [c].[CustomerID]
        FROM [Customers] AS [c]
        ORDER BY [c].[ContactName], [c].[CustomerID]
        OFFSET @__p_0 ROWS
    ) AS [t0]
) AS [c] ON [o].[CustomerID] = [c].[CustomerID]
ORDER BY [c].[ContactName], [c].[CustomerID]",
                Sql);
        }

        private static string Sql => TestSqlLoggerFactory.Sql;
    }
}
