using System.Runtime.InteropServices;

namespace FuseSharp.Native
{
    /// <summary>
    /// Represents the Darwin <c>struct timespec</c> (16 bytes).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TimeSpec
    {
        public long tv_sec;
        public long tv_nsec;

        /// <summary>
        /// Set the file time to the current time. See <c>UTIME_NOW</c> in utimensat(2).
        /// </summary>
        public const long UTIME_NOW = -1;

        /// <summary>
        /// Leave the file time unchanged. See <c>UTIME_OMIT</c> in utimensat(2).
        /// </summary>
        public const long UTIME_OMIT = -2;

        public readonly bool IsNow => tv_nsec == UTIME_NOW;

        public readonly bool IsOmit => tv_nsec == UTIME_OMIT;

        public static TimeSpec FromDateTime(DateTime dateTime)
        {
            var dto = new DateTimeOffset(dateTime.ToUniversalTime());
            var ticks = dto.UtcTicks - DateTimeOffset.UnixEpoch.UtcTicks;
            return new TimeSpec()
            {
                tv_sec = ticks / TimeSpan.TicksPerSecond,
                tv_nsec = ticks % TimeSpan.TicksPerSecond * 100L
            };
        }

        public readonly DateTime ToDateTime()
        {
            if (IsNow)
                return DateTime.UtcNow;

            var ticks = tv_sec * TimeSpan.TicksPerSecond + tv_nsec / 100L;
            return (DateTimeOffset.UnixEpoch + TimeSpan.FromTicks(ticks)).UtcDateTime;
        }
    }
}
