﻿//The MIT License(MIT)

//Copyright(c) 2016 Alberto Rodríguez Orozco & LiveCharts Contributors

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Linq;
using LiveCharts.Definitions.Charts;
using LiveCharts.Definitions.Points;
using LiveCharts.Definitions.Series;

namespace LiveCharts.Series
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="SeriesCore" />
    /// <seealso cref="LiveCharts.Definitions.Series.IPieSeries" />
    public class PieSeriesCore : SeriesCore, IPieSeries
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PieSeriesCore"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        public PieSeriesCore(ISeriesView view) : base(view)
        {
            PreferredSelectionMode= TooltipSelectionMode.SharedXValues;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public override void Update()
        {
            var pieChart = (IPieChart) View.Core.Chart.View;

            var maxPushOut = View.Core.Chart.View.ActualSeries
                .OfType<IPieSeriesView>()
                .Select(x => x.PushOut)
                .DefaultIfEmpty(0).Max();

            var minDimension = Chart.View.DrawMarginWidth < Chart.View.DrawMarginHeight
                ? Chart.View.DrawMarginWidth
                : Chart.View.DrawMarginHeight;
            minDimension -= 10 + maxPushOut;
            minDimension = minDimension < 10 ? 10 : minDimension;
            
            var inner = pieChart.InnerRadius;

            var startAt = pieChart.StartingRotationAngle > 360
                ? 360
                : (pieChart.StartingRotationAngle < 0
                    ? 0
                    : pieChart.StartingRotationAngle);

            foreach (var chartPoint in View.ActualValues.GetPoints(View))
            {
                chartPoint.View = View.InitializePointView(Chart.View);

                var pieSlice = (IPieSlicePointView) chartPoint.View;

                var space = pieChart.InnerRadius +
                            ((minDimension/2) - pieChart.InnerRadius)*((chartPoint.X + 1)/(View.Values.GetTracker(View).XLimit.Max + 1));

                chartPoint.SeriesView = View;

                pieSlice.Radius = space;
                pieSlice.InnerRadius = inner;
                pieSlice.Rotation = startAt + (chartPoint.StackedParticipation - chartPoint.Participation)*360;
                pieSlice.Wedge = chartPoint.Participation*360 > 0 ? chartPoint.Participation*360 : 0;

                chartPoint.View.Draw(null, 0, View, Chart);

                inner = pieSlice.Radius;
            }
        }
    }
}