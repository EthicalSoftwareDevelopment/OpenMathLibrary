# OpenMathLibrary

OpenMathLibrary is a Rust-based library designed to provide a comprehensive suite of mathematical tools and utilities. It is modular, scalable, and adheres to Rust's best practices, making it suitable for various mathematical domains such as actuarial science and engineering.

## Features
- **Actuarial Mathematics**: Includes modules for mortality calculations, annuities, and other actuarial computations.
- **Engineering Mathematics**: Provides tools for statics, dynamics, and other engineering-related calculations.
- **Utilities**: Shared mathematical utilities for common operations.

## Folder Structure
```
C:\Dev\OpenMathLibrary\Rust
│
├── Cargo.toml          # Rust project configuration
├── README.md           # Project documentation
├── LICENSE             # Licensing information
│
├── src                 # Source code
│   ├── lib.rs          # Library entry point
│   ├── actuarial       # Actuarial math module
│   │   ├── mod.rs      # Module entry point
│   │   ├── mortality.rs # Example submodule
│   │   └── annuities.rs # Example submodule
│   ├── engineering     # Engineering math module
│   │   ├── mod.rs      # Module entry point
│   │   ├── statics.rs  # Example submodule
│   │   └── dynamics.rs # Example submodule
│   └── utils           # Shared utilities
│       ├── mod.rs      # Module entry point
│       └── math.rs     # Example utility functions
│
└── tests               # Integration tests
    ├── actuarial_tests.rs
    └── engineering_tests.rs
```

## Getting Started
1. Clone the repository:
   ```bash
   git clone <repository-url>
   ```
2. Navigate to the project directory:
   ```bash
   cd OpenMathLibrary/Rust
   ```
3. Build the project:
   ```bash
   cargo build
   ```
4. Run tests:
   ```bash
   cargo test
   ```

## Contributing
Contributions are welcome! Please fork the repository and submit a pull request.

## License
This project is licensed under the MIT License. See the `LICENSE` file for details.
