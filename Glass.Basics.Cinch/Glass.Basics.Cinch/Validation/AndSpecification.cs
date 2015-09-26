namespace Glass.Basics.Cinch.Validation {
    internal class AndSpecification<TEntity> : ISpecification<TEntity> {
        private readonly ISpecification<TEntity> spec1;
        private readonly ISpecification<TEntity> spec2;

        internal AndSpecification(ISpecification<TEntity> s1, ISpecification<TEntity> s2) {
            spec1 = s1;
            spec2 = s2;
        }

        public bool IsSatisfiedBy(TEntity candidate) {
            return spec1.IsSatisfiedBy(candidate) && spec2.IsSatisfiedBy(candidate);
        }
    }
}