namespace CarCareTracker.Models
{
    public class GasRecordViewModelContainer
    {
        public bool UseKwh { get; set; }
        public bool UseHours { get; set; }
        public List<GasRecordViewModel> GasRecords { get; set; } = new List<GasRecordViewModel>();
        // FEATURE: Flex Fuel - indicates the vehicle supports both regular unleaded and E-85
        public bool IsFlexFuel { get; set; } = false;
        // FEATURE: Odometer Compensation - indicates a compensation factor is configured for this vehicle
        public bool HasOdometerCompensation { get; set; } = false;
    }
}
