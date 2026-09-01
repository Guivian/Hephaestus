using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Net;
using Android.Views;
using Android.Widget;

namespace hephaestus_android;

[Application]
public sealed class HephaestusApplication : Application, Application.IActivityLifecycleCallbacks
{
    ConnectivityManager? connectivityManager;
    NetworkCallback? networkCallback;
    Activity? currentActivity;
    bool? lastOnline;

    public HephaestusApplication(IntPtr handle, Android.Runtime.JniHandleOwnership ownership) : base(handle, ownership) { }

    public override void OnCreate()
    {
        base.OnCreate();
        RegisterActivityLifecycleCallbacks(this);
        connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService)!;
        networkCallback = new NetworkCallback(this);
        connectivityManager.RegisterDefaultNetworkCallback(networkCallback);
        RefreshConnectivity();
    }

    void RefreshConnectivity()
    {
        var network = connectivityManager?.ActiveNetwork;
        var capabilities = network is null ? null : connectivityManager?.GetNetworkCapabilities(network);
        var online = capabilities?.HasCapability(NetCapability.Internet) == true &&
                     capabilities.HasCapability(NetCapability.Validated);
        var changed = lastOnline != online;
        lastOnline = online;

        var activity = currentActivity;
        if (activity is null || (!changed && online)) return;
        activity.RunOnUiThread(() => ShowStatus(activity, online));
    }

    static void ShowStatus(Activity activity, bool online)
    {
        var content = activity.FindViewById<FrameLayout>(Android.Resource.Id.Content);
        if (content is null) return;

        var existing = content.FindViewWithTag("network-status-banner");
        if (existing is not null) content.RemoveView(existing);
        if (online) return;

        var banner = new TextView(activity)
        {
            Tag = "network-status-banner",
            Text = "Sem ligação à rede  ·  Verifique a Internet",
            TextSize = 14,
            Gravity = GravityFlags.Center,
            Typeface = Typeface.DefaultBold,
            Elevation = Dp(activity, 12)
        };
        banner.SetTextColor(Color.White);
        banner.SetPadding(Dp(activity, 16), Dp(activity, 11), Dp(activity, 16), Dp(activity, 11));
        var background = new GradientDrawable();
        background.SetColor(Color.ParseColor("#FC424A"));
        background.SetCornerRadius(Dp(activity, 8));
        banner.Background = background;
        banner.ContentDescription = "Alerta: sem ligação à rede";

        var parameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, GravityFlags.Top)
        {
            LeftMargin = Dp(activity, 12),
            RightMargin = Dp(activity, 12),
            TopMargin = Dp(activity, 10)
        };
        content.AddView(banner, parameters);
    }

    static int Dp(Context context, int value) => (int)(value * context.Resources!.DisplayMetrics!.Density + .5f);

    public void OnActivityResumed(Activity activity) { currentActivity = activity; RefreshConnectivity(); }
    public void OnActivityPaused(Activity activity) { if (currentActivity == activity) currentActivity = null; }
    public void OnActivityCreated(Activity activity, Bundle? state)
    {
        // Android recente desenha as apps debaixo das barras do sistema por omissão.
        // Aplicar os insets no contentor raiz preserva toda a área útil em todos os ecrãs.
        var content = activity.FindViewById<FrameLayout>(Android.Resource.Id.Content);
        if (content is null) return;

        content.SetOnApplyWindowInsetsListener(new SystemBarsInsetsListener());
        content.RequestApplyInsets();
    }
    public void OnActivityStarted(Activity activity) { }
    public void OnActivityStopped(Activity activity) { }
    public void OnActivitySaveInstanceState(Activity activity, Bundle state) { }
    public void OnActivityDestroyed(Activity activity) { if (currentActivity == activity) currentActivity = null; }

    sealed class NetworkCallback(HephaestusApplication owner) : ConnectivityManager.NetworkCallback
    {
        public override void OnAvailable(Network network) => owner.RefreshConnectivity();
        public override void OnLost(Network network) => owner.RefreshConnectivity();
        public override void OnCapabilitiesChanged(Network network, NetworkCapabilities capabilities) => owner.RefreshConnectivity();
    }

    sealed class SystemBarsInsetsListener : Java.Lang.Object, View.IOnApplyWindowInsetsListener
    {
        public WindowInsets OnApplyWindowInsets(View view, WindowInsets insets)
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(30))
            {
                var bars = insets.GetInsets(WindowInsets.Type.SystemBars());
                view.SetPadding(bars.Left, bars.Top, bars.Right, bars.Bottom);
            }
            else
            {
#pragma warning disable CA1422
                view.SetPadding(insets.SystemWindowInsetLeft, insets.SystemWindowInsetTop,
                    insets.SystemWindowInsetRight, insets.SystemWindowInsetBottom);
#pragma warning restore CA1422
            }

            return insets;
        }
    }
}
