
namespace Dispatch.Scripts
{
    public class ProofOfFulfillment
    {
        public bool HasSignature { get; set; }
        public string SignedBy { get; set; } = default!;        
        public string SignatureUrl { get; set; } = default!;
    }
}
