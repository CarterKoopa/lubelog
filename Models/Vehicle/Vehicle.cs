using System.Text.Json.Serialization;

namespace CarCareTracker.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string ImageLocation { get; set; } = "/defaults/noimage.png";
        public string MapLocation { get; set; } = "";
        public int Year { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        [JsonConverter(typeof(FromDateOptional))]
        public string PurchaseDate { get; set; } = string.Empty;
        [JsonConverter(typeof(FromDateOptional))]
        public string SoldDate { get; set; } = string.Empty;
        public decimal PurchasePrice { get; set; }
        public decimal SoldPrice { get; set; }
        public bool IsElectric { get; set; } = false;
        public bool IsDiesel { get; set; } = false;
        // FEATURE: Flex Fuel - indicates vehicle can run on both regular unleaded and E-85
        public bool IsFlexFuel { get; set; } = false;
        public bool UseHours { get; set; } = false;
        public bool OdometerOptional { get; set; } = false;
        public List<ExtraField> ExtraFields { get; set; } = new List<ExtraField>();
        public List<string> Tags { get; set; } = new List<string>();
        public bool HasOdometerAdjustment { get; set; } = false;
        /// <summary>
        /// Primarily used for vehicles with odometer units different from user's settings.
        /// </summary>
        [JsonConverter(typeof(FromDecimalOptional))]
        public string OdometerMultiplier { get; set; } = "1";
        /// <summary>
        /// Primarily used for vehicles where the odometer does not reflect actual mileage.
        /// </summary>
        [JsonConverter(typeof(FromIntOptional))]
        public string OdometerDifference { get; set; } = "0";
        public List<DashboardMetric> DashboardMetrics { get; set; } = new List<DashboardMetric>();
        // FEATURE: Odometer Compensation - factor applied to mileage after OdometerCompensationStart to correct for non-stock tire size.
        // Empty string means compensation is disabled.
        public string OdometerCompensationFactor { get; set; } = "";
        // Dashboard mileage reading at which the compensation factor begins to apply.
        public int OdometerCompensationStart { get; set; } = 0;
        /// <summary>
        /// Determines what is displayed in place of the license plate.
        /// </summary>
        public string VehicleIdentifier { get; set; } = "LicensePlate";
    }
}
