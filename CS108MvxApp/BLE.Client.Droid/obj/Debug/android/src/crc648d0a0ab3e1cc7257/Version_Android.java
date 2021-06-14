package crc648d0a0ab3e1cc7257;


public class Version_Android
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("BLE.Client.Droid.Version_Android, BLE.Client.Droid", Version_Android.class, __md_methods);
	}


	public Version_Android ()
	{
		super ();
		if (getClass () == Version_Android.class)
			mono.android.TypeManager.Activate ("BLE.Client.Droid.Version_Android, BLE.Client.Droid", "", this, new java.lang.Object[] {  });
	}

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
