namespace ROWM.Dal
{
    public partial class Ownership
    {
        public enum OwnershipType { Primary = 1, Related };

        public bool IsPrimary() => this.Ownership_t == (int) OwnershipType.Primary;
    }
}
