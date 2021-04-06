﻿using System.Collections.Generic;
using System.Threading.Tasks;
using OSItemIndex.Aggregator.OSRSBox.Models;
using OSItemIndex.API.Models;

namespace OSItemIndex.Aggregator.OSRSBox
{
    public interface IOsrsBoxService
    {
        Task<IEnumerable<OSRSBoxItem>> GetItemsAsync();
        Task<ReleaseMonitoringProject> GetReleaseMonitoringProjectAsync();
    }
}
