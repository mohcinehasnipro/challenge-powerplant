using Challenge.Service.Dtos;
using Challenge.Service.Interfaces;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Challenge.Service.Services
{
    public class ProductionPlanService : IProductionPlanService
    {
        private readonly ILogger<ProductionPlanService> _logger;

        public ProductionPlanService(ILogger<ProductionPlanService> logger)
        {
            _logger = logger;
        }
        public List<Response> Calculate(Payload payload)
        {
            // Initialize the result production plan
            var responses = new List<Response>();

            try
            {
                // Sort power plants by cost (merit order) and capacity 
                // the cheapest power plants at the top and the most expensive ones at the bottom
                var sortedPowerPlants = payload.Powerplants
                    .OrderBy(p => GetProductionCost(p, payload))
                    .ThenByDescending(p => p.Pmax)
                    .ToList();

                // Initialize remaining load
                double remainingLoad = payload.Load;

                foreach (var powerPlant in sortedPowerPlants)
                {
                    // Tthe available capacity for this power plant
                    double capacity = (powerPlant.Pmax - powerPlant.Pmin);
                    double availableCapacity = capacity < 0 ? 0 : capacity;

                    // The amount of load to be met by this power plant
                    double loadToMeet = Math.Min(remainingLoad, availableCapacity);

                    // Deduct the load met from the remaining load
                    remainingLoad -= loadToMeet;

                    // Add the power plant and its production to the production plan
                    responses.Add(new Response
                    {
                        Name = powerPlant.Name,
                        P = loadToMeet
                    });

                    // If the remaining load is zero, break the loop
                    if (remainingLoad <= 0)
                        break;
                }

                // Fill the remaining load with turbines if needed
                if (remainingLoad > 0)
                {
                    AllocateLoadToWindTurbines(responses, payload.Powerplants, remainingLoad);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return responses;
        }

        private static void AllocateLoadToWindTurbines(List<Response> responses, List<Powerplant> powerplants, double remainingLoad)
        {
            var windTurbines = powerplants
                .Where(p => p.Type == "windturbine")
                .OrderByDescending(p => p.Pmax)
                .ToList();

            foreach (var windTurbine in windTurbines)
            {
                double loadToMeet = Math.Min(remainingLoad, windTurbine.Pmax);

                responses.Add(new Response
                {
                    Name = windTurbine.Name,
                    P = loadToMeet
                });

                remainingLoad -= loadToMeet;

                if (remainingLoad <= 0)
                    break;
            }
        }

        private double GetProductionCost(Powerplant powerPlant, Payload payload)
        {
            if (powerPlant.Type == "windturbine")
            {
                // Wind turbines have zero cost
                return 0;
            }
            else if (powerPlant.Type == "gasfired")
            {
                // Calculate cost based on gas price and efficiency
                double gasCost = payload.Fuels.GasEuroMWh * (1 / powerPlant.Efficiency);

                double co2Cost = payload.Fuels.Co2EuroTon / (1 / powerPlant.Efficiency);

                return gasCost + co2Cost;
            }
            else if (powerPlant.Type == "turbojet")
            {
                // Calculate cost based on kerosene price and efficiency
                double keroseneCost = payload.Fuels.KerosineEuroMWh * (1 / powerPlant.Efficiency);
                double co2Cost = payload.Fuels.Co2EuroTon / (1 / powerPlant.Efficiency);
                return keroseneCost + co2Cost;
            }

            return 0; // Default cost for unknown types
        }

    }

}
