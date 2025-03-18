using UniRx;

public static class Storage
{
    public static ReactiveProperty<int> WheatCount { get; } = new ReactiveProperty<int>(0);
    public static ReactiveProperty<int> FlourCount { get; } = new ReactiveProperty<int>(0);
    public static ReactiveProperty<int> BreadCount { get; } = new ReactiveProperty<int>(0);
}