namespace CarCareTracker.Models
{
    public class GasRecordInputContainer
    {
        public bool UseKwh { get; set; }
        public bool UseHours { get; set; }
        public GasRecordInput GasRecord { get; set; } = new GasRecordInput();
        // FEATURE: Flex Fuel - whether this vehicle supports both regular unleaded and E-85
        public bool IsFlexFuel { get; set; } = false;
        // FEATURE: Odometer Compensation - whether odometer compensation is configured, so the form can label the mileage field as "Dashboard Mileage"
        public bool HasOdometerCompensation { get; set; } = false;
    }
}
