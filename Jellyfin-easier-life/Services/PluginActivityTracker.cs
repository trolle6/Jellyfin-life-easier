using System.Collections.Concurrent;
using MediaBrowser.Controller.Entities;

namespace Jellyfin_easier_life.Services;

/// <summary>
/// Tracks plugin activity and statistics.
/// </summary>
public class PluginActivityTracker
{
    private readonly ConcurrentDictionary<Guid, ActivityRecord> _recentActivity = new();
    private readonly object _statsLock = new object();
    private int _totalItemsProcessed = 0;
    private int _totalItemsSucceeded = 0;
    private int _totalItemsFailed = 0;
    private DateTime? _lastActivityTime = null;
    private DateTime? _pluginStartTime = DateTime.UtcNow;

    /// <summary>
    /// Records that an item was processed.
    /// </summary>
    public void RecordItemProcessed(BaseItem item, bool success)
    {
        lock (_statsLock)
        {
            _totalItemsProcessed++;
            if (success)
            {
                _totalItemsSucceeded++;
            }
            else
            {
                _totalItemsFailed++;
            }
            _lastActivityTime = DateTime.UtcNow;
        }

        // Store recent activity (keep last 100 items)
        var record = new ActivityRecord
        {
            ItemId = item.Id,
            ItemName = item.Name,
            ItemType = item.GetType().Name,
            ProcessedAt = DateTime.UtcNow,
            Success = success
        };

        _recentActivity.AddOrUpdate(item.Id, record, (key, oldValue) => record);

        // Clean up old records if we have too many
        if (_recentActivity.Count > 100)
        {
            var oldest = _recentActivity.Values.OrderBy(r => r.ProcessedAt).First();
            _recentActivity.TryRemove(oldest.ItemId, out _);
        }
    }

    /// <summary>
    /// Gets the plugin statistics.
    /// </summary>
    public PluginStatistics GetStatistics()
    {
        lock (_statsLock)
        {
            return new PluginStatistics
            {
                TotalItemsProcessed = _totalItemsProcessed,
                TotalItemsSucceeded = _totalItemsSucceeded,
                TotalItemsFailed = _totalItemsFailed,
                LastActivityTime = _lastActivityTime,
                PluginStartTime = _pluginStartTime,
                RecentActivity = _recentActivity.Values
                    .OrderByDescending(r => r.ProcessedAt)
                    .Take(20)
                    .ToList()
            };
        }
    }

    /// <summary>
    /// Resets all statistics.
    /// </summary>
    public void Reset()
    {
        lock (_statsLock)
        {
            _totalItemsProcessed = 0;
            _totalItemsSucceeded = 0;
            _totalItemsFailed = 0;
            _lastActivityTime = null;
            _pluginStartTime = DateTime.UtcNow;
        }
        _recentActivity.Clear();
    }
}

/// <summary>
/// Represents a single activity record.
/// </summary>
public class ActivityRecord
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
    public bool Success { get; set; }
}

/// <summary>
/// Plugin statistics.
/// </summary>
public class PluginStatistics
{
    public int TotalItemsProcessed { get; set; }
    public int TotalItemsSucceeded { get; set; }
    public int TotalItemsFailed { get; set; }
    public DateTime? LastActivityTime { get; set; }
    public DateTime? PluginStartTime { get; set; }
    public List<ActivityRecord> RecentActivity { get; set; } = new();
}

