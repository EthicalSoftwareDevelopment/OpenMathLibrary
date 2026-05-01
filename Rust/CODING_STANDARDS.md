# Rust Coding Standards

## General Guidelines
- **Readability**: Write code that is easy to read and understand.
- **Consistency**: Follow consistent naming conventions and formatting.
- **Documentation**: Document all public modules, functions, and types.

## Naming Conventions
- Use `snake_case` for function and variable names.
- Use `PascalCase` for type and trait names.
- Use `SCREAMING_SNAKE_CASE` for constants and statics.
- Use `snake_case` for filenames and module names. Avoid spaces, uppercase letters, or special characters.
  - Example: `ActuarialLibrary.rs` → `actuarial_library.rs`

## Filename Standards
- **Snake Case for Filenames**:
  - Use `snake_case` for all file and module names.
  - Avoid spaces, uppercase letters, or special characters.
  - Example:
    - `ActuarialLibrary.rs` → `actuarial_library.rs`
    - `GeneralEnergyDefinitions.rs` → `general_energy_definitions.rs`

- **Module Root Files**:
  - Use `mod.rs` for module root files if the module is a directory.
  - Example:
    - `src/actuarial_and_arithmetic/mod.rs` for the `actuarial_and_arithmetic` module.

- **Consistent Directory Structure**:
  - Group related modules into directories.
  - Example:
    - `src/actuarial_and_arithmetic/actuarial_library.rs`
    - `src/actuarial_and_arithmetic/arithmetic_library.rs`

- **Tests and Examples**:
  - Place integration tests in the `tests/` directory.
  - Use descriptive filenames for test files.
  - Example:
    - `tests/actuarial_exports.rs`
    - `tests/engineering_exports.rs`

- **Avoid Redundant Prefixes**:
  - Avoid repeating the module name in filenames.
  - Example:
    - `src/actuarial_and_arithmetic/actuarial_library.rs` is preferred over `src/actuarial_and_arithmetic/actuarial_and_arithmetic_library.rs`.

- **Binary Targets**:
  - Place binary targets in the `src/bin/` directory.
  - Use descriptive filenames for binaries.
  - Example:
    - `src/bin/cli.rs` for a command-line interface binary.

## Code Formatting
- Use `rustfmt` to format your code.
- Limit lines to 100 characters.
- Use 4 spaces per indentation level.

## Error Handling
- Use `Result` and `Option` for recoverable and optional errors.
- Use `unwrap` and `expect` sparingly, only when absolutely certain.
- Prefer `?` operator for propagating errors.

## Testing
- Write unit tests for all functions.
- Use `#[test]` attribute for test functions.
- Group related tests in modules.

## Performance
- Avoid premature optimization.
- Use `cargo bench` for benchmarking critical code.
- Prefer `std::collections` for common data structures.

## Unsafe Code
- Minimize the use of `unsafe` blocks.
- Document why `unsafe` is necessary and how it is safe.

## Dependencies
- Use minimal dependencies.
- Keep dependencies up-to-date.
- Check for security vulnerabilities regularly.

## Version Control
- Commit small, logical changes.
- Write clear and descriptive commit messages.
- Use feature branches for new features.

## Code Reviews
- Review code for correctness, readability, and adherence to standards.
- Provide constructive feedback.
- Address all review comments before merging.

## Tools
- Use `clippy` for linting.
- Use `cargo audit` to check for vulnerabilities.
- Use `cargo doc` to generate documentation.

## Additional Resources
- [The Rust Programming Language Book](https://doc.rust-lang.org/book/)
- [Rust API Guidelines](https://rust-lang.github.io/api-guidelines/)
- [Rust Clippy Lints](https://rust-lang.github.io/rust-clippy/)

---

Adhering to these standards will ensure high-quality, maintainable, and reliable Rust code.
