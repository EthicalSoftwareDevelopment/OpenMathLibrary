# C# Coding Standards

This document defines the coding standards for the C# projects in `TheOpenMathLibrary`.

It is based on common C# and .NET best practices, while also reflecting the current structure of this repository:

- `TheOpenMathLibrary.ActuarialCalculators`
- `TheOpenMathLibrary.Engineering`
- `TheOpenMathLibrary.GeneralMathematics`

All projects currently target `.NET 8.0` with nullable reference types enabled.

## 1. General Principles

- Prefer clarity over cleverness.
- Keep methods small, readable, and focused on one responsibility.
- Use descriptive names for classes, methods, parameters, and local variables.
- Avoid duplicate logic; extract shared logic when it improves maintainability.
- Favor correctness and readability over premature optimization.
- Write code that is easy to test.

## 2. Project and File Organization

- Keep each class in its own file.
- Match the file name to the primary class name.
- Group code by domain and project responsibility.
- Do not place unrelated formulas in the same class simply for convenience.
- Keep public APIs stable and predictable.

### Recommended project boundaries
- `TheOpenMathLibrary.ActuarialCalculators`: actuarial, arithmetic, numerical methods, special functions
- `TheOpenMathLibrary.Engineering`: engineering and physics-oriented formulas
- `TheOpenMathLibrary.GeneralMathematics`: foundational algebra, geometry, trigonometry, and wave theory

## 3. Namespaces

- Use consistent namespaces that match the project and folder intent.
- Prefer file-scoped or block-scoped namespaces consistently within a project.
- Avoid mixing unrelated namespaces such as `ActuarialCalculators` inside general mathematics code.

### Preferred pattern
- `TheOpenMathLibrary.ActuarialCalculators`
- `TheOpenMathLibrary.Engineering`
- `TheOpenMathLibrary.GeneralMathematics`

## 4. Naming Conventions

### Types and Members
- Use `PascalCase` for:
  - classes
  - methods
  - properties
  - constants
  - public fields, if any are ever introduced

### Parameters and Locals
- Use `camelCase` for:
  - method parameters
  - local variables
  - private fields

### Booleans
- Use names that read naturally as true/false values.

Examples:
- `isValid`
- `hasConverged`
- `canCompute`

### Acronyms
- Use standard .NET naming style for acronyms.
- Prefer `XmlDocument` over `XMLDocument`.
- Keep method and class names readable.

## 5. Method Design

- Prefer expression-bodied members only when they improve readability.
- Keep formulas direct and easy to verify.
- Avoid unnecessary temporary variables when a direct return is clearer.
- Validate inputs for public methods when invalid values would produce undefined or misleading results.
- Throw meaningful exceptions for invalid arguments.

### Prefer
```csharp
public static double Square(double value)
{
    return value * value;
}
```

### Avoid unnecessary assignments
```csharp
public static double Square(double value)
{
    double result = 0;
    result = value * value;
    return result;
}
```

## 6. Argument Validation

Public methods should validate inputs when needed.

Examples:
- reject negative values where only positive values make sense
- guard against division by zero
- guard against invalid tolerances in numerical methods
- validate iteration counts and bounds

### Example
```csharp
public static double Divide(double numerator, double denominator)
{
    if (denominator == 0)
    {
        throw new ArgumentException("Denominator must not be zero.", nameof(denominator));
    }

    return numerator / denominator;
}
```

## 7. Exceptions

- Throw the most specific exception type possible.
- Use `ArgumentException`, `ArgumentOutOfRangeException`, or `InvalidOperationException` when appropriate.
- Include clear exception messages.
- When relevant, include the parameter name with `nameof(...)`.
- Do not swallow exceptions silently.

## 8. Nullability

Nullable reference types are enabled in these projects.

- Respect nullable annotations.
- Avoid using `!` unless there is a strong and well-understood reason.
- Make null handling explicit in public APIs.
- Prefer returning non-null values whenever practical.

## 9. Using Directives

- Remove unused `using` directives.
- Keep `using` directives at the top of the file.
- Do not add broad imports that are not needed.
- Rely on implicit usings only where they improve clarity.

## 10. XML Documentation

Public classes and methods should include XML documentation comments.

At minimum, document:
- what the method calculates
- the meaning and expected units of parameters
- what the method returns
- exceptions thrown for invalid input
- assumptions or limitations of the formula

### Example
```csharp
/// <summary>
/// Calculates the wavelength from wave speed and frequency.
/// </summary>
/// <param name="waveSpeed">The propagation speed of the wave.</param>
/// <param name="frequency">The wave frequency in hertz.</param>
/// <returns>The wavelength.</returns>
/// <exception cref="ArgumentOutOfRangeException">
/// Thrown when frequency is less than or equal to zero.
/// </exception>
```

## 11. Mathematical Code Guidelines

- Prefer formulas that are easy to audit.
- Use `Math` APIs from the standard library where appropriate.
- Avoid magic numbers when a named constant or `Math.PI` improves clarity.
- Document domain assumptions and units.
- Be explicit about numerical limitations such as convergence, precision, and valid input ranges.
- For iterative algorithms, document:
  - tolerance expectations
  - stopping conditions
  - maximum iteration behavior
  - failure cases

### Recommended practices
- Prefer `Math.PI` over hard-coded approximations like `3.14159`
- Validate parameters before division or square root operations
- Keep numerical methods deterministic and well-bounded

## 12. Static Library Design

Most code in this repository currently uses utility-style static classes and methods.

- Use `static` methods only when no instance state is required.
- Keep static utility classes cohesive.
- Do not turn a class into a miscellaneous catch-all.
- If state, configuration, or extensibility becomes necessary, prefer a dedicated type over overloading utility classes.

## 13. Formatting

- Use consistent indentation and brace style throughout a file.
- Use one blank line between methods.
- Avoid excessive vertical whitespace.
- Keep line lengths reasonable for readability.
- Preserve the existing formatting style of the project when making localized edits.

## 14. Testing Expectations

Although the repository does not yet contain a dedicated C# test project, new code should be written with testing in mind.

- Add unit tests for new public methods when a test project is introduced.
- Cover normal cases, edge cases, and invalid inputs.
- Add regression tests for bugs that are fixed.
- Numerical methods should include tolerance-based assertions rather than exact equality when appropriate.

## 15. Preferred Patterns for This Repository

### Do
- use consistent project-aligned namespaces
- write clear mathematical APIs
- validate inputs for formulas and numerical methods
- use XML documentation on public APIs
- remove unused `using` directives
- return expressions directly when intermediate variables add no value

### Avoid
- inconsistent namespaces across files
- unnecessary local variables initialized only to be reassigned
- undocumented formulas
- hidden assumptions about units or valid ranges
- silent failure behavior
- copy-paste duplication across projects

## 16. Review Checklist

Before committing C# changes, check the following:

- Does the namespace match the project?
- Is the class in the correct project?
- Are names descriptive and consistent?
- Are unused `using` directives removed?
- Are invalid inputs validated?
- Are exception messages clear?
- Are XML docs present for public APIs?
- Is the formula readable and correct?
- Would the code be easy to unit test?
- Does the project build successfully?

## 17. Future Enhancements

This standards file can be expanded later with:

- `.editorconfig` rules
- Roslyn analyzer guidance
- test project conventions
- performance profiling guidance
- NuGet packaging and versioning standards

---

When in doubt, prefer the option that makes the code easier to read, easier to verify, and safer to maintain.

