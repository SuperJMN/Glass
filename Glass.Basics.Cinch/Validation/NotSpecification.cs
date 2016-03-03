namespace Glass.Basics.Cinch.Validation {
    internal class NotSpecification<TEntity> : ISpecification<TEntity> {
        private readonly ISpecification<TEntity> wrapped;

        internal NotSpecification(ISpecification<TEntity> x) {
            wrapped = x;
        }

        public bool IsSatisfiedBy(TEntity candidate) {
            return !wrapped.IsSatisfiedBy(candidate);
        }
    }
}