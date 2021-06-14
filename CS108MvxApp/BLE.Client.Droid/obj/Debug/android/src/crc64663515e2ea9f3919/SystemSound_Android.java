package crc64663515e2ea9f3919;


public class SystemSound_Android
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("BLE.Clinet.Droid.SystemSound_Android, BLE.Client.Droid", SystemSound_Android.class, __md_methods);
	}


	public SystemSound_Android ()
	{
		super ();
		if (getClass () == SystemSound_Android.class)
			mono.android.TypeManager.Activate ("BLE.Clinet.Droid.SystemSound_Android, BLE.Client.Droid", "", this, new java.lang.Object[] {  });
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
