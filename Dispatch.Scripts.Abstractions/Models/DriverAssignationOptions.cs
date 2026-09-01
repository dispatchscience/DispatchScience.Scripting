namespace Dispatch.Scripts
{
    public class DriverAssignationOptions
    {
        public static DriverAssignationOptions Default => new DriverAssignationOptions();

        public bool IgnoreExpiredDocumentsWarning { get; set; } = true;
        public bool IgnoreFailedCertificationsWarning { get; set; } = true;
        public bool IgnoreMissingAttributesWarning { get; set; } = true;
        public bool IgnoreHoldReasonsWarning { get; set; } = true;
    }
}