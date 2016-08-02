// Copyright (c) 2015 ZZZ Projects. All rights reserved
// Licensed under MIT License (MIT) (https://github.com/zzzprojects/Z.ExtensionMethods)
// Website: http://www.zzzprojects.com/
// Feedback / Feature Requests / Issues : http://zzzprojects.uservoice.com/forums/283927
// All ZZZ Projects products: Entity Framework Extensions / Bulk Operations / Extension Methods /Icon Library

using System;

public static partial class Extensions
{
    /// <summary>
    ///     An object extension method that converts this object to a double or default.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>The given data converted to a double.</returns>
    public static double ToDoubleOrDefault(this object @this)
    {
        try
        {
            return Convert.ToDouble(@this);
        }
        catch (Exception)
        {
            return default(double);
        }
    }

    /// <summary>
    ///     An object extension method that converts this object to a double or default.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The given data converted to a double.</returns>
    public static double ToDoubleOrDefault(this object @this, double defaultValue)
    {
        try
        {
            return Convert.ToDouble(@this);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    /// <summary>
    ///     An object extension method that converts this object to a double or default.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="defaultValueFactory">The default value factory.</param>
    /// <returns>The given data converted to a double.</returns>
    public static double ToDoubleOrDefault(this object @this, Func<double> defaultValueFactory)
    {
        try
        {
            return Convert.ToDouble(@this);
        }
        catch (Exception)
        {
            return defaultValueFactory();
        }
    }
}