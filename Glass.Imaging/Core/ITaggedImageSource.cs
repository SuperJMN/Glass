namespace Glass.Imaging.Core
{
    using System;

    public interface ITaggedImageSource
    {
        IObservable<TaggedImage> Images { get; }
        void Start();
    }
}