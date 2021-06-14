package crc64b76f6e8b2d8c8db1;


public class LoginDialogFragment
	extends crc64b76f6e8b2d8c8db1.AbstractDialogFragment_1
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Acr.UserDialogs.Fragments.LoginDialogFragment, Acr.UserDialogs", LoginDialogFragment.class, __md_methods);
	}


	public LoginDialogFragment ()
	{
		super ();
		if (getClass () == LoginDialogFragment.class)
			mono.android.TypeManager.Activate ("Acr.UserDialogs.Fragments.LoginDialogFragment, Acr.UserDialogs", "", this, new java.lang.Object[] {  });
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
