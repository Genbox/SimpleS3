using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "The provider test fixtures supply non-null clients and test inputs; null argument validation belongs to the library APIs under test.")]