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
