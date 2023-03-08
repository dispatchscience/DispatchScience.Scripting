using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public interface IPayoutUpdater : IPayoutReader
    {
        // Add a driver to the payout, needed to override manually.
        Task AddDriver(string driverId);

        // Change the payout of drivers manually. It can only be done for drivers that are already part of the payout.
        // Note: You need to provide a value for each driver that is part of the payout.
        // Note: The amount provided should be the amount before the fuel surcharges as they will be added (or not) depending on the driver's payout schedule.
        // Note: If a payout was settled, the settlement will not be modified unless cancelled and recreated.
        Task OverrideManually(ManualPayout[] payouts);

        // These methods change the way the payout will be calculated for the assigned driver. It cannot be changed once a driver has been added to the payout.
        Task ChangeToPayoutSchedule();
        Task ChangeToFlatRate(decimal amount);
        Task ChangeToCommissionPercentage(decimal deliveryCommissionPercentage, decimal extraFeeCommissionPercentage, decimal fuelSurchageCommissionPercentage);
    }
}