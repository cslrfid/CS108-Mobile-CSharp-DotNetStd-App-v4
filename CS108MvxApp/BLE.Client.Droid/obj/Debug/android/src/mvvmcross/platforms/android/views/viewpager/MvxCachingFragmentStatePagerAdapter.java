package mvvmcross.platforms.android.views.viewpager;


public class MvxCachingFragmentStatePagerAdapter
	extends mvvmcross.platforms.android.views.viewpager.MvxCachingFragmentPagerAdapter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getCount:()I:GetGetCountHandler\n" +
			"n_getItemPosition:(Ljava/lang/Object;)I:GetGetItemPosition_Ljava_lang_Object_Handler\n" +
			"n_getPageTitle:(I)Ljava/lang/CharSequence;:GetGetPageTitle_IHandler\n" +
			"";
		mono.android.Runtime.register ("MvvmCross.Platforms.Android.Views.ViewPager.MvxCachingFragmentStatePagerAdapter, MvvmCross", MvxCachingFragmentStatePagerAdapter.class, __md_methods);
	}


	public MvxCachingFragmentStatePagerAdapter ()
	{
		super ();
		if (getClass () == MvxCachingFragmentStatePagerAdapter.class)
			mono.android.TypeManager.Activate ("MvvmCross.Platforms.Android.Views.ViewPager.MvxCachingFragmentStatePagerAdapter, MvvmCross", "", this, new java.lang.Object[] {  });
	}

	public MvxCachingFragmentStatePagerAdapter (androidx.fragment.app.FragmentManager p0)
	{
		super ();
		if (getClass () == MvxCachingFragmentStatePagerAdapter.class)
			mono.android.TypeManager.Activate ("MvvmCross.Platforms.Android.Views.ViewPager.MvxCachingFragmentStatePagerAdapter, MvvmCross", "AndroidX.Fragment.App.FragmentManager, Xamarin.AndroidX.Fragment", this, new java.lang.Object[] { p0 });
	}


	public int getCount ()
	{
		return n_getCount ();
	}

	private native int n_getCount ();


	public int getItemPosition (java.lang.Object p0)
	{
		return n_getItemPosition (p0);
	}

	private native int n_getItemPosition (java.lang.Object p0);


	public java.lang.CharSequence getPageTitle (int p0)
	{
		return n_getPageTitle (p0);
	}

	private native java.lang.CharSequence n_getPageTitle (int p0);

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
