using CarCareTracker.Models;

namespace CarCareTracker.Helper
{
    public interface IGasHelper
    {
        // FEATURE: Odometer Compensation - added compensationFactor and compensationStart parameters
        List<GasRecordViewModel> GetGasRecordViewModels(List<GasRecord> result, bool useMPG, bool useUKMPG, decimal compensationFactor = 1.0m, int compensationStart = 0);
        string GetAverageGasMileage(List<GasRecordViewModel> results, bool useMPG);
    }
    public class GasHelper : IGasHelper
    {
        public string GetAverageGasMileage(List<GasRecordViewModel> results, bool useMPG)
        {
            var recordsToCalculate = results.Where(x => x.IncludeInAverage);
            if (recordsToCalculate.Any())
            {
                try
                {
                    var totalMileage = recordsToCalculate.Sum(x => x.DeltaMileage);
                    var totalGallons = recordsToCalculate.Sum(x => x.Gallons);
                    var averageGasMileage = totalMileage / totalGallons;
                    if (!useMPG && averageGasMileage > 0)
                    {
                        averageGasMileage = 100 / averageGasMileage;
                    }
                    return averageGasMileage.ToString("F");
                } catch
                {
                    return "0";
                }
            }
            return "0";
        }

        // FEATURE: Odometer Compensation - converts a dashboard mileage reading to real mileage using the compensation factor.
        // Only applies to readings beyond the compensation start mileage.
        private int ComputeRealMileage(int dashboardMileage, decimal compensationFactor, int compensationStart)
        {
            if (compensationFactor <= 0 || compensationFactor == 1.0m || dashboardMileage <= compensationStart)
                return dashboardMileage;
            return compensationStart + (int)decimal.Round((dashboardMileage - compensationStart) * compensationFactor);
        }

        // FEATURE: Odometer Compensation - accepts compensationFactor and compensationStart to compute real mileage for delta/MPG calculations.
        // FEATURE: Flex Fuel - passes FuelType from GasRecord through to GasRecordViewModel.
        public List<GasRecordViewModel> GetGasRecordViewModels(List<GasRecord> result, bool useMPG, bool useUKMPG, decimal compensationFactor = 1.0m, int compensationStart = 0)
        {
            //need to order by to get correct results
            result = result.OrderBy(x => x.Date).ThenBy(x => x.Mileage).ToList();
            var computedResults = new List<GasRecordViewModel>();
            // FEATURE: Odometer Compensation - track previous real mileage so deltas use real distances
            int previousRealMileage = 0;
            decimal unFactoredConsumption = 0.00M;
            int unFactoredMileage = 0;
            //perform computation.
            for (int i = 0; i < result.Count; i++)
            {
                var currentObject = result[i];
                decimal convertedConsumption;
                if (useUKMPG && useMPG)
                {
                    //if we're using UK MPG and the user wants imperial calculation insteace of l/100km
                    //if UK MPG is selected then the gas consumption are stored in liters but need to convert into UK gallons for computation.
                    convertedConsumption = currentObject.Gallons / 4.546M;
                }
                else
                {
                    convertedConsumption = currentObject.Gallons;
                }
                // FEATURE: Odometer Compensation - compute real mileage for this record
                int realMileage = ComputeRealMileage(currentObject.Mileage, compensationFactor, compensationStart);
                if (i > 0)
                {
                    // FEATURE: Odometer Compensation - use real mileage delta for accurate fuel economy calculation
                    var deltaMileage = realMileage - previousRealMileage;
                    if (deltaMileage < 0)
                    {
                        deltaMileage = 0;
                    }
                    var gasRecordViewModel = new GasRecordViewModel()
                    {
                        Id = currentObject.Id,
                        VehicleId = currentObject.VehicleId,
                        MonthId = currentObject.Date.Month,
                        Date = currentObject.Date.ToShortDateString(),
                        Mileage = currentObject.Mileage,
                        // FEATURE: Odometer Compensation - store real mileage for display alongside dashboard mileage
                        RealMileage = realMileage,
                        Gallons = convertedConsumption,
                        Cost = currentObject.Cost,
                        DeltaMileage = deltaMileage,
                        CostPerGallon = convertedConsumption > 0.00M ? currentObject.Cost / convertedConsumption : 0,
                        IsFillToFull = currentObject.IsFillToFull,
                        MissedFuelUp = currentObject.MissedFuelUp,
                        Notes = currentObject.Notes,
                        Tags = currentObject.Tags,
                        ExtraFields = currentObject.ExtraFields,
                        Files = currentObject.Files,
                        // FEATURE: Flex Fuel - pass through the fuel type for per-type statistics and display
                        FuelType = currentObject.FuelType
                    };
                    if (currentObject.MissedFuelUp)
                    {
                        //if they missed a fuel up, we skip MPG calculation.
                        gasRecordViewModel.MilesPerGallon = 0;
                        //reset unFactored vars for missed fuel up because the numbers wont be reliable.
                        unFactoredConsumption = 0;
                        unFactoredMileage = 0;
                    }
                    else if (currentObject.IsFillToFull && currentObject.Mileage != default)
                    {
                        //if user filled to full and an odometer is provided, otherwise we will defer calculations
                        if (convertedConsumption > 0.00M && deltaMileage > 0)
                        {
                            try
                            {
                                gasRecordViewModel.MilesPerGallon = useMPG ? (unFactoredMileage + deltaMileage) / (unFactoredConsumption + convertedConsumption) : 100 / ((unFactoredMileage + deltaMileage) / (unFactoredConsumption + convertedConsumption));
                            }
                            catch
                            {
                                gasRecordViewModel.MilesPerGallon = 0;
                            }
                        }
                        //reset unFactored vars
                        unFactoredConsumption = 0;
                        unFactoredMileage = 0;
                    }
                    else
                    {
                        unFactoredConsumption += convertedConsumption;
                        unFactoredMileage += deltaMileage;
                        gasRecordViewModel.MilesPerGallon = 0;
                    }
                    computedResults.Add(gasRecordViewModel);
                }
                else
                {
                    computedResults.Add(new GasRecordViewModel()
                    {
                        Id = currentObject.Id,
                        VehicleId = currentObject.VehicleId,
                        MonthId = currentObject.Date.Month,
                        Date = currentObject.Date.ToShortDateString(),
                        Mileage = currentObject.Mileage,
                        // FEATURE: Odometer Compensation - store real mileage even for the first record
                        RealMileage = realMileage,
                        Gallons = convertedConsumption,
                        Cost = currentObject.Cost,
                        DeltaMileage = 0,
                        MilesPerGallon = 0,
                        CostPerGallon = convertedConsumption > 0.00M ? currentObject.Cost / convertedConsumption : 0,
                        IsFillToFull = currentObject.IsFillToFull,
                        MissedFuelUp = currentObject.MissedFuelUp,
                        Notes = currentObject.Notes,
                        Tags = currentObject.Tags,
                        ExtraFields = currentObject.ExtraFields,
                        Files = currentObject.Files,
                        // FEATURE: Flex Fuel - pass through the fuel type
                        FuelType = currentObject.FuelType
                    });
                }
                if (currentObject.Mileage != default)
                {
                    // FEATURE: Odometer Compensation - update previous real mileage for next iteration's delta
                    previousRealMileage = realMileage;
                }
            }
            return computedResults;
        }
    }
}
