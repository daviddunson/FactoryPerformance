// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Widget.cs" company="GSD Logic">
//   Copyright © 2019 GSD Logic. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryPerformance
{
    public class Widget
    {
        public Widget(string data)
        {
            this.Data = data;
        }

        public string Data { get; set; }
    }
}