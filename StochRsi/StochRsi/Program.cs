  
/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
using QuantConnect.Data.Market;

namespace QuantConnect.Indicators
{
    /// <summary>
    /// 
    /// Source: https://www.investopedia.com/terms/s/stochrsi.asp
    /// </summary>
    public class StochasticRelativeStrengthIndex : TradeBarIndicator, IIndicatorWarmUpPeriodProvider
    {
        private readonly RelativeStrengthIndex _rsi;
        private readonly Maximum _max;
        private readonly Minimum _min;

        /// <summary>
        /// Gets a flag indicating when the indicators are ready and fully initialized
        /// </summary>
        public override bool IsReady => _rsi.IsReady;
        public override bool IsReady => _max.IsReady;
        public override bool IsReady => _min.IsReady;


        /// <summary>
        /// Required period, in data points, for the indicator to be ready and fully initialized.
        /// </summary>
        public int WarmUpPeriod => _rsi.WarmUpPeriod;
        public int WarmUpPeriod => _max.WarmUpPeriod;
        public int WarmUpPeriod => _min.WarmUpPeriod;

        /// <summary>
        /// Initializeds a new instance of the EaseOfMovement class using the specufued period
        /// </summary>
        /// <param name="period">The period over which to perform to computation</param>
        /// <param name="movingAverageType">Moving average of RSI</param>
        public StochasticRelativeStrengthIndex(int period = 14, MovingAverageType movingAverageType = MovingAverageType.Simple)
            : this($"STORSI({period}, {movingAverageType})", period, movingAverageType)
        {
        }
        /// <summary>
        /// Creates a new EaseOfMovement indicator with the specified period
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        /// <param name="period">The period over which to perform to computation</param>
        /// <param name="movingAverageType">Moving average of RSI</param>
        public StochasticRelativeStrengthIndex(string name, int period, MovingAverageType movingAverageType)
            : base(name)
        {
            _rsi = new RelativeStrengthIndex(period, movingAverageType);
            _max = new Maximum(period);
            _min = new Minimum(period);
        }

        /// <summary>
        /// Computes the next value for this indicator from the given state.
        /// </summary>
        /// <param name="input">The input value to this indicator on this time step</param>
        /// <returns>A a value for this indicator</returns>
        protected override decimal ComputeNextValue(TradeBar input)
        {
            var numerator = 0;
            var denominator = 0;
            //
            // Need to fix if statement below
            //
            if (input.Volume == 0 || input.High == input.Low)
            {
                _rsi.Update(input.Time, 0);
                _max.Update(input.Time, _rsi.Current.Value);
                _min.Update(input.Time, _rsi.Current.Value);
                return _rsi.Current.Value;
            }

            _rsi.Update(input.Time, input.Close);
            _max.Update(input.Time, _rsi.Current.Value);
            _min.Update(input.Time, _rsi.Current.Value);

            numerator = _rsi.Current.Value / _min.Current.Value;
            denominator = _max.Current.Value / _min.Current.Value;

            var stochRsiValue = numerator / denominator;

            return stochRsiValue;
        }

        /// <summary>
        /// Resets this indicator to its initial state
        /// </summary>
        public override void Reset()
        {
            _rsi.Reset();
            _max.Reset();
            _min.Reset();
            base.Reset();
        }
    }
}
