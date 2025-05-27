namespace OrdrMate.Enums;

public enum OrderStatus
{
    Pending = 0,
    Queued = 1,
    InProgress = 2,
    Ready = 3,
    Delivered = 4,
    Cancelled = -1
}