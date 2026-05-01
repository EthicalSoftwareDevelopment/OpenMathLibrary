use open_math_library::{
    ActuarialLibrary, ArithmeticLibrary, BisectionLibrary, PiecewiseSpecialFunctionsLibrary,
};

#[test]
fn actuarial_and_arithmetic_exports_are_available_from_the_crate_root() {
    let accumulation = ActuarialLibrary::accumulation_factor(0.05, 2).unwrap();
    let gcd = ArithmeticLibrary::gcd(84, 30);
    let root = BisectionLibrary::bisection(1.0, 2.0, 1e-12, 100, |x| x * x - 2.0).unwrap();
    let sinc = PiecewiseSpecialFunctionsLibrary::sinc_function(0.0);

    assert!((accumulation - 1.1025).abs() < 1e-12);
    assert_eq!(gcd, 6);
    assert!((root - 2.0_f64.sqrt()).abs() < 1e-9);
    assert!((sinc - 1.0).abs() < 1e-12);
}

#[test]
fn actuarial_and_root_finding_error_paths_are_reported() {
    assert!(ActuarialLibrary::accumulation_factor(-1.0, 1).is_err());
    assert!(ArithmeticLibrary::factorial(35).is_err());
    assert!(BisectionLibrary::bisection(1.0, 2.0, 1e-12, 0, |x| x).is_err());
}

#[test]
fn actuarial_arithmetic_and_special_function_exports_cover_boundary_cases() {
    let annuity = ActuarialLibrary::annuity_immediate(50.0, 0.0, 4).unwrap();
    let perpetuity = ActuarialLibrary::perpetuity(50.0, 0.1).unwrap();
    let lcm = ArithmeticLibrary::lcm(-21, 6);
    let totient = ArithmeticLibrary::totient(10).unwrap();
    let dirichlet = PiecewiseSpecialFunctionsLibrary::dirichlet_kernel(0.0, 2);

    assert_eq!(annuity, 200.0);
    assert_eq!(perpetuity, 500.0);
    assert_eq!(lcm, 42);
    assert_eq!(totient, 4);
    assert_eq!(dirichlet, 5.0);
}
