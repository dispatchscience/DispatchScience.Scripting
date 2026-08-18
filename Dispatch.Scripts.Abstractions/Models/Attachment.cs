namespace Dispatch.Scripts
{
    public class Attachment
    {
        public string AttachmentId { get; set; } = default!;
        public string? Note { get; set; }
        public bool IncludeWithInvoice { get; set; }
        public string? WorkflowStepId { get; set; }
        public string? FileName { get; set; }
        public AttachmentType AttachmentType { get; set; }
    }
}