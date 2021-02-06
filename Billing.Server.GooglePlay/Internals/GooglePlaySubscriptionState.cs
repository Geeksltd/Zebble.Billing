namespace Zebble.Billing
{
    enum GooglePlaySubscriptionState
    {
        /// <summary>
        /// A subscription was recovered from account hold.
        /// </summary>
        Recovered = 1,
        /// <summary>
        /// An active subscription was renewed.
        /// </summary>
        Renewed = 2,
        /// <summary>
        /// A subscription was either voluntarily or involuntarily cancelled.For voluntary cancellation, sent when the user cancels.
        /// </summary>
        Canceled = 3,
        /// <summary>
        /// A new subscription was purchased.
        /// </summary>
        Purchased = 4,
        /// <summary>
        /// A subscription has entered account hold (if enabled).
        /// </summary>
        OnHold = 5,
        /// <summary>
        /// A subscription has entered grace period (if enabled).
        /// </summary>
        InGracePeriod = 6,
        /// <summary>
        /// User has reactivated their subscription from Play > Account > Subscriptions (requires opt-in for subscription restoration)
        /// </summary>
        Restarted = 7,
        /// <summary>
        /// A subscription price change has successfully been confirmed by the user.
        /// </summary>
        PriceChangeConfirmed = 8,
        /// <summary>
        /// A subscription's recurrence time has been extended.
        /// </summary>
        Deferred = 9,
        /// <summary>
        /// A subscription has been paused.
        /// </summary>
        Paused = 10,
        /// <summary>
        /// A subscription pause schedule has been changed.
        /// </summary>
        PauseScheduleChanged = 11,
        /// <summary>
        /// A subscription has been revoked from the user before the expiration time.
        /// </summary>
        Revoked = 12,
        /// <summary>
        /// A subscription has expired.
        /// </summary>
        Expired = 13
    }

}
