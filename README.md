# What is this?

A project seed for a C# dotnet API ("PaylocityBenefitsCalculator").  It is meant to get you started on the Paylocity BackEnd Coding Challenge by taking some initial setup decisions away.

The goal is to respect your time, avoid live coding, and get a sense for how you work.

# Coding Challenge

**Show us how you work.**

Each of our Paylocity product teams operates like a small startup, empowered to deliver business value in
whatever way they see fit. Because our teams are close knit and fast moving it is imperative that you are able
to work collaboratively with your fellow developers. 

This coding challenge is designed to allow you to demonstrate your abilities and discuss your approach to
design and implementation with your potential colleagues. You are free to use whatever technologies you
prefer but please be prepared to discuss the choices you’ve made. We encourage you to focus on creating a
logical and functional solution rather than one that is completely polished and ready for production.

The challenge can be used as a canvas to capture your strengths in addition to reflecting your overall coding
standards and approach. There’s no right or wrong answer.  It’s more about how you think through the
problem. We’re looking to see your skills in all three tiers so the solution can be used as a conversation piece
to show our teams your abilities across the board.

Requirements will be given separately.

# Solution

## Environment

The challenge was updated to .NET 8.0 as mentioned by the interviewer.

## Test Environment

The test environment uses the MVC testing framework, allowing both unit and integration tests to be executed directly from the IDE. This approach enables proper mocking of external dependencies (such as configuration stores, databases, and other services) using libraries like WireMock.NET. It also avoids the need for manual service startup or port management.

## Controller Functionality

The solution decouples controllers from the data access layer, enabling easier unit testing and better separation of concerns. The provided implementations for employees and dependents are in-memory mocks, using the initial data from the challenge. In a real-world scenario, these would call REST API endpoints with proper authentication and authorization.

## Mapping and Validation

AutoMapper is used for mapping between DTOs and domain models, reducing boilerplate code. Note that AutoMapper is licensed and may not be free for commercial use; in production, alternatives or manual mapping should be considered and benchmarked. Roslyn code generators may offer the best performance and maintainability for mapping in large-scale systems.

## Running the Solution

You can run the solution using `dotnet run` in the root directory of the `API` project or from your IDE. The easiest way to interact with the API is via Swagger UI, available at `http://localhost:7124/swagger/index.html` by default.

Endpoints can also be called using any HTTP client (e.g., Postman or curl). The main calculator endpoint is:

`/api/v1/Paychecks/User/{userId}/From/{startDate}/Periodicity/{periodicity}`

- `userId`: ID of the employee
- `startDate`: Start date of the pay period (YYYY-MM-DD)
- `periodicity`: Paycheck periodicity (currently only `0` for `BiWeekly` is supported)

## Paycheck Calculation

The challenge requirements are implemented as follows:

- 26 paychecks per year, with deductions spread as evenly as possible.
- Employees have a base benefits cost of $1,000 per month.
- Each dependent adds $600 per month to the benefits cost.
- Employees earning more than $80,000 per year incur an additional 2% of their annual salary in benefits costs.
- Dependents over 50 years old incur an additional $200 per month.

**Assumptions:**  
Requirements were not clarified more so we are taking liberty of interpretation for demonstration purposes. The bi-weekly pay schedule is assumed to follow the standard U.S. model (26-27 paychecks per year). The rules are for demonstration and may not match legal requirements.

### Calculator Architecture

The core calculator implements the `ICalculatorModel` interface (specifically, `BiWeeklyCalculatorModel`). The model applies a set of rules (`IPaycheckRule`), each of which may use an applicability policy (`IApplicabilityPolicy`) and a proration policy (`IProrationPolicy`). This allows for flexible, composable business logic.

- **Applicability policies** determine if a rule applies to a given employee or dependent for a specific period.
- **Proration policies** determine what portion of the period the rule applies to.

Rules and policies are currently hardcoded for demonstration. In production, these would be configurable to support different legal, company, or country-specific requirements, and to adapt to changes over time.

### Time Boundaries Handling

Bi-weekly pay periods may cross month or year boundaries. In such cases, calculations are split:  
- The first part covers the period from the start date to the end of the month/year.
- The second part covers the start of the next month/year to the end date.

This ensures correct proration of monthly and yearly costs, e.g., $1,000 per month is distributed according to the number of days in each month.

### Known Issues

- **Rounding:** The solution does not handle sub-cent rounding residuals. In production, residuals would be tracked and settled in subsequent paychecks ("penny settlement"), which requires persistent storage.
- **Currency:** The solution assumes USD; currency is not configurable.
- **Input Validation:** The solution expects valid input (e.g., start date before end date). Additional validation should be added for production.
- **Period Alignment:** The solution relies on the caller to provide a valid start date for the pay period; it does not align periods automatically.