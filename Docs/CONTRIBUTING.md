# Contributing

## Toolchain and Validation

Use .NET SDK 10.x, PowerShell 7+, and Microsoft Testing Platform. Projects compile as C# 14 with the required SDK; the repository sets `<LangVersion>latest</LangVersion>` rather than pinning `14.0`.

To build all projects:

```powershell
dotnet build .\<Name>.slnx -c Debug
```

To run all tests:

```powershell
dotnet test --solution .\<Name>.slnx -c Debug
```

To run a specfic test project:

```powershell
dotnet test --project .\Src\<Name>.Tests\<Name>.Tests.csproj -c Debug
```

To run one single unit test:

```powershell
dotnet test --project .\Src\<Name>.Tests\<Name>.Tests.csproj -c Debug --filter-method Genbox.<Name>.Tests.<TestClass>.<TestMethod>
```

## Shared and Local Configuration

Files under `Src/` and `Imports/` provide shared template behavior. Add a same-named file under `Locals/` for repository-specific configuration.

| Shared file                    | Optional local file               |
|--------------------------------|-----------------------------------|
| `Src/Directory.Build.props`    | `Locals/Directory.Build.props`    |
| `Src/Directory.Build.targets`  | `Locals/Directory.Build.targets`  |
| `Src/Directory.Packages.props` | `Locals/Directory.Packages.props` |
| `Imports/Library.props`        | `Locals/Library.props`            |
| `Imports/Tests.props`          | `Locals/Tests.props`              |
| `Imports/Benchmarks.props`     | `Locals/Benchmarks.props`         |
| `Imports/Examples.props`       | `Locals/Examples.props`           |
| `Imports/Console.props`        | `Locals/Console.props`            |
| `Imports/Web.props`            | `Locals/Web.props`                |
| `Imports/Analyzers.props`      | `Locals/Analyzers.props`          |

Local files are imported after their corresponding shared declarations. `Locals/Directory.Build.props` remains before project-type props so `IncludeBaseProject`, `IncludeInternalsVisibleTo`, and `IncludeAnalyzers` can control the items those files add.

All repository-specific package versions, including analyzer packages, belong in `Locals/Directory.Packages.props`. Do not add them to `Src/Directory.Packages.props`; that file is centrally managed as part of the template. Use `PackageVersion Include` for a new package and `PackageVersion Update` to override a template-managed version. Add the corresponding `PackageReference` to the relevant project or local project-type props file, then restore and commit every affected `packages.lock.json`.

## Project Naming

Project suffixes select project-type configuration. Preserve the exact casing because CI runs on Linux.

- Name test projects `<BaseProject>.Tests`. This imports `Imports/Tests.props`, configures xUnit with Microsoft Testing Platform, and references `<BaseProject>` when `IncludeBaseProject` is enabled.
- Name benchmark projects `<BaseProject>.Benchmarks`. This imports `Imports/Benchmarks.props`, configures BenchmarkDotNet, and references `<BaseProject>` when `IncludeBaseProject` is enabled.
- Name ASP.NET Core projects `<BaseProject>.Web`. This imports `Imports/Web.props` and applies the ASP.NET-specific analyzer package.

The suffix is functional, not descriptive only. A different suffix does not receive the corresponding shared configuration or automatic base-project reference.

Libraries automatically expose their internals to correctly named `.Tests` and `.Benchmarks` assemblies. Do not make production types or members public solely to access them from tests or benchmarks.

Run benchmarks in Release:

```powershell
dotnet run --project .\Src\<Name>.Benchmarks\<Name>.Benchmarks.csproj -c Release
```

## Framework Boundary

The packable library under `Src/<Name>` targets `netstandard2.0` and .NET 10; tests, examples, and benchmarks target .NET 10. Keep shared library code and dependencies compatible with `netstandard2.0`, and use conditional compilation only when a modern target needs a specialized implementation. The .NET 10 target is marked `IsAotCompatible`, which enables trim, single-file, and AOT compatibility analysis without making an unsupported compatibility claim for `netstandard2.0`.

## Application Boundaries

Executable applications are the highest-level layer and own interactions with untrusted or external state. They are responsible for:

- Parsing, validating, and normalizing input.
- Loading and validating configuration before work starts.
- Upholding documented contracts and preconditions before calling libraries.
- Selecting policy and defaults, wiring dependencies, and managing resource lifetimes.
- Translating failures into the appropriate output, log entry, response, or exit code.

Libraries may assume that these documented contracts have already been established. Do not repeatedly revalidate the same preconditions throughout trusted library code. Keeping contract enforcement at the application boundary allows libraries to remain simple, non-defensive, and efficient, especially on hot paths.

### Documenting Contracts

Document parameter-specific requirements in `<param>` and cross-parameter or state requirements as a precondition list in `<remarks>`. State that violating a precondition is a caller error. Important assumptions may also use `Debug.Assert` to detect contract violations during development without adding checks to Release builds.

Contracts are part of observable API behavior. Tightening a precondition, weakening a guarantee, or changing accepted input is a breaking behavioral change even when the method signature is unchanged. Update contract documentation and its tests together. Do not promise a specific exception or result when callers violate documented preconditions unless that behavior is itself part of the contract.

```csharp
/// <summary>Parses a previously validated value.</summary>
/// <param name="value">A non-empty value in the supported format.</param>
/// <remarks>
/// Preconditions:
/// <list type="bullet">
/// <item><description>The executable has validated the input format.</description></item>
/// <item><description>The value satisfies the configured size limit.</description></item>
/// </list>
/// Violating these preconditions is a caller error.
/// </remarks>
public static Result Parse(string value)
{
    System.Diagnostics.Debug.Assert(value.Length > 0);
    // Hot-path implementation without repeated boundary validation.
}
```

Tests at the executable boundary should verify that invalid external input is rejected. Library tests should exercise behavior within the documented preconditions rather than requiring every library layer to revalidate boundary input.

## Tests

Tests must provide regression value. Cover core behavior, important edge cases, failure paths, and bugs that could realistically recur. Avoid tests that only confirm property assignment, trivial forwarding, or framework behavior.

Test names must identify the subject, the member under test, and the behavior being verified:

- Name test classes `<Subject>Tests`.
- Name test methods `<Method>_<Scenario>_<ExpectedBehavior>`.

```csharp
public sealed class ParserTests
{
    [Fact]
    public void Parse_ValidInput_ReturnsExpectedValue() {}
}
```