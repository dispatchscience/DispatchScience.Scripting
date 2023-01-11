using System.Collections.Generic;

namespace Dispatch.Scripts
{
    public class Workflow
    {
        public string Id { get; set; } = default!;
        public IList<Step> Steps { get; set; } = new List<Step>();

        public class Step
        {
            public string Id { get; set; } = default!;
            public string TitlePrimary { get; set; } = default!;
            public string? TitleSecondary { get; set; }
            public bool CanSkip { get; set; }
            public Type StepType { get; set; }
            public bool IsActive { get; set; }

            public enum Type
            {
                Id = 0,
                COD,
                Image,
                Signature,
                UserField,
                Barcode,
                ParcelType,
                Weight,
                Dimension,
                Description,
                Tip,
                DocumentSignature
            }
        }
    }
}