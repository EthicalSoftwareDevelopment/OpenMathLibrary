pub struct CivilEngineeringLibrary;

impl CivilEngineeringLibrary {
    pub fn concrete_mix_total_parts(cement_part: f64, sand_part: f64, aggregate_part: f64) -> f64 {
        cement_part + sand_part + aggregate_part
    }

    pub fn concrete_component_quantity(total_dry_volume: f64, component_part: f64, total_parts: f64) -> f64 {
        total_dry_volume * component_part / total_parts
    }

    pub fn cement_quantity(volume: f64, ratio: f64) -> f64 {
        volume / (1.0 + ratio)
    }

    pub fn sand_quantity(volume: f64, ratio: f64) -> f64 {
        volume * ratio / (1.0 + ratio)
    }

    pub fn aggregate_quantity(volume: f64, ratio: f64) -> f64 {
        volume * ratio / (1.0 + ratio)
    }

    pub fn slope_as_percentage(rise: f64, run: f64) -> f64 {
        (rise / run) * 100.0
    }

    pub fn slope_as_ratio(rise: f64, run: f64) -> f64 {
        rise / run
    }

    pub fn earthwork_volume(area: f64, depth: f64) -> f64 {
        area * depth
    }

    pub fn average_cross_sectional_area(area1: f64, area2: f64) -> f64 {
        (area1 + area2) / 2.0
    }

    pub fn steel_quantity(area: f64, spacing: f64) -> f64 {
        area / spacing
    }

    pub fn shear_stress(force: f64, area: f64) -> f64 {
        force / area
    }

    pub fn weight_of_steel_per_unit_length(diameter: f64) -> f64 {
        0.006165 * diameter * diameter
    }

    pub fn unit_weight_of_steel() -> f64 {
        7850.0
    }

    pub fn unit_weight_of_concrete() -> f64 {
        2400.0
    }

    pub fn unit_weight_of_brick() -> f64 {
        1920.0
    }

    pub fn unit_weight_of_water() -> f64 {
        1000.0
    }

    pub fn load_bearing_capacity(area: f64, unit_weight: f64) -> f64 {
        area * unit_weight
    }

    pub fn slab_load(live_load: f64, dead_load: f64) -> f64 {
        live_load + dead_load
    }

    pub fn cantilever_beam_deflection(load: f64, length: f64, modulus: f64, inertia: f64) -> f64 {
        (load * length.powi(3)) / (3.0 * modulus * inertia)
    }

    pub fn moment_of_inertia_of_rectangular_section(base_amount: f64, height: f64) -> f64 {
        (base_amount * height.powi(3)) / 12.0
    }

    pub fn moment_of_inertia_of_circular_section(diameter: f64) -> f64 {
        std::f64::consts::PI * diameter.powi(4) / 64.0
    }

    pub fn bending_moment(force: f64, distance: f64) -> f64 {
        force * distance
    }

    pub fn shear_force(force: f64, _distance: f64) -> f64 {
        force
    }

    pub fn bricks_calculation(length: f64, width: f64, height: f64) -> f64 {
        length * width * height
    }

    pub fn brickwork_volume(length: f64, width: f64, height: f64) -> f64 {
        length * width * height
    }

    pub fn number_of_bricks_required(wall_volume: f64, brick_volume: f64, wastage_factor: f64) -> f64 {
        wall_volume * wastage_factor / brick_volume
    }

    pub fn dry_material_quantity_for_mortar(volume: f64, ratio: f64) -> f64 {
        volume * ratio / (1.0 + ratio)
    }

    pub fn wet_mortar_volume(volume: f64, ratio: f64) -> f64 {
        volume / (1.0 + ratio)
    }

    pub fn excavation_calculation(length: f64, width: f64, depth: f64) -> f64 {
        length * width * depth
    }

    pub fn retaining_wall_stability(height: f64, width: f64, density: f64, angle: f64) -> f64 {
        height * width * density * 0.5 * angle.sin()
    }

    pub fn one_way_slab_thickness(span: f64, load: f64, factor: f64) -> f64 {
        (span.powi(3) * load) / (8.0 * factor)
    }

    pub fn two_way_slab_thickness(span: f64, load: f64, factor: f64) -> f64 {
        (span.powi(3) * load) / (12.0 * factor)
    }

    pub fn compaction_factor(initial_volume: f64, final_volume: f64) -> f64 {
        initial_volume / final_volume
    }

    pub fn soil_settlement(initial_volume: f64, final_volume: f64) -> f64 {
        initial_volume - final_volume
    }
}

#[cfg(test)]
mod tests {
    use super::CivilEngineeringLibrary;

    #[test]
    fn section_and_loading_calculations_match_known_values() {
        assert!((CivilEngineeringLibrary::slope_as_percentage(1.0, 4.0) - 25.0).abs() < 1e-12);
        assert!((CivilEngineeringLibrary::moment_of_inertia_of_rectangular_section(0.3, 0.6) - 0.0054).abs() < 1e-12);
        assert!((CivilEngineeringLibrary::bending_moment(12.0, 2.5) - 30.0).abs() < 1e-12);
        assert!((CivilEngineeringLibrary::shear_force(18.0, 3.0) - 18.0).abs() < 1e-12);
    }

    #[test]
    fn material_estimates_are_consistent() {
        let total_parts = CivilEngineeringLibrary::concrete_mix_total_parts(1.0, 2.0, 4.0);
        let cement = CivilEngineeringLibrary::concrete_component_quantity(1.54, 1.0, total_parts);
        let brick_count = CivilEngineeringLibrary::number_of_bricks_required(1.0, 0.001539, 1.05);

        assert!((total_parts - 7.0).abs() < 1e-12);
        assert!(cement > 0.21 && cement < 0.23);
        assert!(brick_count > 680.0 && brick_count < 683.0);
    }
}
