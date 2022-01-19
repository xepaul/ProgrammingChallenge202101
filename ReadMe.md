#Programming Challenge: Price Calculator for Shopping Basket
##Expectation
- Produce working, **object-oriented** source code to solve the following problems
- Should be send back in electronic format as a complete project with related unit tests
- We will walk through your code together in the next session, answering questions on the code
  and programming/design choices you made.
##Description
Write a program and associated unit tests that can price a basket of goods taking into account some special offers.

The goods that can be purchased, together with their normal prices are:
- Beans – 65p per can
- Bread – 80p per loaf
- Milk – £1.30 per bottle
- Apples – £1.00 per bag

Current special offers:
- Apples have a 10% discount off their normal price **this week**
- Buy 2 cans of Bean and get a loaf of bread for half price
  
The program should accept a list of items in the basket and output the subtotal, the special offer discounts and the final price.
  Input should be via the command line in the form 
```
PriceCalculator item1 item2 item3 ...
```
  For example:
```
PriceCalculator Apple Milk Bread
```
Output should be to the console, for example:
```
Subtotal: £3.10
Apples 10% off: -10p
Total: £3.00
```
If no special offers are applicable the code should output:
```
Subtotal: £1.30
(No offers available)
Total price: £1.30
```
The code and design should meet these requirements, but be sufficiently flexible to allow future changes to the product list and/or discounts applied.

The codes should be well structured, commented, have error handling and be tested

Example of running via dotnet command line from the PriceCalculator project directory
```
dotnet run --project ./PriceCalculator Apple Milk Bread
```
```
Subtotal: £3.10
Apples 10 % off: -10p
Total: £3.00
```

```
❯ dotnet run --project ./PriceCalculator Apple Milk Bred
```
```
Subtotal: £2.30
Apples 10 % off: -10p
Total: £2.20

Unknown products: bred
```
Unit testing

```
dotnet test -v normal
```
```
Microsoft (R) Test Execution Command Line Tool Version 16.9.4
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.4.3+1b45f5407b (64-bit .NET 5.0.5)
[xUnit.net 00:00:00.48]   Discovering: PriceCalculatorTests
[xUnit.net 00:00:00.56]   Discovered:  PriceCalculatorTests
[xUnit.net 00:00:00.56]   Starting:    PriceCalculatorTests
  Passed PriceCalculatorTests.Infrastructure.OptionExtensionsTests.TesFoldWithSome [18 ms]
  Passed PriceCalculatorTests.Infrastructure.OptionTests.TestOptionIsSomeTrue [17 ms]
  Passed PriceCalculatorTests.Infrastructure.OptionTTests.TestOptionEquality [16 ms]
  Passed PriceCalculatorTests.Infrastructure.OptionTests.TestOptionDataTypeIsSomeTrue [1 ms]
  Passed PriceCalculatorTests.Infrastructure.OptionTTests.TestOptionIsSomeFalse [5 ms]
  Passed PriceCalculatorTests.Infrastructure.OptionExtensionsTests.TesFoldWithNone [6 ms]
  Passed PriceCalculatorTests.Infrastructure.OptionTests.TestOptionEquality [1 ms]
  Passed PriceCalculatorTests.Infrastructure.OptionTTests.TestOptionIsSomeTrue [< 1 ms]
  Passed PriceCalculatorTests.Infrastructure.OptionTests.TestOptionIsSomeFalse [2 ms]
  Passed PriceCalculatorTests.Infrastructure.EnumerableExtensionsTests.TestPartitionEithers [81 ms]
  Passed PriceCalculatorTests.Core.ShoppingPriceCalculatorTests.TestPrintNoOffers [84 ms]
  Passed PriceCalculatorTests.Core.DiscountRules.DependentProductDiscountRuleTests.TestDependentProductDiscountRule [138 ms]
[xUnit.net 00:00:01.03]   Finished:    PriceCalculatorTests
  Passed PriceCalculatorTests.Core.DiscountRules.ProductDiscountRuleTests.TestDependentProductDiscountRule [138 ms]
  Passed PriceCalculatorTests.Core.DiscountRuleAggregatorTests.TestPrintAppleOfferBeansOffer [149 ms]
  Passed PriceCalculatorTests.Core.ShoppingPriceCalculatorTests.TestPrintAppleOffer [210 ms]
  Passed PriceCalculatorTests.Core.ShoppingPriceCalculatorTests.TestPrintAppleWithUknowns [14 ms]

Test Run Successful.
Total tests: 16
     Passed: 16
 Total time: 2.3095 Seconds

Build succeeded.
    0 Warning(s)
    0 Error(s)
```

