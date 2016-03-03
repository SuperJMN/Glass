namespace Glass.Basics.Cinch.Validation {
    public interface ISpecification<in TEntity> {
        bool IsSatisfiedBy(TEntity entity);
    }
}