// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgetFactory.cs" company="GSD Logic">
//   Copyright © 2019 GSD Logic. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryPerformance
{
    public class WidgetFactory
    {
        public Widget CreateWidget(string data)
        {
            return new Widget(data);
        }
    }
}