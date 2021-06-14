package mvvmcross.platforms.android.views.appcompat;


public class MvxActionBarDrawerToggle
	extends androidx.appcompat.app.ActionBarDrawerToggle
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDrawerClosed:(Landroid/view/View;)V:GetOnDrawerClosed_Landroid_view_View_Handler\n" +
			"n_onDrawerOpened:(Landroid/view/View;)V:GetOnDrawerOpened_Landroid_view_View_Handler\n" +
			"n_onDrawerSlide:(Landroid/view/View;F)V:GetOnDrawerSlide_Landroid_view_View_FHandler\n" +
			"n_onDrawerStateChanged:(I)V:GetOnDrawerStateChanged_IHandler\n" +
			"";
		mono.android.Runtime.register ("MvvmCross.Platforms.Android.Views.AppCompat.MvxActionBarDrawerToggle, MvvmCross", MvxActionBarDrawerToggle.class, __md_methods);
	}


	public MvxActionBarDrawerToggle (android.app.Activity p0, androidx.drawerlayout.widget.DrawerLayout p1, androidx.appcompat.widget.Toolbar p2, int p3, int p4)
	{
		super (p0, p1, p2, p3, p4);
		if (getClass () == MvxActionBarDrawerToggle.class)
			mono.android.TypeManager.Activate ("MvvmCross.Platforms.Android.Views.AppCompat.MvxActionBarDrawerToggle, MvvmCross", "Android.App.Activity, Mono.Android:AndroidX.DrawerLayout.Widget.DrawerLayout, Xamarin.AndroidX.DrawerLayout:AndroidX.AppCompat.Widget.Toolbar, Xamarin.AndroidX.AppCompat:System.Int32, mscorlib:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3, p4 });
	}


	public MvxActionBarDrawerToggle (android.app.Activity p0, androidx.drawerlayout.widget.DrawerLayout p1, int p2, int p3)
	{
		super (p0, p1, p2, p3);
		if (getClass () == MvxActionBarDrawerToggle.class)
			mono.android.TypeManager.Activate ("MvvmCross.Platforms.Android.Views.AppCompat.MvxActionBarDrawerToggle, MvvmCross", "Android.App.Activity, Mono.Android:AndroidX.DrawerLayout.Widget.DrawerLayout, Xamarin.AndroidX.DrawerLayout:System.Int32, mscorlib:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public void onDrawerClosed (android.view.View p0)
	{
		n_onDrawerClosed (p0);
	}

	private native void n_onDrawerClosed (android.view.View p0);


	public void onDrawerOpened (android.view.View p0)
	{
		n_onDrawerOpened (p0);
	}

	private native void n_onDrawerOpened (android.view.View p0);


	public void onDrawerSlide (android.view.View p0, float p1)
	{
		n_onDrawerSlide (p0, p1);
	}

	private native void n_onDrawerSlide (android.view.View p0, float p1);


	public void onDrawerStateChanged (int p0)
	{
		n_onDrawerStateChanged (p0);
	}

	private native void n_onDrawerStateChanged (int p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
