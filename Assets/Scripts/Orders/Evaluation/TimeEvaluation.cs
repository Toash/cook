using System;

public class TimeEvaluation
{
    public bool EarnedFastDelivery { get; private set; }
    public float TimeRemainingRatio { get; private set; } // optional but useful

    public TimeEvaluation(
        float completionTimeSeconds,
        float timeLimitSeconds,
        float fastThresholdRatio)
    {
        float remaining = Math.Max(0f, timeLimitSeconds - completionTimeSeconds);

        TimeRemainingRatio = timeLimitSeconds > 0f
            ? Math.Clamp(remaining / timeLimitSeconds, 0f, 1f)
            : 0f;

        EarnedFastDelivery = completionTimeSeconds <= timeLimitSeconds
                             && TimeRemainingRatio >= fastThresholdRatio;
    }
}