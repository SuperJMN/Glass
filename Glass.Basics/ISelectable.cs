namespace Glass.Basics {

    using System;

    public interface ISelectable {
        bool IsSelected { get; set; }
        event EventHandler<bool> IsSelectedChanged;
    }
}