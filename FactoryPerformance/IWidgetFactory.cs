// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWidgetFactory.cs" company="GSD Logic">
//   Copyright © 2019 GSD Logic. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryPerformance
{
    public interface IWidgetFactory
    {
        Widget CreateWidget(string data);
    }
}