namespace Dispatch.Scripts
{
    public enum ScriptAccountScope
    {
        /// <summary>
        /// The billed account - the Billing Account on a Bill To order. This is what every script helper has
        /// always resolved to, and stays the default.
        /// </summary>
        Default = 0,

        /// <summary>
        /// The account that placed a Bill To order. Resolves to nothing on a standard order, where the helper
        /// returns null / empty rather than falling back to the billed account.
        /// </summary>
        Ordering = 1
    }
}
