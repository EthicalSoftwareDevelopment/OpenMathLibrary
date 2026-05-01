//! Time-value-of-money and actuarial present-value formulas.

/// Provides actuarial discounting, annuity, and present-value calculations.
pub struct ActuarialLibrary;

impl ActuarialLibrary {
    /// Computes the accumulation factor `(1 + i)^n` for a given interest rate and period count.
    pub fn accumulation_factor(interest_rate: f64, periods: u32) -> Result<f64, &'static str> {
        Self::validate_rate(interest_rate)?;
        Ok((1.0 + interest_rate).powi(periods as i32))
    }

    /// Computes the discount factor, which is the reciprocal of the accumulation factor.
    pub fn discount_factor(interest_rate: f64, periods: u32) -> Result<f64, &'static str> {
        Ok(1.0 / Self::accumulation_factor(interest_rate, periods)?)
    }

    /// Projects a present value forward for the supplied rate and number of periods.
    pub fn future_value(
        present_value: f64,
        interest_rate: f64,
        periods: u32,
    ) -> Result<f64, &'static str> {
        Ok(present_value * Self::accumulation_factor(interest_rate, periods)?)
    }

    /// Discounts a future value back to the present for the supplied rate and periods.
    pub fn present_value(
        future_value: f64,
        interest_rate: f64,
        periods: u32,
    ) -> Result<f64, &'static str> {
        Ok(future_value * Self::discount_factor(interest_rate, periods)?)
    }

    /// Converts an effective interest rate into the corresponding effective discount rate.
    pub fn effective_discount_rate(interest_rate: f64) -> Result<f64, &'static str> {
        Self::validate_rate(interest_rate)?;
        Ok(interest_rate / (1.0 + interest_rate))
    }

    /// Computes the net present value of a series of cash flows.
    pub fn net_present_value(interest_rate: f64, cash_flows: &[f64]) -> Result<f64, &'static str> {
        Self::validate_rate(interest_rate)?;

        let mut total = 0.0;
        for (period, cash_flow) in cash_flows.iter().enumerate() {
            total += *cash_flow / (1.0 + interest_rate).powi(period as i32);
        }

        Ok(total)
    }

    /// Computes the present value of an annuity-immediate.
    pub fn annuity_immediate(
        payment: f64,
        interest_rate: f64,
        periods: u32,
    ) -> Result<f64, &'static str> {
        Self::validate_rate(interest_rate)?;

        if periods == 0 {
            return Ok(0.0);
        }
        if interest_rate.abs() <= f64::EPSILON {
            return Ok(payment * periods as f64);
        }

        let discount = Self::discount_factor(interest_rate, periods)?;
        Ok(payment * (1.0 - discount) / interest_rate)
    }

    /// Computes the present value of an annuity-due.
    pub fn annuity_due(
        payment: f64,
        interest_rate: f64,
        periods: u32,
    ) -> Result<f64, &'static str> {
        Ok(Self::annuity_immediate(payment, interest_rate, periods)? * (1.0 + interest_rate))
    }

    /// Computes the value of a level perpetuity.
    pub fn perpetuity(payment: f64, interest_rate: f64) -> Result<f64, &'static str> {
        if interest_rate <= 0.0 {
            return Err("Interest rate must be positive for a perpetuity.");
        }

        Ok(payment / interest_rate)
    }

    fn validate_rate(interest_rate: f64) -> Result<(), &'static str> {
        if !interest_rate.is_finite() {
            Err("Interest rate must be finite.")
        } else if interest_rate <= -1.0 {
            Err("Interest rate must be greater than -100%.")
        } else {
            Ok(())
        }
    }
}

#[cfg(test)]
mod tests {
    use super::ActuarialLibrary;

    #[test]
    fn time_value_of_money_functions_are_consistent() {
        let accumulation = ActuarialLibrary::accumulation_factor(0.05, 2).unwrap();
        let discount = ActuarialLibrary::discount_factor(0.05, 2).unwrap();

        assert!((accumulation - 1.1025).abs() < 1e-12);
        assert!((discount - (1.0 / 1.1025)).abs() < 1e-12);
        assert!((accumulation * discount - 1.0).abs() < 1e-12);
    }

    #[test]
    fn present_and_future_value_are_inverse_operations() {
        let future_value = ActuarialLibrary::future_value(1_000.0, 0.04, 3).unwrap();
        let present_value = ActuarialLibrary::present_value(future_value, 0.04, 3).unwrap();

        assert!((present_value - 1_000.0).abs() < 1e-9);
    }

    #[test]
    fn annuities_and_npv_produce_expected_results() {
        let annuity_immediate = ActuarialLibrary::annuity_immediate(100.0, 0.05, 3).unwrap();
        let annuity_due = ActuarialLibrary::annuity_due(100.0, 0.05, 3).unwrap();
        let npv =
            ActuarialLibrary::net_present_value(0.05, &[-250.0, 100.0, 100.0, 100.0]).unwrap();

        assert!(annuity_due > annuity_immediate);
        assert!(npv > 20.0 && npv < 25.0);
    }

    #[test]
    fn annuity_edge_cases_use_expected_closed_forms() {
        let zero_periods = ActuarialLibrary::annuity_immediate(250.0, 0.07, 0).unwrap();
        let zero_rate_immediate = ActuarialLibrary::annuity_immediate(250.0, 0.0, 4).unwrap();
        let zero_rate_due = ActuarialLibrary::annuity_due(250.0, 0.0, 4).unwrap();

        assert_eq!(zero_periods, 0.0);
        assert_eq!(zero_rate_immediate, 1_000.0);
        assert_eq!(zero_rate_due, 1_000.0);
    }

    #[test]
    fn effective_discount_and_perpetuity_match_known_values() {
        let discount_rate = ActuarialLibrary::effective_discount_rate(0.25).unwrap();
        let perpetuity = ActuarialLibrary::perpetuity(120.0, 0.06).unwrap();

        assert!((discount_rate - 0.2).abs() < 1e-12);
        assert!((perpetuity - 2_000.0).abs() < 1e-12);
    }

    #[test]
    fn invalid_rates_and_perpetuity_inputs_are_rejected() {
        assert!(ActuarialLibrary::accumulation_factor(-1.0, 1).is_err());
        assert!(ActuarialLibrary::discount_factor(f64::NAN, 1).is_err());
        assert!(ActuarialLibrary::net_present_value(f64::INFINITY, &[100.0]).is_err());
        assert!(ActuarialLibrary::perpetuity(100.0, 0.0).is_err());
        assert!(ActuarialLibrary::perpetuity(100.0, -0.01).is_err());
    }
}
