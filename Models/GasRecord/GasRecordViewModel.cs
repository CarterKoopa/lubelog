namespace CarCareTracker.Models
{
    public class GasRecordViewModel
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int MonthId { get; set; }
        public string Date { get; set; } = string.Empty;
        /// <summary>
        /// American moment
        /// </summary>
        public int Mileage { get; set; }
        /// <summary>
        /// Wtf is a kilometer?
        /// </summary>
        public decimal Gallons { get; set; }
        public decimal Cost { get; set; }
        public int DeltaMileage { get; set; }
        public decimal MilesPerGallon { get; set; }
        public decimal CostPerGallon { get; set; }
        public bool IsFillToFull { get; set; }
        public bool MissedFuelUp { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public List<ExtraField> ExtraFields { get; set; } = new List<ExtraField>();
        public List<UploadedFiles> Files { get; set; } = new List<UploadedFiles>();
        public bool IncludeInAverage { get { return MilesPerGallon > 0 || (!IsFillToFull && !MissedFuelUp) || (Mileage == default && !MissedFuelUp); } }
        // FEATURE: Flex Fuel - which fuel type was ADDED at this fill-up (used for the Fuel Type column display)
        public string FuelType { get; set; } = "Gasoline";
        // FEATURE: Flex Fuel - which fuel type was BURNED to reach this fill-up (= the previous fill's FuelType).
        // This is used for per-type MPG averages: the MPG shown at fill N represents efficiency of the fuel
        // that was actually consumed since fill N-1, so it should be credited to fill N-1's fuel type.
        public string MpgFuelType { get; set; } = "Gasoline";
        // FEATURE: Odometer Compensation - real mileage after applying the compensation factor (equals Mileage when no compensation is configured)
        public int RealMileage { get; set; }
    }
}
