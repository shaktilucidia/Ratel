// Ratel - Opensource federated messenger
// Copyright (C) 2026 Shakti Lucidia
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ratel_shared_observability.Metrics;

/// <summary>
/// Use it to measure the time taken by a block of code and record it in a histogram metric
/// </summary>
public sealed class MetricsTimer : IDisposable
{
    private readonly Histogram<double> _histogram;
    private readonly Stopwatch _stopwatch;
    private readonly TagList _tags;

    public MetricsTimer
    (
        Histogram<double> histogram,
        TagList tags = default
    )
    {
        _histogram = histogram;
        _tags = tags;
         _stopwatch = Stopwatch.StartNew();
    }

    public void Dispose()
    {
        _stopwatch.Stop();

        _histogram.Record(_stopwatch.Elapsed.TotalMilliseconds, _tags);
    }
}
