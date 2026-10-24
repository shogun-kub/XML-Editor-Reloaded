using XmlEditorReloaded.Core.Imaging;

namespace XmlEditorReloaded.Imaging.Cache;

/// <summary>
/// Size-bounded LRU cache for decoded RGBA frames, invalidated by file modification time.
/// </summary>
public sealed class DecodedFrameCache
{
    private readonly long _capacityBytes;
    private readonly Dictionary<string, LinkedListNode<Entry>> _entries = new(StringComparer.Ordinal);
    private readonly LinkedList<Entry> _lru = new();
    private readonly object _gate = new();
    private long _totalBytes;

    /// <summary>
    /// Initializes a new instance of the <see cref="DecodedFrameCache"/> class.
    /// </summary>
    /// <param name="capacityBytes">Maximum total pixel-buffer size in bytes; must be positive.</param>
    public DecodedFrameCache(long capacityBytes = 128L * 1024 * 1024)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacityBytes);
        _capacityBytes = capacityBytes;
    }

    /// <summary>
    /// Gets the maximum cache size in bytes.
    /// </summary>
    public long CapacityBytes => _capacityBytes;

    /// <summary>
    /// Gets the number of cached frames.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_gate)
            {
                return _entries.Count;
            }
        }
    }

    /// <summary>
    /// Gets the total size of cached pixel buffers in bytes.
    /// </summary>
    public long TotalBytes
    {
        get
        {
            lock (_gate)
            {
                return _totalBytes;
            }
        }
    }

    /// <summary>
    /// Attempts to retrieve a cached frame for a given file modification time.
    /// </summary>
    /// <param name="key">Cache key, usually path plus frame index.</param>
    /// <param name="lastWriteTimeUtc">Current file modification time.</param>
    /// <param name="buffer">Cached frame when available and still fresh.</param>
    /// <returns><see langword="true"/> when a fresh cached frame was found.</returns>
    public bool TryGet(string key, DateTime lastWriteTimeUtc, out RgbaBuffer? buffer)
    {
        ArgumentNullException.ThrowIfNull(key);

        lock (_gate)
        {
            if (!_entries.TryGetValue(key, out var node))
            {
                buffer = null;
                return false;
            }

            if (node.Value.LastWriteTimeUtc != lastWriteTimeUtc)
            {
                Remove(node);
                buffer = null;
                return false;
            }

            _lru.Remove(node);
            _lru.AddFirst(node);
            buffer = node.Value.Buffer;
            return true;
        }
    }

    /// <summary>
    /// Adds or replaces a decoded frame in the cache.
    /// </summary>
    /// <param name="key">Cache key, usually path plus frame index.</param>
    /// <param name="buffer">Decoded RGBA frame.</param>
    /// <param name="lastWriteTimeUtc">File modification time the frame was decoded from.</param>
    /// <exception cref="ArgumentNullException">Thrown when a parameter is null.</exception>
    public void Set(string key, RgbaBuffer buffer, DateTime lastWriteTimeUtc)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(buffer);

        lock (_gate)
        {
            if (_entries.TryGetValue(key, out var existing))
            {
                Remove(existing);
            }

            var size = buffer.Pixels.LongLength;
            if (size > _capacityBytes)
            {
                return;
            }

            var node = _lru.AddFirst(new Entry(key, buffer, lastWriteTimeUtc, size));
            _entries[key] = node;
            _totalBytes += size;

            while (_totalBytes > _capacityBytes && _lru.Last is { } last)
            {
                Remove(last);
            }
        }
    }

    /// <summary>
    /// Removes all cached frames.
    /// </summary>
    public void Clear()
    {
        lock (_gate)
        {
            _entries.Clear();
            _lru.Clear();
            _totalBytes = 0;
        }
    }

    private void Remove(LinkedListNode<Entry> node)
    {
        _lru.Remove(node);
        _entries.Remove(node.Value.Key);
        _totalBytes -= node.Value.SizeBytes;
    }

    private sealed record Entry(string Key, RgbaBuffer Buffer, DateTime LastWriteTimeUtc, long SizeBytes);
}
