﻿namespace MaterialDesignThemes.Wpf;

public class RatingBarButton : ButtonBase
{
    static RatingBarButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingBarButton), new FrameworkPropertyMetadata(typeof(RatingBarButton)));
    }

    private static readonly DependencyPropertyKey ValuePropertyKey =
        DependencyProperty.RegisterReadOnly(
            "Value", typeof(int), typeof(RatingBarButton),
            new PropertyMetadata(default(int)));

    public static readonly DependencyProperty ValueProperty =
        ValuePropertyKey.DependencyProperty;

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        internal set => SetValue(ValuePropertyKey, value);
    }

    public RatingBar RatingBar { get; } = null!;    // Null initializer added to suppress warning (for the obsoleted empty constructor)

    public RatingBarButton(RatingBar ratingBar) => RatingBar = ratingBar;

    // Only added the default constructor for back-compat. Ideally should not be used from MDIX consumers, but you never know.
    [Obsolete("Should not be used. Use the constructor taking a RatingBar instance as parameter instead. This constructor will be removed in a future version.")]
    public RatingBarButton()
    { }
}
